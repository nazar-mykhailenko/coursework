import { useState, useEffect } from "react";
import api from "../../services/api";
import "./DashboardPage.css";

const DashboardPage = () => {
  const [userData, setUserData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        // This is a placeholder - replace with actual API endpoint to get user data
        // const response = await api.get('/user/profile');
        // setUserData(response.data);

        // Simulated data for now
        setUserData({
          username: "User",
          email: "user@example.com",
        });
        setLoading(false);
      } catch (err) {
        setError("Failed to load user data");
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <div className="dashboard-loading">Loading...</div>;
  }

  if (error) {
    return <div className="dashboard-error">{error}</div>;
  }

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <h2>Welcome to AgroPlaner</h2>
        <p className="dashboard-subtitle">Your agricultural planning tool</p>
      </div>

      <div className="dashboard-grid">
        <div className="dashboard-card">
          <h3>My Crops</h3>
          <p>Manage your crops and planting schedules</p>
          <button className="dashboard-button">View Crops</button>
        </div>

        <div className="dashboard-card">
          <h3>Weather Data</h3>
          <p>Check weather forecasts and historical data</p>
          <button className="dashboard-button">View Weather</button>
        </div>

        <div className="dashboard-card">
          <h3>Soil Analysis</h3>
          <p>Review soil data and recommendations</p>
          <button className="dashboard-button">View Soil Data</button>
        </div>

        <div className="dashboard-card">
          <h3>Predictions</h3>
          <p>Growth predictions and yield estimates</p>
          <button className="dashboard-button">View Predictions</button>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
