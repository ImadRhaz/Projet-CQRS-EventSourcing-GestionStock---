import React from 'react';
import './App.css';
//import 'bootstrap/dist/css/bootstrap.min.css';

const Dashboard = () => {
  return (
    <div className="dashboard-container">
      <div className="message-container">
        <h1>Stock and Order Management</h1>
        <ul className="feature-list">
          <li><strong>Radar Registration:</strong> Ensure that each radar is properly registered in the system.</li>
          <li><strong>Detailed Tracking:</strong> Get comprehensive information on the status of each radar, associated parts, and ongoing orders.</li>
          <li><strong>Smooth and Organized Management:</strong> Benefit from effective resource management for optimal radar maintenance.</li>
          <li><strong>Order Management:</strong> Facilitate the tracking and management of parts orders for radar maintenance.</li>
        </ul>
      </div>
    </div>
  );
};

export default Dashboard;