import React, { useState, useEffect } from "react";
import { cropService } from "../../services/cropService";
import "./Calendar.css";

interface CalendarEvent {
  id: string;
  date: Date;
  title: string;
  type: "irrigation" | "fertilization" | "harvest" | "planting";
  cropName: string;
  details?: string;
}

interface CalendarProps {
  onEventClick?: (event: CalendarEvent) => void;
}

const Calendar: React.FC<CalendarProps> = ({ onEventClick }) => {
  const [currentDate, setCurrentDate] = useState(new Date());
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadEvents();
  }, []);

  const loadEvents = async () => {
    try {
      setLoading(true);
      setError(null);

      // Get all crops
      const cropsResult = await cropService.getCrops();
      const allEvents: CalendarEvent[] = [];

      // For each crop, get predictions and create events
      for (const crop of cropsResult.items) {
        try {
          const prediction = await cropService.getFullPrediction(crop.id);

          // Add irrigation event
          if (prediction.nextIrrigationDate) {
            allEvents.push({
              id: `irrigation-${crop.id}`,
              date: new Date(prediction.nextIrrigationDate),
              title: "Irrigation",
              type: "irrigation",
              cropName: crop.name,
              details: `${prediction.irrigationAmount}L needed`,
            });
          }

          // Add fertilization event
          if (prediction.nextFertilizationDate) {
            allEvents.push({
              id: `fertilization-${crop.id}`,
              date: new Date(prediction.nextFertilizationDate),
              title: "Fertilization",
              type: "fertilization",
              cropName: crop.name,
              details: `N: ${prediction.nitrogenFertilizerAmount}kg, P: ${prediction.phosphorusFertilizerAmount}kg, K: ${prediction.potassiumFertilizerAmount}kg`,
            });
          }

          // Add harvest event
          if (prediction.predictedHarvestDate) {
            allEvents.push({
              id: `harvest-${crop.id}`,
              date: new Date(prediction.predictedHarvestDate),
              title: "Harvest",
              type: "harvest",
              cropName: crop.name,
              details: `Expected yield: ${crop.expectedYield}kg`,
            });
          }

          // Add planting event (if planting date exists)
          if (crop.plantingDate) {
            allEvents.push({
              id: `planting-${crop.id}`,
              date: new Date(crop.plantingDate),
              title: "Planted",
              type: "planting",
              cropName: crop.name,
              details: `${crop.plant?.name || "Crop"} planted`,
            });
          }
        } catch (err) {
          console.warn(
            `Failed to load predictions for crop ${crop.name}:`,
            err
          );
        }
      }

      setEvents(allEvents);
    } catch (err) {
      setError("Failed to load calendar events");
      console.error("Error loading events:", err);
    } finally {
      setLoading(false);
    }
  };

  const getDaysInMonth = (date: Date) => {
    return new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
  };

  const getFirstDayOfMonth = (date: Date) => {
    return new Date(date.getFullYear(), date.getMonth(), 1).getDay();
  };

  const getEventsForDate = (date: Date) => {
    return events.filter(
      (event) =>
        event.date.getDate() === date.getDate() &&
        event.date.getMonth() === date.getMonth() &&
        event.date.getFullYear() === date.getFullYear()
    );
  };

  const navigateMonth = (direction: "prev" | "next") => {
    setCurrentDate((prev) => {
      const newDate = new Date(prev);
      if (direction === "prev") {
        newDate.setMonth(prev.getMonth() - 1);
      } else {
        newDate.setMonth(prev.getMonth() + 1);
      }
      return newDate;
    });
  };

  const goToToday = () => {
    setCurrentDate(new Date());
  };

  const renderCalendarDays = () => {
    const daysInMonth = getDaysInMonth(currentDate);
    const firstDay = getFirstDayOfMonth(currentDate);
    const days = [];

    // Add empty cells for days before the first day of the month
    for (let i = 0; i < firstDay; i++) {
      days.push(<div key={`empty-${i}`} className="calendar-day empty"></div>);
    }

    // Add days of the month
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth(),
        day
      );
      const dayEvents = getEventsForDate(date);
      const isToday = date.toDateString() === new Date().toDateString();

      days.push(
        <div key={day} className={`calendar-day ${isToday ? "today" : ""}`}>
          <span className="day-number">{day}</span>
          <div className="day-events">
            {dayEvents.map((event) => (
              <div
                key={event.id}
                className={`event event-${event.type}`}
                onClick={() => onEventClick?.(event)}
                title={`${event.title} - ${event.cropName}: ${event.details}`}
              >
                <span className="event-title">{event.title}</span>
                <span className="event-crop">{event.cropName}</span>
              </div>
            ))}
          </div>
        </div>
      );
    }

    return days;
  };

  if (loading) {
    return (
      <div className="calendar-container">
        <div className="calendar-loading">Loading calendar events...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="calendar-container">
        <div className="calendar-error">
          {error}
          <button onClick={loadEvents} className="retry-button">
            Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="calendar-container">
      <div className="calendar-header">
        <button onClick={() => navigateMonth("prev")} className="nav-button">
          ‹
        </button>
        <h3 className="calendar-title">
          {currentDate.toLocaleDateString("en-US", {
            month: "long",
            year: "numeric",
          })}
        </h3>
        <button onClick={() => navigateMonth("next")} className="nav-button">
          ›
        </button>
        <button onClick={goToToday} className="today-button">
          Today
        </button>
      </div>

      <div className="calendar-weekdays">
        {["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"].map((day) => (
          <div key={day} className="weekday">
            {day}
          </div>
        ))}
      </div>

      <div className="calendar-grid">{renderCalendarDays()}</div>

      <div className="calendar-legend">
        <div className="legend-item">
          <span className="legend-color irrigation"></span>
          <span>Irrigation</span>
        </div>
        <div className="legend-item">
          <span className="legend-color fertilization"></span>
          <span>Fertilization</span>
        </div>
        <div className="legend-item">
          <span className="legend-color harvest"></span>
          <span>Harvest</span>
        </div>
        <div className="legend-item">
          <span className="legend-color planting"></span>
          <span>Planting</span>
        </div>
      </div>
    </div>
  );
};

export default Calendar;
