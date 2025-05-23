import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/hooks";
import {
  fetchPlants,
  clearPredictionResult,
  clearError,
} from "../../store/slices/predictionsSlice";
import SeedingPredictionForm from "./SeedingPredictionForm";
import "./Predictions.css";

const SeedingPredictionPage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { plants, plantsLoading, loading, error, predictionResult } =
    useAppSelector((state) => state.predictions);

  useEffect(() => {
    dispatch(fetchPlants());
    // Clear any previous prediction results
    dispatch(clearPredictionResult());

    return () => {
      // Clean up when component unmounts
      dispatch(clearPredictionResult());
      dispatch(clearError());
    };
  }, [dispatch]);

  const goBack = () => {
    navigate(-1);
  };

  return (
    <div className="prediction-container">
      <h1>Seeding Date Prediction</h1>
      <p className="prediction-description">
        Enter your crop details and get a recommendation for the optimal seeding
        date.
      </p>

      <SeedingPredictionForm
        plants={plants}
        plantsLoading={plantsLoading}
        isSubmitting={loading}
        error={error}
        predictionResult={predictionResult}
      />

      <div className="form-actions mt-4">
        <button onClick={goBack} className="btn btn-secondary">
          Back
        </button>
      </div>
    </div>
  );
};

export default SeedingPredictionPage;
