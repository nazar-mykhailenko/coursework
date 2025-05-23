import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import api from "../../services/api";
import "./DashboardPage.css";

const DashboardPage = () => {
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
        </div>{" "}
        <div className="dashboard-card">
          <h3>Predictions</h3>
          <p>Growth predictions and yield estimates</p>
          <Link to="/predictions/seeding" className="dashboard-button">
            Seeding Prediction
          </Link>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;
