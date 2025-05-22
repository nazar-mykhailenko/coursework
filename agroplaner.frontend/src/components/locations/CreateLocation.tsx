import { useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/hooks";
import {
  createLocation,
  clearLocationError,
} from "../../store/slices/locationsSlice";
import LocationForm from "./LocationForm";

const CreateLocation = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { loading, error } = useAppSelector((state) => state.locations);

  const handleSubmit = async (data: {
    name: string;
    latitude: number;
    longitude: number;
  }) => {
    const resultAction = await dispatch(createLocation(data));

    if (createLocation.fulfilled.match(resultAction)) {
      navigate("/locations");
    }
  };

  return (
    <div className="location-container">
      <h1>Add New Location</h1>
      <LocationForm
        onSubmit={handleSubmit}
        isSubmitting={loading}
        error={error}
        submitButtonText="Create Location"
      />
    </div>
  );
};

export default CreateLocation;
