import { useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/hooks";
import {
  fetchLocation,
  updateLocation,
  clearCurrentLocation,
} from "../../store/slices/locationsSlice";
import LocationForm from "./LocationForm";

const EditLocation = () => {
  const { id } = useParams<{ id: string }>();
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { currentLocation, loading, error } = useAppSelector(
    (state) => state.locations
  );

  useEffect(() => {
    if (id) {
      dispatch(fetchLocation(Number(id)));
    }

    // Clean up when component unmounts
    return () => {
      dispatch(clearCurrentLocation());
    };
  }, [dispatch, id]);

  const handleSubmit = async (data: {
    name: string;
    latitude: number;
    longitude: number;
  }) => {
    if (!id) return;

    const resultAction = await dispatch(
      updateLocation({ id: Number(id), data })
    );

    if (updateLocation.fulfilled.match(resultAction)) {
      navigate(`/locations/${id}`);
    }
  };

  if (loading && !currentLocation) {
    return <div className="loading">Loading location...</div>;
  }

  if (!currentLocation && !loading) {
    return <div className="error-message">Location not found</div>;
  }

  return (
    <div className="location-container">
      <h1>Edit Location</h1>
      {currentLocation && (
        <LocationForm
          initialData={currentLocation}
          onSubmit={handleSubmit}
          isSubmitting={loading}
          error={error}
          submitButtonText="Save Changes"
        />
      )}
    </div>
  );
};

export default EditLocation;
