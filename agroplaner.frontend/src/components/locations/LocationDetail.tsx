import { useEffect } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/hooks";
import {
  fetchLocation,
  deleteLocation,
  clearCurrentLocation,
} from "../../store/slices/locationsSlice";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import "leaflet/dist/leaflet.css";

const LocationDetail = () => {
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

  const handleDelete = async () => {
    if (
      !id ||
      !window.confirm("Are you sure you want to delete this location?")
    ) {
      return;
    }

    const resultAction = await dispatch(deleteLocation(Number(id)));

    if (deleteLocation.fulfilled.match(resultAction)) {
      navigate("/locations");
    }
  };

  if (loading) {
    return <div className="loading">Loading location details...</div>;
  }

  if (error) {
    return (
      <div className="location-container">
        <Link to="/locations" className="back-link">
          ← Back to Locations
        </Link>
        <div className="error-message">{error}</div>
      </div>
    );
  }

  if (!currentLocation) {
    return (
      <div className="location-container">
        <Link to="/locations" className="back-link">
          ← Back to Locations
        </Link>
        <div className="error-message">Location not found</div>
      </div>
    );
  }

  return (
    <div className="location-container">
      <Link to="/locations" className="back-link">
        ← Back to Locations
      </Link>

      <div className="location-detail">
        <h1>{currentLocation.name}</h1>

        <div>
          <h3>Coordinates</h3>
          <p>Latitude: {currentLocation.latitude.toFixed(6)}</p>
          <p>Longitude: {currentLocation.longitude.toFixed(6)}</p>
        </div>

        <div className="map-container">
          <MapContainer
            center={[currentLocation.latitude, currentLocation.longitude]}
            zoom={13}
            style={{ height: "100%", width: "100%" }}
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <Marker
              position={[currentLocation.latitude, currentLocation.longitude]}
            >
              <Popup>
                <strong>{currentLocation.name}</strong>
              </Popup>
            </Marker>
          </MapContainer>
        </div>

        <div className="location-actions">
          <Link to={`/locations/${id}/edit`} className="btn btn-primary">
            Edit Location
          </Link>
          <button onClick={handleDelete} className="btn btn-danger">
            Delete Location
          </button>
        </div>
      </div>
    </div>
  );
};

export default LocationDetail;
