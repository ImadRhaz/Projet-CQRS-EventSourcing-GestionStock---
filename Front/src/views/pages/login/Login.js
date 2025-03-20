/*
LES ETAPES : 
-Importer des modules nécéssaire 
-Déclaration des composants Login ( de methode) Pour stocker les champs du formulaire 
-Fonction de Rechargement de la page lors de soumission de formulaire 
-Préparer les donnés a envoyer au backend 
-Envoie les données au backend via la methode Post 
-Si la connexion réussit (statut 200),Récupère le token JWT, Décode le token,
Afficher dans le log puis stock dans localstorage et redirige vers /dashboard else Msg d'erreur Récuperer de backend
-Le Formulaire Pour toute les champs 
-Exporation du Composent pour qu'il puisse étre utiliser ailleurs
*/


// 1- Importation des modules nécessaires
import React, { useState } from 'react'; // useState pour gérer les états
import { Link, useNavigate } from 'react-router-dom'; // Pour la navigation et les liens
import axios from 'axios'; // Pour les requêtes HTTP
import { jwtDecode } from 'jwt-decode'; // Pour décoder le token JWT

// Composants UI de CoreUI
import {
  CButton,
  CCard,
  CCardBody,
  CCardGroup,
  CCol,
  CContainer,
  CForm,
  CFormInput,
  CInputGroup,
  CInputGroupText,
  CRow,
} from '@coreui/react';
import CIcon from '@coreui/icons-react'; // Pour les icônes
import { cilLockLocked, cilUser } from '@coreui/icons'; // Icônes spécifiques
import { BASE_URL } from '../../../config'; // URL de base de l'API backend

const endpoint = 'Query/login'; // Ensure leading slash
// 2- Déclaration du composant Login
const Login = () => {
  // États pour stocker les valeurs des champs du formulaire
  const [email, setEmail] = useState(''); // Pour l'email
  const [password, setPassword] = useState(''); // Pour le mot de passe
  const [errorMessage, setErrorMessage] = useState(''); // Pour les messages d'erreur

  const navigate = useNavigate(); // Pour la navigation

  // 3- Fonction de soumission du formulaire
  const handleLogin = async (e) => {
    e.preventDefault(); // Empêche le rechargement de la page

         //  Préparation des données à envoyer au backend
    const data = { email, password };
    try {
      const response = await axios.post(`${BASE_URL}${endpoint}`, data, {
        headers: {
          'Content-Type': 'application/json',
        },
      });

      console.log(response.data); // Affiche la réponse du backend dans la console

      // 6-Si la connexion réussit (statut 200),Récupère le token JWT, Décode le token
      if (response.status === 200) {
        const token = response.data.token;  
        const decodedToken = jwtDecode(token); 

        console.log(decodedToken); // Affiche le token décodé dans la console

        // Stocke l'ID de l'utilisateur et le token dans le localStorage
        localStorage.setItem('idUser', decodedToken.nameid);
        localStorage.setItem('token', token);

        navigate('/dashboard'); // Redirige l'utilisateur vers le tableau de bord
      } else {
        // Si la connexion échoue, affiche un message d'erreur
        setErrorMessage('Une erreur est survenue lors de la connexion');
        console.error('Erreur :', response);
      }
    } catch (error) {
      // En cas d'erreur, affiche un message d'erreur spécifique ou générique
      setErrorMessage(error.response?.data?.message || 'Erreur de connexion');
      console.error('Erreur :', error.response);
    }
  };

  // 7- Rendu du formulaire
  return (
    <div className="bg-body-tertiary min-vh-100 d-flex flex-row align-items-center">
      <CContainer>
        <CRow className="justify-content-center">
          <CCol md={8}>
            <CCardGroup>
              {/* Carte pour le formulaire de connexion */}
              <CCard className="p-4">
                <CCardBody>
                  <CForm onSubmit={handleLogin}>
                    <h1>Login</h1>
                    <p className="text-body-secondary">Sign In to your account</p>

                    {/* Affiche un message d'erreur si présent */}
                    {errorMessage && (
                      <div className="alert alert-danger" role="alert">
                        {errorMessage}
                      </div>
                    )}

                    {/* Champ Email */}
                    <CInputGroup className="mb-3">
                      <CInputGroupText>
                        <CIcon icon={cilUser} /> {/* Icône utilisateur */}
                      </CInputGroupText>
                      <CFormInput
                        placeholder="Email"
                        autoComplete="email"
                        value={email} // Valeur du champ email
                        onChange={(e) => setEmail(e.target.value)} // Met à jour l'état email
                        required // Champ obligatoire
                      />
                    </CInputGroup>

                    {/* Champ Mot de passe */}
                    <CInputGroup className="mb-4">
                      <CInputGroupText>
                        <CIcon icon={cilLockLocked} /> {/* Icône mot de passe */}
                      </CInputGroupText>
                      <CFormInput
                        type="password"
                        placeholder="Password"
                        autoComplete="current-password"
                        value={password} // Valeur du champ mot de passe
                        onChange={(e) => setPassword(e.target.value)} // Met à jour l'état password
                        required // Champ obligatoire
                      />
                    </CInputGroup>

                    {/* Bouton de connexion et lien pour mot de passe oublié */}
                    <CRow>
                      <CCol xs={6}>
                        <CButton type="submit" color="primary" className="px-4">
                          Login
                        </CButton>
                      </CCol>
                      <CCol xs={6} className="text-right">
                        <Link to="/forgotPassword">
                          <CButton color="link" className="px-0">
                            Forgot password?
                          </CButton>
                        </Link>
                      </CCol>
                    </CRow>
                  </CForm>
                </CCardBody>
              </CCard>

              {/* Carte pour l'inscription */}
              <CCard className="text-white bg-primary py-5" style={{ width: '44%' }}>
                <CCardBody className="text-center">
                  <div>
                    <h2>Sign up</h2>
                    <p>
                      Ce site vous permet de gérer efficacement Vos FM1 ainsi que votre Stock.
                    </p>
                    <Link to="/register">
                      <CButton color="primary" className="mt-3" active tabIndex={-1}>
                        Register Now!
                      </CButton>
                    </Link>
                  </div>
                </CCardBody>
              </CCard>
            </CCardGroup>
          </CCol>
        </CRow>
      </CContainer>
    </div>
  );
};

// 8- Exportation du composant
export default Login;