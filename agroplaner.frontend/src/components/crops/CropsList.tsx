import React, { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { type RootState, type AppDispatch } from "../../store";
import { fetchCrops, deleteCrop } from "../../store/slices/cropsSlice";
import "./CropsList.css";

const CropsList: React.FC = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { crops, loading, error, totalCount } = useSelector(
    (state: RootState) => state.crops
  );

  useEffect(() => {
    dispatch(fetchCrops());
  }, [dispatch]);

  const handleDelete = async (id: number, name: string) => {
    if (window.confirm(`Are you sure you want to delete crop "${name}"?`)) {
      try {
        await dispatch(deleteCrop(id)).unwrap();
        // Refresh the list after deletion
        dispatch(fetchCrops());
      } catch (error) {
        console.error("Failed to delete crop:", error);
      }
    }
  };

  if (loading) {
    return <div className="crops-loading">Loading crops...</div>;
  }

  if (error) {
    return <div className="crops-error">Error: {error}</div>;
  }

  return (
    <div className="crops-list-container">
      <div className="crops-header">
        <h1>Crops Management</h1>
        <Link to="/crops/create" className="btn btn-primary">
          Create New Crop
        </Link>
      </div>

      <div className="crops-stats">
        <span className="total-count">Total Crops: {totalCount}</span>
      </div>

      {crops.length === 0 ? (
        <div className="no-crops">
          <p>No crops found. Start by creating your first crop!</p>
          <Link to="/crops/create" className="btn btn-primary">
            Create First Crop
          </Link>
        </div>
      ) : (
        <div className="crops-grid">
          {crops.map((crop) => (
            <div key={crop.id} className="crop-card">
              <div className="crop-card-header">
                <h3>{crop.name}</h3>
                <span className="crop-id">ID: {crop.id}</span>
              </div>

              <div className="crop-card-body">
                <div className="crop-info">
                  <div className="info-item">
                    <span className="label">Plant ID:</span>
                    <span className="value">{crop.plantId}</span>
                  </div>
                  <div className="info-item">
                    <span className="label">Location ID:</span>
                    <span className="value">{crop.locationId}</span>
                  </div>
                  <div className="info-item">
                    <span className="label">Expected Yield:</span>
                    <span className="value">{crop.expectedYield} kg</span>
                  </div>
                  <div className="info-item">
                    <span className="label">Field Area:</span>
                    <span className="value">{crop.fieldArea} ha</span>
                  </div>
                  <div className="info-item">
                    <span className="label">Current GDD:</span>
                    <span className="value">{crop.cumulativeGDDToday}°C</span>
                  </div>
                </div>
              </div>

              <div className="crop-card-actions">
                <Link to={`/crops/${crop.id}`} className="btn btn-info">
                  View Details
                </Link>
                <Link
                  to={`/crops/${crop.id}/edit`}
                  className="btn btn-secondary"
                >
                  Edit
                </Link>
                <button
                  onClick={() => handleDelete(crop.id, crop.name)}
                  className="btn btn-danger"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default CropsList;
