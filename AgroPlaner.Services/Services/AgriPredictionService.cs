using AgroPlaner.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgroPlaner.Services.Services
{
    public class AgriPredictionService
    {
        private const double PSYCHROMETRIC_CONSTANT = 0.066;
        private const double MIN_NUTRIENT_THRESHOLD = 0.1;
        private const double MAX_PRECIPITATION_THRESHOLD = 20.0;
        private const double DEFAULT_DAILY_UPTAKE_FRACTION = 0.1;
        private const double IRRIGATION_EFFICIENCY = 0.9;
        private const double SOIL_MOISTURE_TARGET_FRACTION = 0.8;
        private const double SOIL_TEMP_MEMORY_FACTOR = 0.7;
        private const double AIR_TEMP_INFLUENCE_FACTOR = 0.3;
        private const double SOLAR_RADIATION_GROUND_FLUX = 0.0;
        private const double ETO_TEMP_OFFSET = 273.0;
        private const double ETO_WIND_FACTOR = 900.0;
        private const double ETO_SLOPE_FACTOR = 0.34;
        private const double VAPOR_PRESSURE_COEFFICIENT = 0.6108;
        private const double VAPOR_PRESSURE_EXPONENT_FACTOR = 17.27;
        private const double VAPOR_PRESSURE_TEMP_OFFSET = 237.3;
        private const double DELTA_COEFFICIENT = 0.409;
        private const double DELTA_ANGLE_FACTOR = 0.0172;
        private const double DELTA_OFFSET = 0.5;
        private const double NITROGEN_EFFICIENCY = 0.9;
        private const double PHOSPHORUS_EFFICIENCY = 0.8;
        private const double POTASSIUM_EFFICIENCY = 0.85;
        private const double TAW_CONVERSION_FACTOR = 1000.0;
        private const double PERCENTAGE_CONVERSION_FACTOR = 100.0;
        private const double AVERAGE_DAILY_GDD = 10.0; // Average GDD per day for nutrient uptake

        public void UpdateCumulativeGDD(
            Crop crop,
            DateTime plantingDate,
            List<WeatherData> weatherHistory
        )
        {
            if (crop == null || weatherHistory == null || crop.Plant == null)
            {
                Console.WriteLine("Error: Invalid crop or weather history data.");
                return;
            }

            double cumulativeGDD = 0;
            foreach (var weather in weatherHistory.Where(w => w.Date >= plantingDate && w.Date <= DateTime.Today))
            {
                if (weather.MaxTemp < weather.MinTemp)
                {
                    Console.WriteLine($"Warning: Invalid temperature data for {weather.Date}: MaxTemp={weather.MaxTemp}, MinTemp={weather.MinTemp}");
                    continue;
                }
                double dailyGDD = Math.Max((weather.MaxTemp + weather.MinTemp) / 2 - crop.Plant.BaseTempForGDD, 0);
                cumulativeGDD += dailyGDD;
            }
            crop.CumulativeGDDToday = cumulativeGDD;
            Console.WriteLine($"Updated Cumulative GDD: {cumulativeGDD:F2}");
        }

        public DateTime? PredictSeedingDate(
            Crop crop,
            List<WeatherData> weatherForecast,
            SoilData soil
        )
        {
            if (crop == null || crop.Plant == null || weatherForecast == null || soil == null)
            {
                Console.WriteLine("Error: Invalid input data for seeding date prediction.");
                return null;
            }

            double currentSoilTemp = soil.Temperature;
            foreach (var weather in weatherForecast.OrderBy(w => w.Date))
            {
                double avgAirTemp = (weather.MaxTemp + weather.MinTemp) / 2;
                currentSoilTemp = SOIL_TEMP_MEMORY_FACTOR * currentSoilTemp + AIR_TEMP_INFLUENCE_FACTOR * avgAirTemp;
                if (currentSoilTemp >= crop.Plant.MinSoilTempForSeeding && weather.Precipitation < MAX_PRECIPITATION_THRESHOLD)
                {
                    Console.WriteLine($"Seeding date predicted: {weather.Date}, Soil Temp: {currentSoilTemp:F2}");
                    return weather.Date;
                }
            }
            Console.WriteLine("No suitable seeding date found in forecast.");
            return null;
        }

        private double CalculateETo(WeatherData weather)
        {
            if (weather == null)
            {
                Console.WriteLine("Error: Null weather data for ETo calculation.");
                return 0;
            }

            double T = (weather.MaxTemp + weather.MinTemp) / 2;
            double delta = DELTA_COEFFICIENT * Math.Sin(DELTA_ANGLE_FACTOR * T + DELTA_OFFSET);
            double Rn = weather.SolarRadiation;
            double G = SOLAR_RADIATION_GROUND_FLUX;
            double u2 = weather.WindSpeed;
            double es = VAPOR_PRESSURE_COEFFICIENT * Math.Exp(VAPOR_PRESSURE_EXPONENT_FACTOR * T / (T + VAPOR_PRESSURE_TEMP_OFFSET));
            double ea = es * (weather.RelativeHumidity / PERCENTAGE_CONVERSION_FACTOR);

            double numerator = delta * (Rn - G) + PSYCHROMETRIC_CONSTANT * (ETO_WIND_FACTOR / (T + ETO_TEMP_OFFSET)) * u2 * (es - ea);
            double denominator = delta + PSYCHROMETRIC_CONSTANT * (1 + ETO_SLOPE_FACTOR * u2);
            double ETo = numerator / denominator;
            Console.WriteLine($"Calculated ETo: {ETo:F2} mm/day");
            return ETo;
        }

        public (double Amount, DateTime? NextDate) CalculateIrrigation(
            Crop crop,
            List<WeatherData> weatherHistory,
            WeatherData todayWeather,
            SoilData soil,
            int growthStage
        )
        {
            if (crop == null || crop.Plant == null || weatherHistory == null || todayWeather == null || soil == null)
            {
                Console.WriteLine("Error: Invalid input data for irrigation calculation.");
                return (0, null);
            }

            double ETo = CalculateETo(todayWeather);
            double Kc = crop.Plant.KcValues[growthStage % crop.Plant.KcValues.Length];
            double ETc = Kc * ETo;

            double TAW = (soil.FieldCapacity - crop.Plant.WiltingPoint) * crop.Plant.RootDepth * TAW_CONVERSION_FACTOR;
            double AD = crop.Plant.AllowableDepletionFraction * TAW;

            double moisture = soil.CurrentMoisture;
            foreach (var weather in weatherHistory.Where(w => w.Date > soil.IrrigationHistory.LastOrDefault()?.Date && w.Date <= DateTime.Today))
            {
                moisture += weather.Precipitation - CalculateETo(weather) * Kc;
                moisture = Math.Max(Math.Min(moisture, soil.FieldCapacity), crop.Plant.WiltingPoint);
            }

            double waterDeficit = soil.FieldCapacity * SOIL_MOISTURE_TARGET_FRACTION - (moisture + todayWeather.Precipitation - ETc);
            Console.WriteLine($"Water Deficit: {waterDeficit:F2}, Current Moisture: {moisture:F2}, ETc: {ETc:F2}");

            if (waterDeficit > 0)
            {
                double irrigationAmount = waterDeficit / IRRIGATION_EFFICIENCY; // mm/ha
                Console.WriteLine($"Irrigation needed: {irrigationAmount:F2} mm/ha on {DateTime.Today}");
                return (irrigationAmount, DateTime.Today);
            }

            double daysUntilNext = (moisture - (soil.FieldCapacity - AD)) / ETc;
            DateTime? nextIrrigationDate = daysUntilNext > 0 ? DateTime.Today.AddDays((int)Math.Ceiling(daysUntilNext)) : null;
            Console.WriteLine($"No irrigation needed today. Next irrigation: {nextIrrigationDate?.ToString() ?? "None"}");
            return (0, nextIrrigationDate);
        }

        public string GetCurrentGrowthStage(Crop crop)
        {
            if (crop == null || crop.Plant == null)
            {
                Console.WriteLine("Error: Invalid crop data for growth stage.");
                return "Unknown";
            }

            double cumulativeGDD = crop.CumulativeGDDToday;
            foreach (var stage in crop.Plant.PlantGrowthStages)
            {
                if (cumulativeGDD >= stage.MinGDD && cumulativeGDD <= stage.MaxGDD)
                {
                    Console.WriteLine($"Current Growth Stage: {stage.StageName}, GDD: {cumulativeGDD:F2}");
                    return stage.StageName;
                }
            }
            string stageName = cumulativeGDD > crop.Plant.MaturityGDD ? "Mature" : "Not yet emerged";
            Console.WriteLine($"Current Growth Stage: {stageName}, GDD: {cumulativeGDD:F2}");
            return stageName;
        }

        public int GetGrowthStageIndex(string stageName, Crop crop)
        {
            if (crop == null || crop.Plant == null || string.IsNullOrEmpty(stageName))
            {
                Console.WriteLine("Error: Invalid stage or crop data.");
                return -1;
            }

            var stages = crop.Plant.PlantGrowthStages.OrderBy(s => s.OrderIndex).ToList();
            int index = stages.FindIndex(s => s.StageName == stageName);
            Console.WriteLine($"Growth Stage Index for {stageName}: {index}");
            return index;
        }

        private (double N, double P, double K) EstimateDailyNutrientUptake(
            Crop crop,
            string currentStage,
            double totalNRequired,
            double totalPRequired,
            double totalKRequired,
            List<WeatherData> weatherHistory
        )
        {
            if (crop == null || crop.Plant == null || string.IsNullOrEmpty(currentStage))
            {
                Console.WriteLine("Error: Invalid input for nutrient uptake calculation.");
                return (0, 0, 0);
            }

            int stageIndex = GetGrowthStageIndex(currentStage, crop);
            if (stageIndex == -1)
            {
                Console.WriteLine("Warning: Unknown stage, using default uptake fraction.");
                return (
                    totalNRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD * AVERAGE_DAILY_GDD,
                    totalPRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD * AVERAGE_DAILY_GDD,
                    totalKRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD * AVERAGE_DAILY_GDD
                );
            }

            double nFraction = crop.Plant.NitrogenUptakeDistribution[stageIndex];
            double pFraction = crop.Plant.PhosphorusUptakeDistribution[stageIndex];
            double kFraction = crop.Plant.PotassiumUptakeDistribution[stageIndex];

            var stageRange = crop.Plant.PlantGrowthStages.FirstOrDefault(s => s.StageName == currentStage);
            if (stageRange == null)
            {
                Console.WriteLine("Warning: Stage range not found, using default uptake.");
                return (
                    totalNRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD * AVERAGE_DAILY_GDD,
                    totalPRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD * AVERAGE_DAILY_GDD,
                    totalKRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD * AVERAGE_DAILY_GDD
                );
            }

            double stageGDD = stageRange.MaxGDD - stageRange.MinGDD;
            // Convert uptake per GDD to uptake per day using average daily GDD
            double dailyN = totalNRequired * nFraction / stageGDD * AVERAGE_DAILY_GDD;
            double dailyP = totalPRequired * pFraction / stageGDD * AVERAGE_DAILY_GDD;
            double dailyK = totalKRequired * kFraction / stageGDD * AVERAGE_DAILY_GDD;

            Console.WriteLine($"Daily Nutrient Uptake - N: {dailyN:F2}, P: {dailyP:F2}, K: {dailyK:F2} kg/ha/day");
            return (dailyN, dailyP, dailyK);
        }

        public (
            double NAmount,
            double PAmount,
            double KAmount,
            DateTime? NextDate
        ) CalculateFertilization(
            Crop crop,
            SoilData soil,
            DateTime plantingDate,
            List<WeatherData> weatherHistory,
            List<WeatherData> weatherForecast
        )
        {
            if (crop == null || crop.Plant == null || soil == null || weatherHistory == null || weatherForecast == null)
            {
                Console.WriteLine("Error: Invalid input for fertilization calculation.");
                return (0, 0, 0, null);
            }

            double totalNRequired = crop.ExpectedYield * crop.Plant.NitrogenContent; // kg/ha
            double totalPRequired = crop.ExpectedYield * crop.Plant.PhosphorusContent; // kg/ha
            double totalKRequired = crop.ExpectedYield * crop.Plant.PotassiumContent; // kg/ha

            double currentN = Math.Max(soil.AvailableNitrogen, 0);
            double currentP = Math.Max(soil.AvailablePhosphorus, 0);
            double currentK = Math.Max(soil.AvailablePotassium, 0);

            string currentStage = GetCurrentGrowthStage(crop);
            var (dailyN, dailyP, dailyK) = EstimateDailyNutrientUptake(
                crop, currentStage, totalNRequired, totalPRequired, totalKRequired, weatherHistory);

            DateTime lastFertDate = soil.FertilizationHistory.LastOrDefault()?.Date ?? plantingDate;
            double daysSinceLastFert = (DateTime.Today - lastFertDate).Days;
            currentN = Math.Max(currentN - dailyN * daysSinceLastFert, 0);
            currentP = Math.Max(currentP - dailyP * daysSinceLastFert, 0);
            currentK = Math.Max(currentK - dailyK * daysSinceLastFert, 0);

            Console.WriteLine($"Current Nutrients - N: {currentN:F2}, P: {currentP:F2}, K: {currentK:F2} kg/ha");
            Console.WriteLine($"Days since last fertilization: {daysSinceLastFert:F2}");

            double remainingN = totalNRequired - currentN;
            double remainingP = totalPRequired - currentP;
            double remainingK = totalKRequired - currentK;

            if (remainingN <= 0 && remainingP <= 0 && remainingK <= 0)
            {
                Console.WriteLine("No fertilization needed.");
                return (0, 0, 0, null);
            }

            double daysNDepletion = currentN > 0 ? (currentN - totalNRequired * MIN_NUTRIENT_THRESHOLD) / dailyN : 0;
            double daysPDepletion = currentP > 0 ? (currentP - totalPRequired * MIN_NUTRIENT_THRESHOLD) / dailyP : 0;
            double daysKDepletion = currentK > 0 ? (currentK - totalKRequired * MIN_NUTRIENT_THRESHOLD) / dailyK : 0;

            double minDays = Math.Min(Math.Min(daysNDepletion, daysPDepletion), daysKDepletion);
            DateTime depletionDate = DateTime.Today.AddDays((int)Math.Ceiling(minDays));

            Console.WriteLine($"Depletion days - N: {daysNDepletion:F2}, P: {daysPDepletion:F2}, K: {daysKDepletion:F2}");
            Console.WriteLine($"Earliest depletion date: {depletionDate}");

            foreach (var weather in weatherForecast.Where(w => w.Date >= depletionDate).OrderBy(w => w.Date))
            {
                if (weather.Precipitation < MAX_PRECIPITATION_THRESHOLD)
                {
                    double nAmount = remainingN > 0 ? remainingN * PERCENTAGE_CONVERSION_FACTOR / crop.Plant.FertilizerNitrogenPercentage / NITROGEN_EFFICIENCY : 0;
                    double pAmount = remainingP > 0 ? remainingP * PERCENTAGE_CONVERSION_FACTOR / crop.Plant.FertilizerPhosphorusPercentage / PHOSPHORUS_EFFICIENCY : 0;
                    double kAmount = remainingK > 0 ? remainingK * PERCENTAGE_CONVERSION_FACTOR / crop.Plant.FertilizerPotassiumPercentage / POTASSIUM_EFFICIENCY : 0;
                    Console.WriteLine($"Fertilization scheduled: N={nAmount:F2}, P={pAmount:F2}, K={kAmount:F2} kg/ha on {weather.Date}");
                    return (nAmount, pAmount, kAmount, weather.Date);
                }
            }

            double fallbackNAmount = remainingN > 0 ? remainingN * PERCENTAGE_CONVERSION_FACTOR / crop.Plant.FertilizerNitrogenPercentage / NITROGEN_EFFICIENCY : 0;
            double fallbackPAmount = remainingP > 0 ? remainingP * PERCENTAGE_CONVERSION_FACTOR / crop.Plant.FertilizerPhosphorusPercentage / PHOSPHORUS_EFFICIENCY : 0;
            double fallbackKAmount = remainingK > 0 ? remainingK * PERCENTAGE_CONVERSION_FACTOR / crop.Plant.FertilizerPotassiumPercentage / POTASSIUM_EFFICIENCY : 0;
            Console.WriteLine($"Fallback fertilization: N={fallbackNAmount:F2}, P={fallbackPAmount:F2}, K={fallbackKAmount:F2} kg/ha on {depletionDate}");
            return (fallbackNAmount, fallbackPAmount, fallbackKAmount, depletionDate);
        }

        public DateTime? PredictHarvestDate(Crop crop, List<WeatherData> weatherForecast)
        {
            if (crop == null || crop.Plant == null || weatherForecast == null)
            {
                Console.WriteLine("Error: Invalid input for harvest date prediction.");
                return null;
            }

            double cumulativeGDD = crop.CumulativeGDDToday;
            foreach (var weather in weatherForecast.Where(w => w.Date > DateTime.Today))
            {
                double dailyGDD = Math.Max((weather.MaxTemp + weather.MinTemp) / 2 - crop.Plant.BaseTempForGDD, 0);
                cumulativeGDD += dailyGDD;
                if (cumulativeGDD >= crop.Plant.MaturityGDD)
                {
                    Console.WriteLine($"Harvest date predicted: {weather.Date}, GDD: {cumulativeGDD:F2}");
                    return weather.Date;
                }
            }
            Console.WriteLine("No harvest date predicted within forecast period.");
            return null;
        }

        public PredictionResult Predict(
            Crop crop,
            List<WeatherData> weatherHistory,
            List<WeatherData> weatherForecast,
            DateTime plantingDate
        )
        {
            if (crop == null || weatherHistory == null || weatherForecast == null || weatherForecast.Count == 0)
            {
                Console.WriteLine("Error: Invalid input for prediction.");
                return new PredictionResult();
            }

            UpdateCumulativeGDD(crop, plantingDate, weatherHistory);
            var result = new PredictionResult();
            string currentStage = GetCurrentGrowthStage(crop);
            int growthStageIndex = GetGrowthStageIndex(currentStage, crop);

            var (irrigationAmount, nextIrrigationDate) = CalculateIrrigation(
                crop, weatherHistory, weatherForecast.First(), crop.Soil, growthStageIndex);
            result.IrrigationAmount = irrigationAmount;
            result.NextIrrigationDate = nextIrrigationDate;

            var (nAmount, pAmount, kAmount, nextFertilizationDate) = CalculateFertilization(
                crop, crop.Soil, plantingDate, weatherHistory, weatherForecast);
            result.NitrogenFertilizerAmount = nAmount;
            result.PhosphorusFertilizerAmount = pAmount;
            result.PotassiumFertilizerAmount = kAmount;
            result.NextFertilizationDate = nextFertilizationDate;

            result.PredictedHarvestDate = PredictHarvestDate(crop, weatherForecast);

            Console.WriteLine($"Current Growth Stage: {currentStage}");
            Console.WriteLine($"Cumulative GDD Today: {crop.CumulativeGDDToday:F2}");
            Console.WriteLine($"Fertilization History: {crop.Soil.FertilizationHistory.Count} events");
            Console.WriteLine($"Irrigation History: {crop.Soil.IrrigationHistory.Count} events");
            Console.WriteLine($"Field Location: {crop.Location.Name} ({crop.Location.Latitude}, {crop.Location.Longitude})");
            return result;
        }

        public void ApplyEvent(Crop crop, PredictionResult result)
        {
            if (crop == null || crop.Soil == null || result == null)
            {
                Console.WriteLine("Error: Invalid input for applying events.");
                return;
            }

            if (result.IrrigationAmount > 0 && result.NextIrrigationDate == DateTime.Today)
            {
                crop.Soil.ApplyIrrigation(new IrrigationEvent { Date = DateTime.Today, Amount = result.IrrigationAmount });
                Console.WriteLine($"Applied irrigation: {result.IrrigationAmount:F2} mm/ha");
            }

            if (result.NitrogenFertilizerAmount > 0 || result.PhosphorusFertilizerAmount > 0 || result.PotassiumFertilizerAmount > 0)
            {
                if (result.NextFertilizationDate == DateTime.Today)
                {
                    crop.Soil.ApplyFertilization(new FertilizationEvent
                    {
                        Date = DateTime.Today,
                        NitrogenAmount = result.NitrogenFertilizerAmount * crop.Plant.FertilizerNitrogenPercentage / PERCENTAGE_CONVERSION_FACTOR * NITROGEN_EFFICIENCY,
                        PhosphorusAmount = result.PhosphorusFertilizerAmount * crop.Plant.FertilizerPhosphorusPercentage / PERCENTAGE_CONVERSION_FACTOR * PHOSPHORUS_EFFICIENCY,
                        PotassiumAmount = result.PotassiumFertilizerAmount * crop.Plant.FertilizerPotassiumPercentage / PERCENTAGE_CONVERSION_FACTOR * POTASSIUM_EFFICIENCY,
                    });
                    Console.WriteLine($"Applied fertilization: N={result.NitrogenFertilizerAmount:F2}, P={result.PhosphorusFertilizerAmount:F2}, K={result.PotassiumFertilizerAmount:F2} kg/ha");
                }
            }
        }
    }
}