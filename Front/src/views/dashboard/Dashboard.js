import React from 'react';
import './App.css';

const Dashboard = () => {
  return (
    <div className="dashboard-container">
      <div className="message-container">
        <h1>Gestion des voitures</h1>
        <ul className="feature-list">
          <li><strong>Enregistrement des véhicules :</strong> Ajoutez et gérez facilement tous vos véhicules dans le système, avec des informations détaillées.</li>
          <li><strong>Suivi des entretiens :</strong> Gardez un historique des tâches de maintenance pour chaque véhicule, incluant les coûts et les calendriers.</li>
         
          <li><strong>Coordination des tâches :</strong> Assignez et gérez efficacement les tâches liées aux réparations ou améliorations des véhicules.</li>
        </ul>
      </div>
    </div>
  );
};

export default Dashboard;
