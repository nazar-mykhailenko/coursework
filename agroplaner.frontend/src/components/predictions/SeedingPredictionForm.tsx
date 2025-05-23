import { useState, type FormEvent, useEffect } from "react";
import { MapContainer, TileLayer, Marker, useMapEvents } from "react-leaflet";
import { LatLng } from "leaflet";
import "leaflet/dist/leaflet.css";
import "./leaflet-fix";
import { useAppDispatch } from "../../store/hooks";
import { predictSeedingDate } from "../../store/slices/predictionsSlice";
import type {
  Plant,
  SeedingPredictionResponse,
} from "../../services/predictionService";

interface SeedingPredictionFormProps {
  plants: Plant[];
  plantsLoading: boolean;
  isSubmitting: boolean;
  error: string | null;
  predictionResult: SeedingPredictionResponse | null;
}

// Map marker component that allows clicking to set a marker position
const LocationMarker = ({
  position,
  setPosition,
}: {
  position: [number, number];
  setPosition: (position: [number, number]) => void;
}) => {
  const map = useMapEvents({
    click(e) {
      setPosition([e.latlng.lat, e.latlng.lng]);
    },
  });

  // Center map on the marker position when it changes
  useEffect(() => {
    if (position[0] !== 0 && position[1] !== 0) {
      map.flyTo(new LatLng(position[0], position[1]), map.getZoom());
    }
  }, [map, position]);

  return position[0] !== 0 && position[1] !== 0 ? (
    <Marker position={position} />
  ) : null;
};

const SeedingPredictionForm = ({
  plants,
  plantsLoading,
  isSubmitting,
  error,
  predictionResult,
}: SeedingPredictionFormProps) => {
  const dispatch = useAppDispatch();

  // Form state
  const [plantId, setPlantId] = useState<number>(0);
  const [position, setPosition] = useState<[number, number]>([0, 0]);
  const [expectedYield, setExpectedYield] = useState<number>(0);
  const [fieldArea, setFieldArea] = useState<number>(0);

  // Soil data state
  const [soilTemperature, setSoilTemperature] = useState<number>(15);
  const [soilFieldCapacity, setSoilFieldCapacity] = useState<number>(0.3);
  const [soilCurrentMoisture, setSoilCurrentMoisture] = useState<number>(0.2);
  const [soilNitrogen, setSoilNitrogen] = useState<number>(50);
  const [soilPhosphorus, setSoilPhosphorus] = useState<number>(20);
  const [soilPotassium, setSoilPotassium] = useState<number>(30);

  // Validation state
  const [formErrors, setFormErrors] = useState<{ [key: string]: string }>({});

  const validateForm = () => {
    const errors: { [key: string]: string } = {};

    if (plantId === 0) {
      errors.plantId = "Please select a plant";
    }

    if (position[0] === 0 && position[1] === 0) {
      errors.position = "Please select a location on the map";
    }

    if (expectedYield <= 0) {
      errors.expectedYield = "Expected yield must be greater than 0";
    }

    if (fieldArea <= 0) {
      errors.fieldArea = "Field area must be greater than 0";
    }

    // Validate soil data
    if (soilTemperature < 0) {
      errors.soilTemperature = "Soil temperature must be at least 0";
    }

    if (soilFieldCapacity <= 0 || soilFieldCapacity > 1) {
      errors.soilFieldCapacity = "Field capacity must be between 0 and 1";
    }

    if (soilCurrentMoisture < 0 || soilCurrentMoisture > 1) {
      errors.soilCurrentMoisture = "Current moisture must be between 0 and 1";
    }

    if (soilNitrogen < 0) {
      errors.soilNitrogen = "Available nitrogen must be at least 0";
    }

    if (soilPhosphorus < 0) {
      errors.soilPhosphorus = "Available phosphorus must be at least 0";
    }

    if (soilPotassium < 0) {
      errors.soilPotassium = "Available potassium must be at least 0";
    }

    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    dispatch(
      predictSeedingDate({
        plantId,
        latitude: position[0],
        longitude: position[1],
        expectedYield,
        fieldArea,
        soil: {
          temperature: soilTemperature,
          fieldCapacity: soilFieldCapacity,
          currentMoisture: soilCurrentMoisture,
          availableNitrogen: soilNitrogen,
          availablePhosphorus: soilPhosphorus,
          availablePotassium: soilPotassium,
        },
      })
    );
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return "Not available";
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="seeding-prediction-form">
      {error && <div className="error-message">{error}</div>}

      {predictionResult && (
        <div className="prediction-result">
          <h3>Recommendation</h3>
          <p>
            <strong>Recommended Seeding Date:</strong>{" "}
            {formatDate(predictionResult.recommendedSeedingDate)}
          </p>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="form-section">
          <h3>Crop Information</h3>

          <div className="form-group">
            <label htmlFor="plantId">Select Plant</label>
            <select
              id="plantId"
              value={plantId}
              onChange={(e) => setPlantId(Number(e.target.value))}
              className={formErrors.plantId ? "error" : ""}
              disabled={plantsLoading || isSubmitting}
            >
              <option value={0}>Select a plant...</option>
              {plants.map((plant) => (
                <option key={plant.plantId} value={plant.plantId}>
                  {plant.name}
                </option>
              ))}
            </select>
            {formErrors.plantId && (
              <div className="field-error">{formErrors.plantId}</div>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="expectedYield">Expected Yield (tons/hectare)</label>
            <input
              type="number"
              id="expectedYield"
              value={expectedYield}
              onChange={(e) => setExpectedYield(Number(e.target.value))}
              className={formErrors.expectedYield ? "error" : ""}
              disabled={isSubmitting}
              step="0.1"
              min="0"
            />
            {formErrors.expectedYield && (
              <div className="field-error">{formErrors.expectedYield}</div>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="fieldArea">Field Area (hectares)</label>
            <input
              type="number"
              id="fieldArea"
              value={fieldArea}
              onChange={(e) => setFieldArea(Number(e.target.value))}
              className={formErrors.fieldArea ? "error" : ""}
              disabled={isSubmitting}
              step="0.1"
              min="0"
            />
            {formErrors.fieldArea && (
              <div className="field-error">{formErrors.fieldArea}</div>
            )}
          </div>
        </div>

        <div className="form-section">
          <h3>Location</h3>
          <p>Click on the map to select your field location</p>

          <div className="coordinates-display">
            {position[0] !== 0 && position[1] !== 0 ? (
              <p>
                Selected coordinates: {position[0].toFixed(6)},{" "}
                {position[1].toFixed(6)}
              </p>
            ) : (
              <p>No location selected</p>
            )}
            {formErrors.position && (
              <div className="field-error">{formErrors.position}</div>
            )}
          </div>

          <MapContainer
            center={[51.505, -0.09]}
            zoom={3}
            className="map-container"
            style={{ height: "400px" }}
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <LocationMarker position={position} setPosition={setPosition} />
          </MapContainer>
        </div>

        <div className="form-section">
          <h3>Soil Data</h3>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="soilTemperature">Soil Temperature (°C)</label>
              <input
                type="number"
                id="soilTemperature"
                value={soilTemperature}
                onChange={(e) => setSoilTemperature(Number(e.target.value))}
                className={formErrors.soilTemperature ? "error" : ""}
                disabled={isSubmitting}
                step="0.1"
              />
              {formErrors.soilTemperature && (
                <div className="field-error">{formErrors.soilTemperature}</div>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="soilFieldCapacity">
                Field Capacity (fraction)
              </label>
              <input
                type="number"
                id="soilFieldCapacity"
                value={soilFieldCapacity}
                onChange={(e) => setSoilFieldCapacity(Number(e.target.value))}
                className={formErrors.soilFieldCapacity ? "error" : ""}
                disabled={isSubmitting}
                step="0.01"
                min="0"
                max="1"
              />
              {formErrors.soilFieldCapacity && (
                <div className="field-error">
                  {formErrors.soilFieldCapacity}
                </div>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="soilCurrentMoisture">
                Current Moisture (fraction)
              </label>
              <input
                type="number"
                id="soilCurrentMoisture"
                value={soilCurrentMoisture}
                onChange={(e) => setSoilCurrentMoisture(Number(e.target.value))}
                className={formErrors.soilCurrentMoisture ? "error" : ""}
                disabled={isSubmitting}
                step="0.01"
                min="0"
                max="1"
              />
              {formErrors.soilCurrentMoisture && (
                <div className="field-error">
                  {formErrors.soilCurrentMoisture}
                </div>
              )}
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="soilNitrogen">Available Nitrogen (kg/ha)</label>
              <input
                type="number"
                id="soilNitrogen"
                value={soilNitrogen}
                onChange={(e) => setSoilNitrogen(Number(e.target.value))}
                className={formErrors.soilNitrogen ? "error" : ""}
                disabled={isSubmitting}
                step="1"
                min="0"
              />
              {formErrors.soilNitrogen && (
                <div className="field-error">{formErrors.soilNitrogen}</div>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="soilPhosphorus">
                Available Phosphorus (kg/ha)
              </label>
              <input
                type="number"
                id="soilPhosphorus"
                value={soilPhosphorus}
                onChange={(e) => setSoilPhosphorus(Number(e.target.value))}
                className={formErrors.soilPhosphorus ? "error" : ""}
                disabled={isSubmitting}
                step="1"
                min="0"
              />
              {formErrors.soilPhosphorus && (
                <div className="field-error">{formErrors.soilPhosphorus}</div>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="soilPotassium">Available Potassium (kg/ha)</label>
              <input
                type="number"
                id="soilPotassium"
                value={soilPotassium}
                onChange={(e) => setSoilPotassium(Number(e.target.value))}
                className={formErrors.soilPotassium ? "error" : ""}
                disabled={isSubmitting}
                step="1"
                min="0"
              />
              {formErrors.soilPotassium && (
                <div className="field-error">{formErrors.soilPotassium}</div>
              )}
            </div>
          </div>
        </div>

        <div className="form-actions">
          <button
            type="submit"
            className="btn btn-primary"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Calculating..." : "Calculate Seeding Date"}
          </button>
        </div>
      </form>
    </div>
  );
};

export default SeedingPredictionForm;
