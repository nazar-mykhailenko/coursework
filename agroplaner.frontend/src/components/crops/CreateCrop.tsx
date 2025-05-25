import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import { type RootState, type AppDispatch } from "../../store";
import { createCrop } from "../../store/slices/cropsSlice";
import { plantService, type Plant } from "../../services/plantService";
import { locationService, type Location } from "../../services/locationService";
import { soilDataService } from "../../services/soilDataService";
import "./CreateCrop.css";

const CreateCrop: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const { loading, error } = useSelector((state: RootState) => state.crops);

  const [plants, setPlants] = useState<Plant[]>([]);
  const [locations, setLocations] = useState<Location[]>([]);
  const [loadingData, setLoadingData] = useState(true);
  const [dataError, setDataError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    name: "",
    plantId: "",
    locationId: "",
    expectedYield: "",
    fieldArea: "",
    // Soil data
    currentMoisture: "",
    fieldCapacity: "",
    temperature: "",
    availableNitrogen: "",
    availablePhosphorus: "",
    availablePotassium: "",
  });

  const [formErrors, setFormErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoadingData(true);
        const [plantsData, locationsData] = await Promise.all([
          plantService.getPlants(),
          locationService.getLocations(),
        ]);
        setPlants(plantsData);
        setLocations(locationsData);
      } catch (error) {
        console.error("Failed to load data:", error);
        setDataError("Failed to load plants and locations data");
      } finally {
        setLoadingData(false);
      }
    };

    loadData();
  }, []);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.name.trim()) errors.name = "Crop name is required";
    if (!formData.plantId) errors.plantId = "Please select a plant";
    if (!formData.locationId) errors.locationId = "Please select a location";
    if (!formData.expectedYield || Number(formData.expectedYield) <= 0) {
      errors.expectedYield = "Expected yield must be greater than 0";
    }
    if (!formData.fieldArea || Number(formData.fieldArea) <= 0) {
      errors.fieldArea = "Field area must be greater than 0";
    }

    // Soil data validation
    if (
      !formData.currentMoisture ||
      Number(formData.currentMoisture) < 0 ||
      Number(formData.currentMoisture) > 100
    ) {
      errors.currentMoisture = "Current moisture must be between 0 and 100";
    }
    if (!formData.fieldCapacity || Number(formData.fieldCapacity) <= 0) {
      errors.fieldCapacity = "Field capacity must be greater than 0";
    }
    if (!formData.temperature) {
      errors.temperature = "Soil temperature is required";
    }
    if (!formData.availableNitrogen || Number(formData.availableNitrogen) < 0) {
      errors.availableNitrogen = "Available nitrogen must be 0 or greater";
    }
    if (
      !formData.availablePhosphorus ||
      Number(formData.availablePhosphorus) < 0
    ) {
      errors.availablePhosphorus = "Available phosphorus must be 0 or greater";
    }
    if (
      !formData.availablePotassium ||
      Number(formData.availablePotassium) < 0
    ) {
      errors.availablePotassium = "Available potassium must be 0 or greater";
    }

    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleInputChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    // Clear error for this field when user starts typing
    if (formErrors[name]) {
      setFormErrors((prev) => ({
        ...prev,
        [name]: "",
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) {
      return;
    }

    try {
      // First create the crop (backend creates empty soil data automatically)
      const cropData = {
        name: formData.name.trim(),
        plantId: Number(formData.plantId),
        locationId: Number(formData.locationId),
        expectedYield: Number(formData.expectedYield),
        fieldArea: Number(formData.fieldArea),
      };

      const newCrop = await dispatch(createCrop(cropData)).unwrap();

      // Then update the soil data with the provided values
      const soilData = {
        soilDataId: 0, // Will be set by backend
        cropId: newCrop.id,
        currentMoisture: Number(formData.currentMoisture),
        fieldCapacity: Number(formData.fieldCapacity),
        temperature: Number(formData.temperature),
        availableNitrogen: Number(formData.availableNitrogen),
        availablePhosphorus: Number(formData.availablePhosphorus),
        availablePotassium: Number(formData.availablePotassium),
      };

      await soilDataService.updateSoilData(soilData);

      navigate("/crops");
    } catch (error) {
      console.error("Failed to create crop:", error);
    }
  };

  if (loadingData) {
    return <div className="create-crop-loading">Loading form data...</div>;
  }

  if (dataError) {
    return <div className="create-crop-error">Error: {dataError}</div>;
  }

  return (
    <div className="create-crop-container">
      <div className="create-crop-header">
        <h1>Create New Crop</h1>
        <button
          type="button"
          onClick={() => navigate("/crops")}
          className="btn btn-secondary"
        >
          Back to Crops
        </button>
      </div>

      <form onSubmit={handleSubmit} className="create-crop-form">
        {error && <div className="form-error">{error}</div>}

        {/* Basic Crop Information */}
        <div className="form-section">
          <h2>Basic Information</h2>

          <div className="form-group">
            <label htmlFor="name">Crop Name *</label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleInputChange}
              className={formErrors.name ? "error" : ""}
              placeholder="Enter crop name"
            />
            {formErrors.name && (
              <span className="error-message">{formErrors.name}</span>
            )}
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="plantId">Plant Type *</label>
              <select
                id="plantId"
                name="plantId"
                value={formData.plantId}
                onChange={handleInputChange}
                className={formErrors.plantId ? "error" : ""}
              >
                <option value="">Select a plant</option>
                {plants.map((plant) => (
                  <option key={plant.plantId} value={plant.plantId}>
                    {plant.name}
                  </option>
                ))}
              </select>
              {formErrors.plantId && (
                <span className="error-message">{formErrors.plantId}</span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="locationId">Location *</label>
              <select
                id="locationId"
                name="locationId"
                value={formData.locationId}
                onChange={handleInputChange}
                className={formErrors.locationId ? "error" : ""}
              >
                <option value="">Select a location</option>
                {locations.map((location) => (
                  <option key={location.locationId} value={location.locationId}>
                    {location.name} ({location.latitude}, {location.longitude})
                  </option>
                ))}
              </select>
              {formErrors.locationId && (
                <span className="error-message">{formErrors.locationId}</span>
              )}
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="expectedYield">Expected Yield (kg) *</label>
              <input
                type="number"
                id="expectedYield"
                name="expectedYield"
                value={formData.expectedYield}
                onChange={handleInputChange}
                className={formErrors.expectedYield ? "error" : ""}
                placeholder="0"
                min="0"
                step="0.01"
              />
              {formErrors.expectedYield && (
                <span className="error-message">
                  {formErrors.expectedYield}
                </span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="fieldArea">Field Area (ha) *</label>
              <input
                type="number"
                id="fieldArea"
                name="fieldArea"
                value={formData.fieldArea}
                onChange={handleInputChange}
                className={formErrors.fieldArea ? "error" : ""}
                placeholder="0"
                min="0"
                step="0.01"
              />
              {formErrors.fieldArea && (
                <span className="error-message">{formErrors.fieldArea}</span>
              )}
            </div>
          </div>
        </div>

        {/* Soil Data Section */}
        <div className="form-section">
          <h2>Initial Soil Data</h2>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="currentMoisture">Current Moisture (%) *</label>
              <input
                type="number"
                id="currentMoisture"
                name="currentMoisture"
                value={formData.currentMoisture}
                onChange={handleInputChange}
                className={formErrors.currentMoisture ? "error" : ""}
                placeholder="0"
                min="0"
                max="100"
                step="0.1"
              />
              {formErrors.currentMoisture && (
                <span className="error-message">
                  {formErrors.currentMoisture}
                </span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="fieldCapacity">Field Capacity (%) *</label>
              <input
                type="number"
                id="fieldCapacity"
                name="fieldCapacity"
                value={formData.fieldCapacity}
                onChange={handleInputChange}
                className={formErrors.fieldCapacity ? "error" : ""}
                placeholder="0"
                min="0"
                step="0.1"
              />
              {formErrors.fieldCapacity && (
                <span className="error-message">
                  {formErrors.fieldCapacity}
                </span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="temperature">Soil Temperature (°C) *</label>
              <input
                type="number"
                id="temperature"
                name="temperature"
                value={formData.temperature}
                onChange={handleInputChange}
                className={formErrors.temperature ? "error" : ""}
                placeholder="0"
                step="0.1"
              />
              {formErrors.temperature && (
                <span className="error-message">{formErrors.temperature}</span>
              )}
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="availableNitrogen">
                Available Nitrogen (mg/kg) *
              </label>
              <input
                type="number"
                id="availableNitrogen"
                name="availableNitrogen"
                value={formData.availableNitrogen}
                onChange={handleInputChange}
                className={formErrors.availableNitrogen ? "error" : ""}
                placeholder="0"
                min="0"
                step="0.01"
              />
              {formErrors.availableNitrogen && (
                <span className="error-message">
                  {formErrors.availableNitrogen}
                </span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="availablePhosphorus">
                Available Phosphorus (mg/kg) *
              </label>
              <input
                type="number"
                id="availablePhosphorus"
                name="availablePhosphorus"
                value={formData.availablePhosphorus}
                onChange={handleInputChange}
                className={formErrors.availablePhosphorus ? "error" : ""}
                placeholder="0"
                min="0"
                step="0.01"
              />
              {formErrors.availablePhosphorus && (
                <span className="error-message">
                  {formErrors.availablePhosphorus}
                </span>
              )}
            </div>

            <div className="form-group">
              <label htmlFor="availablePotassium">
                Available Potassium (mg/kg) *
              </label>
              <input
                type="number"
                id="availablePotassium"
                name="availablePotassium"
                value={formData.availablePotassium}
                onChange={handleInputChange}
                className={formErrors.availablePotassium ? "error" : ""}
                placeholder="0"
                min="0"
                step="0.01"
              />
              {formErrors.availablePotassium && (
                <span className="error-message">
                  {formErrors.availablePotassium}
                </span>
              )}
            </div>
          </div>
        </div>

        <div className="form-actions">
          <button
            type="button"
            onClick={() => navigate("/crops")}
            className="btn btn-secondary"
          >
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? "Creating..." : "Create Crop"}
          </button>
        </div>
      </form>
    </div>
  );
};

export default CreateCrop;
