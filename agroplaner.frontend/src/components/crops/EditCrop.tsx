import React, { useState, useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate, useParams } from "react-router-dom";
import { type RootState, type AppDispatch } from "../../store";
import { fetchCrop, updateCrop } from "../../store/slices/cropsSlice";
import { plantService, type Plant } from "../../services/plantService";
import { locationService, type Location } from "../../services/locationService";
import { soilDataService, type SoilData } from "../../services/soilDataService";
import { type UpdateCropDto } from "../../services/cropService";
import "./EditCrop.css";

const EditCrop: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const { currentCrop, loading, error } = useSelector(
    (state: RootState) => state.crops
  );

  const [plants, setPlants] = useState<Plant[]>([]);
  const [locations, setLocations] = useState<Location[]>([]);
  const [soilData, setSoilData] = useState<SoilData | null>(null);
  const [loadingData, setLoadingData] = useState(true);
  const [dataError, setDataError] = useState<string | null>(null);

  const [formData, setFormData] = useState({
    name: "",
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
  const [irrigationData, setIrrigationData] = useState({
    amount: "",
  });

  const [fertilizationData, setFertilizationData] = useState({
    nitrogenAmount: "",
    phosphorusAmount: "",
    potassiumAmount: "",
  });

  const [formErrors, setFormErrors] = useState<Record<string, string>>({});
  const [activeTab, setActiveTab] = useState<
    "basic" | "irrigation" | "fertilization"
  >("basic");

  useEffect(() => {
    if (id) {
      dispatch(fetchCrop(Number(id)));
    }
  }, [dispatch, id]);

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

        // Load soil data for the crop
        if (id) {
          try {
            const soilDataResponse = await soilDataService.getSoilDataByCrop(
              Number(id)
            );
            setSoilData(soilDataResponse);
          } catch (error) {
            console.warn("No soil data found for crop:", error);
          }
        }
      } catch (error) {
        console.error("Failed to load data:", error);
        setDataError("Failed to load form data");
      } finally {
        setLoadingData(false);
      }
    };

    loadData();
  }, [id]);

  useEffect(() => {
    if (currentCrop) {
      setFormData({
        name: currentCrop.name,
        expectedYield: currentCrop.expectedYield.toString(),
        fieldArea: currentCrop.fieldArea.toString(),
        currentMoisture: soilData?.currentMoisture?.toString() || "",
        fieldCapacity: soilData?.fieldCapacity?.toString() || "",
        temperature: soilData?.temperature?.toString() || "",
        availableNitrogen: soilData?.availableNitrogen?.toString() || "",
        availablePhosphorus: soilData?.availablePhosphorus?.toString() || "",
        availablePotassium: soilData?.availablePotassium?.toString() || "",
      });
    }
  }, [currentCrop, soilData]);

  const validateBasicForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!formData.name.trim()) errors.name = "Crop name is required";
    if (!formData.expectedYield || Number(formData.expectedYield) <= 0) {
      errors.expectedYield = "Expected yield must be greater than 0";
    }
    if (!formData.fieldArea || Number(formData.fieldArea) <= 0) {
      errors.fieldArea = "Field area must be greater than 0";
    }

    // Soil data validation
    if (
      formData.currentMoisture &&
      (Number(formData.currentMoisture) < 0 ||
        Number(formData.currentMoisture) > 100)
    ) {
      errors.currentMoisture = "Current moisture must be between 0 and 100";
    }
    if (formData.fieldCapacity && Number(formData.fieldCapacity) <= 0) {
      errors.fieldCapacity = "Field capacity must be greater than 0";
    }
    if (formData.availableNitrogen && Number(formData.availableNitrogen) < 0) {
      errors.availableNitrogen = "Available nitrogen must be 0 or greater";
    }
    if (
      formData.availablePhosphorus &&
      Number(formData.availablePhosphorus) < 0
    ) {
      errors.availablePhosphorus = "Available phosphorus must be 0 or greater";
    }
    if (
      formData.availablePotassium &&
      Number(formData.availablePotassium) < 0
    ) {
      errors.availablePotassium = "Available potassium must be 0 or greater";
    }

    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
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

  const handleIrrigationChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setIrrigationData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleFertilizationChange = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const { name, value } = e.target;
    setFertilizationData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleUpdateCrop = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateBasicForm() || !currentCrop) {
      return;
    }

    try {
      const updateData: UpdateCropDto = {
        id: currentCrop.id,
        name: formData.name.trim(),
        expectedYield: Number(formData.expectedYield),
        fieldArea: Number(formData.fieldArea),
      };

      await dispatch(
        updateCrop({ id: currentCrop.id, data: updateData })
      ).unwrap();

      // Update soil data if provided
      if (
        soilData &&
        (formData.currentMoisture ||
          formData.fieldCapacity ||
          formData.temperature ||
          formData.availableNitrogen ||
          formData.availablePhosphorus ||
          formData.availablePotassium)
      ) {
        const soilUpdateData = {
          soilDataId: soilData.soilDataId,
          cropId: currentCrop.id,
          currentMoisture:
            Number(formData.currentMoisture) || soilData.currentMoisture,
          fieldCapacity:
            Number(formData.fieldCapacity) || soilData.fieldCapacity,
          temperature: Number(formData.temperature) || soilData.temperature,
          availableNitrogen:
            Number(formData.availableNitrogen) || soilData.availableNitrogen,
          availablePhosphorus:
            Number(formData.availablePhosphorus) ||
            soilData.availablePhosphorus,
          availablePotassium:
            Number(formData.availablePotassium) || soilData.availablePotassium,
        };

        await soilDataService.updateSoilData(soilUpdateData);
      }

      navigate("/crops");
    } catch (error) {
      console.error("Failed to update crop:", error);
    }
  };
  const handleApplyIrrigation = async () => {
    if (!currentCrop || !irrigationData.amount) {
      alert("Please fill in irrigation amount");
      return;
    }

    try {
      await soilDataService.applyIrrigation(currentCrop.id, {
        amount: Number(irrigationData.amount),
      });

      setIrrigationData({ amount: "" });
      alert("Irrigation applied successfully!");
    } catch (error) {
      console.error("Failed to apply irrigation:", error);
      alert("Failed to apply irrigation");
    }
  };

  const handleApplyFertilization = async () => {
    if (!currentCrop) {
      alert("No crop selected");
      return;
    }

    try {
      await soilDataService.applyFertilization(currentCrop.id, {
        nitrogenAmount: Number(fertilizationData.nitrogenAmount) || 0,
        phosphorusAmount: Number(fertilizationData.phosphorusAmount) || 0,
        potassiumAmount: Number(fertilizationData.potassiumAmount) || 0,
      });

      setFertilizationData({
        nitrogenAmount: "",
        phosphorusAmount: "",
        potassiumAmount: "",
      });
      alert("Fertilization applied successfully!");
    } catch (error) {
      console.error("Failed to apply fertilization:", error);
      alert("Failed to apply fertilization");
    }
  };

  if (loadingData || loading) {
    return <div className="edit-crop-loading">Loading crop data...</div>;
  }

  if (dataError || error) {
    return <div className="edit-crop-error">Error: {dataError || error}</div>;
  }

  if (!currentCrop) {
    return <div className="edit-crop-error">Crop not found</div>;
  }

  const selectedPlant = plants.find((p) => p.plantId === currentCrop.plantId);
  const selectedLocation = locations.find(
    (l) => l.locationId === currentCrop.locationId
  );

  return (
    <div className="edit-crop-container">
      <div className="edit-crop-header">
        <h1>Edit Crop: {currentCrop.name}</h1>
        <button
          type="button"
          onClick={() => navigate("/crops")}
          className="btn btn-secondary"
        >
          Back to Crops
        </button>
      </div>

      <div className="crop-info-bar">
        <div className="info-item">
          <span className="label">Plant:</span>
          <span className="value">
            {selectedPlant?.name || `ID: ${currentCrop.plantId}`}
          </span>
        </div>
        <div className="info-item">
          <span className="label">Location:</span>
          <span className="value">
            {selectedLocation?.name || `ID: ${currentCrop.locationId}`}
          </span>
        </div>
        <div className="info-item">
          <span className="label">Current GDD:</span>
          <span className="value">{currentCrop.cumulativeGDDToday}°C</span>
        </div>
      </div>

      <div className="edit-crop-tabs">
        <button
          className={`tab-button ${activeTab === "basic" ? "active" : ""}`}
          onClick={() => setActiveTab("basic")}
        >
          Basic Information
        </button>
        <button
          className={`tab-button ${activeTab === "irrigation" ? "active" : ""}`}
          onClick={() => setActiveTab("irrigation")}
        >
          Irrigation
        </button>
        <button
          className={`tab-button ${
            activeTab === "fertilization" ? "active" : ""
          }`}
          onClick={() => setActiveTab("fertilization")}
        >
          Fertilization
        </button>
      </div>

      <div className="edit-crop-content">
        {activeTab === "basic" && (
          <form onSubmit={handleUpdateCrop} className="edit-crop-form">
            <div className="form-section">
              <h2>Crop Information</h2>

              <div className="form-group">
                <label htmlFor="name">Crop Name *</label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  className={formErrors.name ? "error" : ""}
                />
                {formErrors.name && (
                  <span className="error-message">{formErrors.name}</span>
                )}
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
                    min="0"
                    step="0.01"
                  />
                  {formErrors.fieldArea && (
                    <span className="error-message">
                      {formErrors.fieldArea}
                    </span>
                  )}
                </div>
              </div>
            </div>

            <div className="form-section">
              <h2>Soil Data</h2>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="currentMoisture">Current Moisture (%)</label>
                  <input
                    type="number"
                    id="currentMoisture"
                    name="currentMoisture"
                    value={formData.currentMoisture}
                    onChange={handleInputChange}
                    className={formErrors.currentMoisture ? "error" : ""}
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
                  <label htmlFor="fieldCapacity">Field Capacity (%)</label>
                  <input
                    type="number"
                    id="fieldCapacity"
                    name="fieldCapacity"
                    value={formData.fieldCapacity}
                    onChange={handleInputChange}
                    className={formErrors.fieldCapacity ? "error" : ""}
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
                  <label htmlFor="temperature">Soil Temperature (°C)</label>
                  <input
                    type="number"
                    id="temperature"
                    name="temperature"
                    value={formData.temperature}
                    onChange={handleInputChange}
                    className={formErrors.temperature ? "error" : ""}
                    step="0.1"
                  />
                  {formErrors.temperature && (
                    <span className="error-message">
                      {formErrors.temperature}
                    </span>
                  )}
                </div>
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="availableNitrogen">
                    Available Nitrogen (mg/kg)
                  </label>
                  <input
                    type="number"
                    id="availableNitrogen"
                    name="availableNitrogen"
                    value={formData.availableNitrogen}
                    onChange={handleInputChange}
                    className={formErrors.availableNitrogen ? "error" : ""}
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
                    Available Phosphorus (mg/kg)
                  </label>
                  <input
                    type="number"
                    id="availablePhosphorus"
                    name="availablePhosphorus"
                    value={formData.availablePhosphorus}
                    onChange={handleInputChange}
                    className={formErrors.availablePhosphorus ? "error" : ""}
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
                    Available Potassium (mg/kg)
                  </label>
                  <input
                    type="number"
                    id="availablePotassium"
                    name="availablePotassium"
                    value={formData.availablePotassium}
                    onChange={handleInputChange}
                    className={formErrors.availablePotassium ? "error" : ""}
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
              <button
                type="submit"
                className="btn btn-primary"
                disabled={loading}
              >
                {loading ? "Updating..." : "Update Crop"}
              </button>
            </div>
          </form>
        )}

        {activeTab === "irrigation" && (
          <div className="irrigation-section">
            <h2>Apply Irrigation</h2>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="irrigationAmount">Amount (mm)</label>
                <input
                  type="number"
                  id="irrigationAmount"
                  name="amount"
                  value={irrigationData.amount}
                  onChange={handleIrrigationChange}
                  min="0"
                  step="0.1"
                  placeholder="Enter irrigation amount"
                />{" "}
              </div>
            </div>
            <div className="form-actions">
              <button
                type="button"
                onClick={handleApplyIrrigation}
                className="btn btn-primary"
              >
                Apply Irrigation
              </button>
            </div>
          </div>
        )}

        {activeTab === "fertilization" && (
          <div className="fertilization-section">
            <h2>Apply Fertilization</h2>
            <div className="form-row">
              <div className="form-group">
                <label htmlFor="nitrogenAmount">Nitrogen Amount (kg/ha)</label>
                <input
                  type="number"
                  id="nitrogenAmount"
                  name="nitrogenAmount"
                  value={fertilizationData.nitrogenAmount}
                  onChange={handleFertilizationChange}
                  min="0"
                  step="0.01"
                  placeholder="0"
                />
              </div>
              <div className="form-group">
                <label htmlFor="phosphorusAmount">
                  Phosphorus Amount (kg/ha)
                </label>
                <input
                  type="number"
                  id="phosphorusAmount"
                  name="phosphorusAmount"
                  value={fertilizationData.phosphorusAmount}
                  onChange={handleFertilizationChange}
                  min="0"
                  step="0.01"
                  placeholder="0"
                />
              </div>
              <div className="form-group">
                <label htmlFor="potassiumAmount">
                  Potassium Amount (kg/ha)
                </label>
                <input
                  type="number"
                  id="potassiumAmount"
                  name="potassiumAmount"
                  value={fertilizationData.potassiumAmount}
                  onChange={handleFertilizationChange}
                  min="0"
                  step="0.01"
                  placeholder="0"
                />
              </div>{" "}
            </div>
            <div className="form-actions">
              <button
                type="button"
                onClick={handleApplyFertilization}
                className="btn btn-primary"
              >
                Apply Fertilization
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default EditCrop;
