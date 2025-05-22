import { useEffect } from "react";
import { Link } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../store/hooks";
import {
  fetchLocations,
  deleteLocation,
} from "../../store/slices/locationsSlice";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import "./LocationPages.css";
import "leaflet/dist/leaflet.css";
import L from "leaflet";

// Fix for default leaflet icon missing
// @ts-ignore
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl:
    "https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon-2x.png",
  iconUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.7.1/dist/images/marker-shadow.png",
});

const LocationsList = () => {
  const dispatch = useAppDispatch();
  const { locations, loading, error } = useAppSelector(
    (state) => state.locations
  );

  useEffect(() => {
    dispatch(fetchLocations());
  }, [dispatch]);
  const handleDelete = async (id: number) => {
    if (
      window.confirm(
        "Are you sure you want to delete this location? WARNING: Deleting this location will also delete all crops assigned to it."
      )
    ) {
      await dispatch(deleteLocation(id));
    }
  };

  if (loading) {
    return <div className="loading">Loading locations...</div>;
  }

  return (
    <div className="location-container">
      <div className="location-header">
        <h1>My Locations</h1>
        <Link to="/locations/new" className="btn btn-primary">
          Add New Location
        </Link>
      </div>

      {error && <div className="error-message">{error}</div>}

      {locations.length === 0 ? (
        <div>
          <p>You haven't added any locations yet.</p>
          <p>
            <Link to="/locations/new">Add your first location</Link> to get
            started.
          </p>
        </div>
      ) : (
        <>
          {/* Map showing all locations */}
          <MapContainer
            center={[51.505, -0.09]} // Default center
            zoom={3}
            className="map-container"
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            {locations.map((location) => (
              <Marker
                key={location.locationId}
                position={[location.latitude, location.longitude]}
              >
                <Popup>
                  <strong>{location.name}</strong>
                  <br />
                  <Link to={`/locations/${location.locationId}`}>
                    View details
                  </Link>
                </Popup>
              </Marker>
            ))}
          </MapContainer>

          <div className="location-list">
            {locations.map((location) => (
              <div key={location.locationId} className="location-card">
                <h3>{location.name}</h3>
                <div>
                  <small>
                    Coordinates: {location.latitude.toFixed(4)},{" "}
                    {location.longitude.toFixed(4)}
                  </small>
                </div>

                {/* Mini map for each location */}
                <div className="mini-map">
                  <MapContainer
                    center={[location.latitude, location.longitude]}
                    zoom={10}
                    zoomControl={false}
                    attributionControl={false}
                    style={{ height: "100%", width: "100%" }}
                  >
                    <TileLayer url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" />
                    <Marker
                      position={[location.latitude, location.longitude]}
                    />
                  </MapContainer>
                </div>

                <div className="location-card-footer">
                  <Link
                    to={`/locations/${location.locationId}`}
                    className="btn"
                  >
                    View Details
                  </Link>
                  <button
                    onClick={() => handleDelete(location.locationId)}
                    className="btn btn-danger"
                  >
                    Delete
                  </button>
                </div>
              </div>
            ))}
          </div>
        </>
      )}
    </div>
  );
};

export default LocationsList;
