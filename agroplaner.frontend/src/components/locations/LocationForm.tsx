import { useState, useEffect, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { MapContainer, TileLayer, Marker, useMapEvents } from "react-leaflet";
import { LatLng } from "leaflet";
import "leaflet/dist/leaflet.css";
import type { Location } from "../../services/locationService";

interface LocationFormProps {
  initialData?: Location;
  onSubmit: (data: {
    name: string;
    latitude: number;
    longitude: number;
  }) => Promise<void>;
  isSubmitting: boolean;
  error: string | null;
  submitButtonText: string;
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

const LocationForm = ({
  initialData,
  onSubmit,
  isSubmitting,
  error,
  submitButtonText,
}: LocationFormProps) => {
  const [name, setName] = useState(initialData?.name || "");
  const [position, setPosition] = useState<[number, number]>(
    initialData ? [initialData.latitude, initialData.longitude] : [0, 0]
  );
  const [mapCenter, setMapCenter] = useState<[number, number]>(
    initialData
      ? [initialData.latitude, initialData.longitude]
      : [51.505, -0.09]
  );
  const [nameError, setNameError] = useState("");

  const handlePositionChange = (newPosition: [number, number]) => {
    setPosition(newPosition);
  };

  const validateForm = () => {
    let isValid = true;

    if (!name.trim()) {
      setNameError("Location name is required");
      isValid = false;
    } else {
      setNameError("");
    }

    if (position[0] === 0 && position[1] === 0) {
      isValid = false;
    }

    return isValid;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    await onSubmit({
      name,
      latitude: position[0],
      longitude: position[1],
    });
  };

  return (
    <div className="location-form">
      <Link to="/locations" className="back-link">
        ← Back to Locations
      </Link>

      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="name">Location Name</label>
          <input
            type="text"
            id="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Enter a name for this location"
            className={nameError ? "error" : ""}
          />
          {nameError && <div className="field-error">{nameError}</div>}
        </div>

        <div className="coordinates-display">
          <p>
            {position[0] !== 0 && position[1] !== 0
              ? `Selected coordinates: ${position[0].toFixed(
                  6
                )}, ${position[1].toFixed(6)}`
              : "Click on the map to select a location"}
          </p>
        </div>

        <div className="form-group">
          <label>Location Map (Click to set location)</label>
          <MapContainer
            center={mapCenter}
            zoom={3}
            className="map-container"
            style={{ height: "400px" }}
          >
            <TileLayer
              attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />
            <LocationMarker
              position={position}
              setPosition={handlePositionChange}
            />
          </MapContainer>
        </div>

        <div className="form-actions">
          <Link to="/locations" className="btn btn-secondary">
            Cancel
          </Link>
          <button
            type="submit"
            className="btn btn-primary"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Saving..." : submitButtonText}
          </button>
        </div>
      </form>
    </div>
  );
};

export default LocationForm;
