using AgroPlaner.DAL.Models;

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

        public void UpdateCumulativeGDD(
            Crop crop,
            DateTime plantingDate,
            List<WeatherData> weatherHistory
        )
        {
            double cumulativeGDD = 0;
            foreach (
                var weather in weatherHistory.Where(w =>
                    w.Date >= plantingDate && w.Date <= DateTime.Today
                )
            )
            {
                double dailyGDD = Math.Max(
                    (weather.MaxTemp + weather.MinTemp) / 2 - crop.Plant.BaseTempForGDD,
                    0
                );
                cumulativeGDD += dailyGDD;
            }
            crop.CumulativeGDDToday = cumulativeGDD;
        }

        public DateTime? PredictSeedingDate(
            Crop crop,
            List<WeatherData> weatherForecast,
            SoilData soil
        )
        {
            double currentSoilTemp = soil.Temperature;
            foreach (var weather in weatherForecast.OrderBy(w => w.Date))
            {
                double avgAirTemp = (weather.MaxTemp + weather.MinTemp) / 2;
                currentSoilTemp =
                    SOIL_TEMP_MEMORY_FACTOR * currentSoilTemp
                    + AIR_TEMP_INFLUENCE_FACTOR * avgAirTemp;
                if (
                    currentSoilTemp >= crop.Plant.MinSoilTempForSeeding
                    && weather.Precipitation < MAX_PRECIPITATION_THRESHOLD
                )
                {
                    return weather.Date;
                }
            }
            return null;
        }

        private double CalculateETo(WeatherData weather)
        {
            double T = (weather.MaxTemp + weather.MinTemp) / 2;
            double delta = DELTA_COEFFICIENT * Math.Sin(DELTA_ANGLE_FACTOR * T + DELTA_OFFSET);
            double Rn = weather.SolarRadiation;
            double G = SOLAR_RADIATION_GROUND_FLUX;
            double u2 = weather.WindSpeed;
            double es =
                VAPOR_PRESSURE_COEFFICIENT
                * Math.Exp(VAPOR_PRESSURE_EXPONENT_FACTOR * T / (T + VAPOR_PRESSURE_TEMP_OFFSET));
            double ea = es * (weather.RelativeHumidity / PERCENTAGE_CONVERSION_FACTOR);

            double numerator =
                delta * (Rn - G)
                + PSYCHROMETRIC_CONSTANT
                    * (ETO_WIND_FACTOR / (T + ETO_TEMP_OFFSET))
                    * u2
                    * (es - ea);
            double denominator = delta + PSYCHROMETRIC_CONSTANT * (1 + ETO_SLOPE_FACTOR * u2);
            return numerator / denominator;
        }

        public (double Amount, DateTime? NextDate) CalculateIrrigation(
            Crop crop,
            List<WeatherData> weatherHistory,
            WeatherData todayWeather,
            SoilData soil,
            int growthStage
        )
        {
            double ETo = CalculateETo(todayWeather);
            double Kc = crop.Plant.KcValues[growthStage % crop.Plant.KcValues.Length];
            double ETc = Kc * ETo;

            double TAW =
                (soil.FieldCapacity - crop.Plant.WiltingPoint)
                * crop.Plant.RootDepth
                * TAW_CONVERSION_FACTOR;
            double AD = crop.Plant.AllowableDepletionFraction * TAW;

            double moisture = soil.CurrentMoisture;
            foreach (
                var weather in weatherHistory.Where(w =>
                    w.Date > soil.IrrigationHistory.LastOrDefault()?.Date
                    && w.Date <= DateTime.Today
                )
            )
            {
                moisture += weather.Precipitation - CalculateETo(weather) * Kc;
                moisture = Math.Max(
                    Math.Min(moisture, soil.FieldCapacity),
                    crop.Plant.WiltingPoint
                );
            }

            double waterDeficit =
                soil.FieldCapacity * SOIL_MOISTURE_TARGET_FRACTION
                - (moisture + todayWeather.Precipitation - ETc);
            if (waterDeficit > 0)
            {
                double irrigationAmount = waterDeficit / IRRIGATION_EFFICIENCY; // mm/ha
                return (irrigationAmount, DateTime.Today);
            }

            double daysUntilNext = (moisture - (soil.FieldCapacity - AD)) / ETc;
            return (0, DateTime.Today.AddDays((int)daysUntilNext));
        }

        public string GetCurrentGrowthStage(Crop crop)
        {
            double cumulativeGDD = crop.CumulativeGDDToday;
            foreach (var stage in crop.Plant.PlantGrowthStages)
            {
                if (cumulativeGDD >= stage.MinGDD && cumulativeGDD <= stage.MaxGDD)
                {
                    return stage.StageName;
                }
            }
            return cumulativeGDD > crop.Plant.MaturityGDD ? "Mature" : "Not yet emerged";
        }

        public int GetGrowthStageIndex(string stageName, Crop crop)
        {
            var stages = crop.Plant.PlantGrowthStages.OrderBy(s => s.OrderIndex).ToList();
            return stages.FindIndex(s => s.StageName == stageName);
        }

        private (double N, double P, double K) EstimateDailyNutrientUptake(
            Crop crop,
            string currentStage,
            double totalNRequired,
            double totalPRequired,
            double totalKRequired
        )
        {
            int stageIndex = GetGrowthStageIndex(currentStage, crop);
            if (stageIndex == -1)
                return (
                    totalNRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD,
                    totalPRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD,
                    totalKRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD
                );

            double nFraction = crop.Plant.NitrogenUptakeDistribution[stageIndex];
            double pFraction = crop.Plant.PhosphorusUptakeDistribution[stageIndex];
            double kFraction = crop.Plant.PotassiumUptakeDistribution[stageIndex];

            var stageRange = crop.Plant.PlantGrowthStages.FirstOrDefault(s =>
                s.StageName == currentStage
            );
            if (stageRange == null)
                return (
                    totalNRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD,
                    totalPRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD,
                    totalKRequired * DEFAULT_DAILY_UPTAKE_FRACTION / crop.Plant.MaturityGDD
                );

            double stageGDD = stageRange.MaxGDD - stageRange.MinGDD;

            return (
                totalNRequired * nFraction / stageGDD,
                totalPRequired * pFraction / stageGDD,
                totalKRequired * kFraction / stageGDD
            );
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
            double totalNRequired = crop.ExpectedYield * crop.Plant.NitrogenContent; // kg/ha
            double totalPRequired = crop.ExpectedYield * crop.Plant.PhosphorusContent; // kg/ha
            double totalKRequired = crop.ExpectedYield * crop.Plant.PotassiumContent; // kg/ha

            double currentN = soil.AvailableNitrogen;
            double currentP = soil.AvailablePhosphorus;
            double currentK = soil.AvailablePotassium;

            string currentStage = GetCurrentGrowthStage(crop);
            var (dailyN, dailyP, dailyK) = EstimateDailyNutrientUptake(
                crop,
                currentStage,
                totalNRequired,
                totalPRequired,
                totalKRequired
            );

            DateTime lastFertDate = soil.FertilizationHistory.LastOrDefault()?.Date ?? plantingDate;
            double daysSinceLastFert = (DateTime.Today - lastFertDate).Days;
            currentN -= dailyN * daysSinceLastFert;
            currentP -= dailyP * daysSinceLastFert;
            currentK -= dailyK * daysSinceLastFert;

            double remainingN = totalNRequired - Math.Max(currentN, 0);
            double remainingP = totalPRequired - Math.Max(currentP, 0);
            double remainingK = totalKRequired - Math.Max(currentK, 0);

            if (remainingN <= 0 && remainingP <= 0 && remainingK <= 0)
                return (0, 0, 0, null);

            double daysNDepletion =
                currentN > 0 ? (currentN - totalNRequired * MIN_NUTRIENT_THRESHOLD) / dailyN : 0;
            double daysPDepletion =
                currentP > 0 ? (currentP - totalPRequired * MIN_NUTRIENT_THRESHOLD) / dailyP : 0;
            double daysKDepletion =
                currentK > 0 ? (currentK - totalKRequired * MIN_NUTRIENT_THRESHOLD) / dailyK : 0;

            double minDays = Math.Min(Math.Min(daysNDepletion, daysPDepletion), daysKDepletion);
            DateTime depletionDate = DateTime.Today.AddDays((int)minDays);

            foreach (
                var weather in weatherForecast
                    .Where(w => w.Date >= depletionDate)
                    .OrderBy(w => w.Date)
            )
            {
                if (weather.Precipitation < MAX_PRECIPITATION_THRESHOLD)
                {
                    double nAmount =
                        remainingN > 0
                            ? remainingN
                                * PERCENTAGE_CONVERSION_FACTOR
                                / crop.Plant.FertilizerNitrogenPercentage
                            : 0;
                    double pAmount =
                        remainingP > 0
                            ? remainingP
                                * PERCENTAGE_CONVERSION_FACTOR
                                / crop.Plant.FertilizerPhosphorusPercentage
                            : 0;
                    double kAmount =
                        remainingK > 0
                            ? remainingK
                                * PERCENTAGE_CONVERSION_FACTOR
                                / crop.Plant.FertilizerPotassiumPercentage
                            : 0;
                    return (nAmount, pAmount, kAmount, weather.Date);
                }
            }

            double fallbackNAmount =
                remainingN > 0
                    ? remainingN
                        * PERCENTAGE_CONVERSION_FACTOR
                        / crop.Plant.FertilizerNitrogenPercentage
                    : 0;
            double fallbackPAmount =
                remainingP > 0
                    ? remainingP
                        * PERCENTAGE_CONVERSION_FACTOR
                        / crop.Plant.FertilizerPhosphorusPercentage
                    : 0;
            double fallbackKAmount =
                remainingK > 0
                    ? remainingK
                        * PERCENTAGE_CONVERSION_FACTOR
                        / crop.Plant.FertilizerPotassiumPercentage
                    : 0;
            return (fallbackNAmount, fallbackPAmount, fallbackKAmount, depletionDate);
        }

        public DateTime? PredictHarvestDate(Crop crop, List<WeatherData> weatherForecast)
        {
            double cumulativeGDD = crop.CumulativeGDDToday;
            foreach (var weather in weatherForecast.Where(w => w.Date > DateTime.Today))
            {
                double dailyGDD = Math.Max(
                    (weather.MaxTemp + weather.MinTemp) / 2 - crop.Plant.BaseTempForGDD,
                    0
                );
                cumulativeGDD += dailyGDD;
                if (cumulativeGDD >= crop.Plant.MaturityGDD)
                {
                    return weather.Date;
                }
            }
            return null;
        }

        public PredictionResult Predict(
            Crop crop,
            List<WeatherData> weatherHistory,
            List<WeatherData> weatherForecast,
            DateTime plantingDate
        )
        {
            UpdateCumulativeGDD(crop, plantingDate, weatherHistory);
            var result = new PredictionResult();
            string currentStage = GetCurrentGrowthStage(crop);
            int growthStageIndex = GetGrowthStageIndex(currentStage, crop);

            result.RecommendedSeedingDate = PredictSeedingDate(crop, weatherForecast, crop.Soil);
            var (irrigationAmount, nextIrrigationDate) = CalculateIrrigation(
                crop,
                weatherHistory,
                weatherForecast.First(),
                crop.Soil,
                growthStageIndex
            );
            result.IrrigationAmount = irrigationAmount;
            result.NextIrrigationDate = nextIrrigationDate;

            var (nAmount, pAmount, kAmount, nextFertilizationDate) = CalculateFertilization(
                crop,
                crop.Soil,
                plantingDate,
                weatherHistory,
                weatherForecast
            );
            result.NitrogenFertilizerAmount = nAmount;
            result.PhosphorusFertilizerAmount = pAmount;
            result.PotassiumFertilizerAmount = kAmount;
            result.NextFertilizationDate = nextFertilizationDate;

            result.PredictedHarvestDate = PredictHarvestDate(crop, weatherForecast);

            Console.WriteLine($"Current Growth Stage: {currentStage}");
            Console.WriteLine($"Cumulative GDD Today: {crop.CumulativeGDDToday:F2}");
            Console.WriteLine(
                $"Fertilization History: {crop.Soil.FertilizationHistory.Count} events"
            );
            Console.WriteLine($"Irrigation History: {crop.Soil.IrrigationHistory.Count} events");
            Console.WriteLine(
                $"Field Location: {crop.Location.Name} ({crop.Location.Latitude}, {crop.Location.Longitude})"
            );
            return result;
        }

        public void ApplyEvent(Crop crop, PredictionResult result)
        {
            if (result.IrrigationAmount > 0 && result.NextIrrigationDate == DateTime.Today)
            {
                crop.Soil.ApplyIrrigation(
                    new IrrigationEvent { Date = DateTime.Today, Amount = result.IrrigationAmount }
                );
            }
            if (
                result.NitrogenFertilizerAmount > 0
                || result.PhosphorusFertilizerAmount > 0
                || result.PotassiumFertilizerAmount > 0
            )
            {
                if (result.NextFertilizationDate == DateTime.Today)
                {
                    crop.Soil.ApplyFertilization(
                        new FertilizationEvent
                        {
                            Date = DateTime.Today,
                            NitrogenAmount =
                                result.NitrogenFertilizerAmount
                                * crop.Plant.FertilizerNitrogenPercentage
                                / PERCENTAGE_CONVERSION_FACTOR,
                            PhosphorusAmount =
                                result.PhosphorusFertilizerAmount
                                * crop.Plant.FertilizerPhosphorusPercentage
                                / PERCENTAGE_CONVERSION_FACTOR,
                            PotassiumAmount =
                                result.PotassiumFertilizerAmount
                                * crop.Plant.FertilizerPotassiumPercentage
                                / PERCENTAGE_CONVERSION_FACTOR,
                        }
                    );
                }
            }
        }
    }
}
