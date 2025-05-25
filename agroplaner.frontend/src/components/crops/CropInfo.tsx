import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, useParams, Link } from "react-router-dom";
import { type RootState, type AppDispatch } from "../../store";
import { fetchCrop } from "../../store/slices/cropsSlice";
import { plantService, type Plant } from "../../services/plantService";
import { locationService, type Location } from "../../services/locationService";
import { soilDataService, type SoilData } from "../../services/soilDataService";
import {
  cropService,
  type FullPredictionResponse,
} from "../../services/cropService";
import "./CropInfo.css";

const CropInfo: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { currentCrop, loading, error } = useSelector(
    (state: RootState) => state.crops
  );
  const [plant, setPlant] = useState<Plant | null>(null);
  const [location, setLocation] = useState<Location | null>(null);
  const [soilData, setSoilData] = useState<SoilData | null>(null);
  const [fullPrediction, setFullPrediction] =
    useState<FullPredictionResponse | null>(null);
  const [loadingData, setLoadingData] = useState(true);
  const [dataError, setDataError] = useState<string | null>(null);

  useEffect(() => {
    if (id) {
      dispatch(fetchCrop(Number(id)));
    }
  }, [dispatch, id]);

  useEffect(() => {
    const loadData = async () => {
      if (!currentCrop) return;

      try {
        setLoadingData(true);

        const promises = [];

        // Load plant data
        promises.push(
          plantService
            .getPlant(currentCrop.plantId)
            .then(setPlant)
            .catch(console.error)
        );

        // Load location data
        promises.push(
          locationService
            .getLocation(currentCrop.locationId)
            .then(setLocation)
            .catch(console.error)
        );

        // Load soil data
        promises.push(
          soilDataService
            .getSoilDataByCrop(currentCrop.id)
            .then(setSoilData)
            .catch(() => {
              console.warn("No soil data found for crop");
              setSoilData(null);
            })
        ); // Load full prediction data
        promises.push(
          cropService
            .getFullPrediction(currentCrop.id)
            .then(setFullPrediction)
            .catch(() => {
              console.warn("No full prediction data available");
              setFullPrediction(null);
            })
        );

        await Promise.allSettled(promises);
      } catch (error) {
        console.error("Failed to load crop details:", error);
        setDataError("Failed to load crop details");
      } finally {
        setLoadingData(false);
      }
    };

    if (currentCrop) {
      loadData();
    }
  }, [currentCrop]);

  const formatDate = (dateString?: string): string => {
    if (!dateString) return "Not available";
    return new Date(dateString).toLocaleDateString();
  };

  const getIrrigationStatus = (): {
    message: string;
    type: "success" | "warning" | "info";
  } => {
    if (!fullPrediction?.nextIrrigationDate) {
      return { message: "No irrigation data available", type: "info" };
    }

    const nextDate = new Date(fullPrediction.nextIrrigationDate);
    const today = new Date();
    const daysUntil = Math.ceil(
      (nextDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)
    );

    if (daysUntil <= 0) {
      return { message: "Irrigation needed now!", type: "warning" };
    } else if (daysUntil <= 3) {
      return {
        message: `Irrigation needed in ${daysUntil} day(s)`,
        type: "warning",
      };
    } else {
      return {
        message: `Next irrigation in ${daysUntil} day(s)`,
        type: "success",
      };
    }
  };

  const getFertilizationStatus = (): {
    message: string;
    type: "success" | "warning" | "info";
  } => {
    if (!fullPrediction?.nextFertilizationDate) {
      return { message: "No fertilization data available", type: "info" };
    }

    const nextDate = new Date(fullPrediction.nextFertilizationDate);
    const today = new Date();
    const daysUntil = Math.ceil(
      (nextDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)
    );

    if (daysUntil <= 0) {
      return { message: "Fertilization needed now!", type: "warning" };
    } else if (daysUntil <= 7) {
      return {
        message: `Fertilization needed in ${daysUntil} day(s)`,
        type: "warning",
      };
    } else {
      return {
        message: `Next fertilization in ${daysUntil} day(s)`,
        type: "success",
      };
    }
  };

  const getHarvestStatus = (): {
    message: string;
    type: "success" | "warning" | "info";
  } => {
    if (!fullPrediction?.predictedHarvestDate) {
      return { message: "No harvest prediction available", type: "info" };
    }

    const harvestDate = new Date(fullPrediction.predictedHarvestDate);
    const today = new Date();
    const daysUntil = Math.ceil(
      (harvestDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24)
    );

    if (daysUntil <= 0) {
      return { message: "Ready for harvest!", type: "warning" };
    } else if (daysUntil <= 14) {
      return { message: `Harvest in ${daysUntil} day(s)`, type: "warning" };
    } else {
      return { message: `Harvest in ${daysUntil} day(s)`, type: "success" };
    }
  };

  if (loading || loadingData) {
    return <div className="crop-info-loading">Loading crop information...</div>;
  }

  if (error || dataError) {
    return <div className="crop-info-error">Error: {error || dataError}</div>;
  }

  if (!currentCrop) {
    return <div className="crop-info-error">Crop not found</div>;
  }

  const irrigationStatus = getIrrigationStatus();
  const fertilizationStatus = getFertilizationStatus();
  const harvestStatus = getHarvestStatus();

  return (
    <div className="crop-info-container">
      <div className="crop-info-header">
        <h1>{currentCrop.name}</h1>
        <div className="header-actions">
          <Link
            to={`/crops/${currentCrop.id}/edit`}
            className="btn btn-primary"
          >
            Edit Crop
          </Link>
          <button
            type="button"
            onClick={() => navigate("/crops")}
            className="btn btn-secondary"
          >
            Back to Crops
          </button>
        </div>
      </div>

      {/* Status Cards */}
      <div className="status-cards">
        <div className={`status-card ${irrigationStatus.type}`}>
          <div className="status-icon">💧</div>
          <div className="status-content">
            <h3>Irrigation</h3>
            <p>{irrigationStatus.message}</p>
            {fullPrediction?.irrigationAmount && (
              <small>Amount: {fullPrediction.irrigationAmount} mm</small>
            )}
          </div>
        </div>

        <div className={`status-card ${fertilizationStatus.type}`}>
          <div className="status-icon">🌱</div>
          <div className="status-content">
            <h3>Fertilization</h3>
            <p>{fertilizationStatus.message}</p>
            {fullPrediction?.nitrogenFertilizerAmount && (
              <small>N: {fullPrediction.nitrogenFertilizerAmount} kg/ha</small>
            )}
          </div>
        </div>

        <div className={`status-card ${harvestStatus.type}`}>
          <div className="status-icon">🌾</div>
          <div className="status-content">
            <h3>Harvest</h3>
            <p>{harvestStatus.message}</p>
            {fullPrediction?.predictedHarvestDate && (
              <small>
                Date: {formatDate(fullPrediction.predictedHarvestDate)}
              </small>
            )}
          </div>
        </div>
      </div>

      <div className="info-sections">
        {/* Basic Crop Information */}
        <div className="info-section">
          <h2>Crop Information</h2>
          <div className="info-grid">
            <div className="info-item">
              <span className="label">Crop ID:</span>
              <span className="value">{currentCrop.id}</span>
            </div>
            <div className="info-item">
              <span className="label">Plant:</span>
              <span className="value">
                {plant?.name || `ID: ${currentCrop.plantId}`}
              </span>
            </div>
            <div className="info-item">
              <span className="label">Location:</span>
              <span className="value">
                {location
                  ? `${location.name} (${location.latitude}, ${location.longitude})`
                  : `ID: ${currentCrop.locationId}`}
              </span>
            </div>
            <div className="info-item">
              <span className="label">Expected Yield:</span>
              <span className="value">{currentCrop.expectedYield} kg</span>
            </div>
            <div className="info-item">
              <span className="label">Field Area:</span>
              <span className="value">{currentCrop.fieldArea} ha</span>
            </div>
            <div className="info-item">
              <span className="label">Current GDD:</span>
              <span className="value">{currentCrop.cumulativeGDDToday}°C</span>
            </div>
            {currentCrop.plantingDate && (
              <div className="info-item">
                <span className="label">Planting Date:</span>
                <span className="value">
                  {formatDate(currentCrop.plantingDate)}
                </span>
              </div>
            )}
          </div>
        </div>{" "}
        {/* Plant Details */}
        {plant && (
          <div className="info-section">
            <h2>Plant Details</h2>
            <div className="info-grid">
              <div className="info-item">
                <span className="label">Base Temperature:</span>
                <span className="value">{plant.baseTempForGDD}°C</span>
              </div>
              <div className="info-item">
                <span className="label">Maturity GDD:</span>
                <span className="value">{plant.maturityGDD}°C</span>
              </div>
              <div className="info-item">
                <span className="label">Root Depth:</span>
                <span className="value">{plant.rootDepth} cm</span>
              </div>
              <div className="info-item">
                <span className="label">Min Soil Temp:</span>
                <span className="value">{plant.minSoilTempForSeeding}°C</span>
              </div>
              <div className="info-item">
                <span className="label">Nitrogen Content:</span>
                <span className="value">{plant.nitrogenContent}%</span>
              </div>
              <div className="info-item">
                <span className="label">Phosphorus Content:</span>
                <span className="value">{plant.phosphorusContent}%</span>
              </div>
              <div className="info-item">
                <span className="label">Potassium Content:</span>
                <span className="value">{plant.potassiumContent}%</span>
              </div>
            </div>
          </div>
        )}
        {/* Soil Data */}
        {soilData && (
          <div className="info-section">
            <h2>Soil Conditions</h2>
            <div className="info-grid">
              <div className="info-item">
                <span className="label">Current Moisture:</span>
                <span className="value">{soilData.currentMoisture}%</span>
              </div>
              <div className="info-item">
                <span className="label">Field Capacity:</span>
                <span className="value">{soilData.fieldCapacity}%</span>
              </div>
              <div className="info-item">
                <span className="label">Temperature:</span>
                <span className="value">{soilData.temperature}°C</span>
              </div>
              <div className="info-item">
                <span className="label">Available Nitrogen:</span>
                <span className="value">
                  {soilData.availableNitrogen} mg/kg
                </span>
              </div>
              <div className="info-item">
                <span className="label">Available Phosphorus:</span>
                <span className="value">
                  {soilData.availablePhosphorus} mg/kg
                </span>
              </div>
              <div className="info-item">
                <span className="label">Available Potassium:</span>
                <span className="value">
                  {soilData.availablePotassium} mg/kg
                </span>
              </div>
            </div>
          </div>
        )}
        {/* Prediction Data */}
        {fullPrediction && (
          <div className="info-section">
            <h2>Prediction Details</h2>
            <div className="prediction-details">
              <div className="prediction-group">
                <h3>Irrigation Recommendations</h3>
                <div className="info-grid">
                  <div className="info-item">
                    <span className="label">Recommended Amount:</span>
                    <span className="value">
                      {fullPrediction.irrigationAmount} mm
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="label">Next Irrigation:</span>
                    <span className="value">
                      {formatDate(fullPrediction.nextIrrigationDate)}
                    </span>
                  </div>
                </div>
              </div>

              <div className="prediction-group">
                <h3>Fertilization Recommendations</h3>
                <div className="info-grid">
                  <div className="info-item">
                    <span className="label">Nitrogen:</span>
                    <span className="value">
                      {fullPrediction.nitrogenFertilizerAmount} kg/ha
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="label">Phosphorus:</span>
                    <span className="value">
                      {fullPrediction.phosphorusFertilizerAmount} kg/ha
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="label">Potassium:</span>
                    <span className="value">
                      {fullPrediction.potassiumFertilizerAmount} kg/ha
                    </span>
                  </div>
                  <div className="info-item">
                    <span className="label">Next Fertilization:</span>
                    <span className="value">
                      {formatDate(fullPrediction.nextFertilizationDate)}
                    </span>
                  </div>
                </div>
              </div>

              <div className="prediction-group">
                <h3>Harvest Information</h3>
                <div className="info-grid">
                  <div className="info-item">
                    <span className="label">Predicted Harvest Date:</span>
                    <span className="value">
                      {formatDate(fullPrediction.predictedHarvestDate)}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default CropInfo;
