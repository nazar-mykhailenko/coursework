import { useState } from "react";
import { Link } from "react-router-dom";
import Calendar from "../calendar/Calendar";
import "./DashboardPage.css";

interface CalendarEvent {
  id: string;
  date: Date;
  title: string;
  type: "irrigation" | "fertilization" | "harvest" | "planting";
  cropName: string;
  details?: string;
}

const DashboardPage = () => {
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(
    null
  );

  const handleEventClick = (event: CalendarEvent) => {
    setSelectedEvent(event);
  };

  const closeEventModal = () => {
    setSelectedEvent(null);
  };

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h2>Welcome to AgroPlaner</h2>
        <p className="dashboard-subtitle">
          Your agricultural planning dashboard
        </p>
      </div>

      <div className="dashboard-content">
        <div className="calendar-section">
          <h3>Agricultural Calendar</h3>
          <Calendar onEventClick={handleEventClick} />
        </div>
      </div>

      {/* Event Details Modal */}
      {selectedEvent && (
        <div className="event-modal-overlay" onClick={closeEventModal}>
          <div className="event-modal" onClick={(e) => e.stopPropagation()}>
            <div className="event-modal-header">
              <h3>{selectedEvent.title}</h3>
              <button className="close-button" onClick={closeEventModal}>
                ×
              </button>
            </div>
            <div className="event-modal-content">
              <p>
                <strong>Crop:</strong> {selectedEvent.cropName}
              </p>
              <p>
                <strong>Date:</strong> {selectedEvent.date.toLocaleDateString()}
              </p>
              <p>
                <strong>Type:</strong> {selectedEvent.type}
              </p>
              {selectedEvent.details && (
                <p>
                  <strong>Details:</strong> {selectedEvent.details}
                </p>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default DashboardPage;
