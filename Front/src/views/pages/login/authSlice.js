// 1- Importation de la fonction `createSlice` de Redux Toolkit
// `createSlice` simplifie la création d'un slice Redux en générant automatiquement les actions et le reducer.
import { createSlice } from '@reduxjs/toolkit';

// 2- Déclaration de l'état initial (`initialState`)
// L'état initial définit la structure et les valeurs par défaut du slice.
const initialState = {
  isAuthenticated: false, // Indique si l'utilisateur est connecté (false par défaut)
  user: null, // Stocke les informations de l'utilisateur connecté (null par défaut)
};

// 3- Création du slice avec `createSlice`
// Un slice est une partie du store Redux qui gère un état spécifique (ici, l'authentification).
const authSlice = createSlice({
  name: 'auth', // Nom du slice (utilisé pour générer les types d'actions)
  initialState, // État initial du slice
  reducers: {
    // 4- Définition des reducers
    // Les reducers sont des fonctions qui mettent à jour l'état en réponse à des actions.

    // Reducer pour la connexion (`login`)
    login: (state, action) => {
      // Met à jour l'état pour indiquer que l'utilisateur est connecté
      state.isAuthenticated = true;

      // Stocke les informations de l'utilisateur dans l'état
      // `action.payload` contient les données de l'utilisateur (par exemple, son nom, son email, etc.)
      state.user = action.payload;
    },

    // Reducer pour la déconnexion (`logout`)
    logout: (state) => {
      // Met à jour l'état pour indiquer que l'utilisateur est déconnecté
      state.isAuthenticated = false;

      // Supprime les informations de l'utilisateur de l'état
      state.user = null;
    },
  },
});

// 5- Exportation des actions
// Les actions sont des fonctions qui déclenchent les reducers.
// Ici, on exporte les actions `login` et `logout` pour qu'elles puissent être utilisées dans les composants.
export const { login, logout } = authSlice.actions;

// 6- Exportation du reducer
// Le reducer est une fonction qui gère les mises à jour de l'état en fonction des actions.
// On exporte le reducer généré par `createSlice` pour l'ajouter au store Redux.
export default authSlice.reducer;