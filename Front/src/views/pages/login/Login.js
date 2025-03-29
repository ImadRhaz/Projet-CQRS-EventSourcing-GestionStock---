import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

// CoreUI components
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
import CIcon from '@coreui/icons-react';
import { cilLockLocked, cilUser } from '@coreui/icons';
import { BASE_URL } from '../../../config';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [twoFactorCode, setTwoFactorCode] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [twoFactorRequired, setTwoFactorRequired] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  // Fonction pour gérer la connexion
  const handleLogin = async (e) => {
    e.preventDefault();
    setErrorMessage('');
    setIsLoading(true);

    const data = { email, password };

    try {
      const response = await axios.post(`${BASE_URL}2fa/login`, data, { // Corrected URL
        headers: {
          'Content-Type': 'application/json',
        },
      });

      console.log("Login Response:", response.data);

      if (response.status === 200) {
        if (response.data.twoFactorRequired) {
          // Si la 2FA est requise, afficher le champ pour le code 2FA
          setTwoFactorRequired(true);
        } else if (response.data.token) {
          // Si la connexion réussit sans 2FA, stocker le token et rediriger
          const token = response.data.token;
          const decodedToken = jwtDecode(token);

          console.log("Decoded Token:", decodedToken);

          localStorage.setItem('idUser', decodedToken.nameid);
          localStorage.setItem('token', token);

          navigate('/dashboard');
        } else {
          setErrorMessage('Réponse inattendue du serveur.');
        }
      } else {
        setErrorMessage('Échec de la connexion. Veuillez vérifier vos identifiants.');
      }
    } catch (error) {
      console.error("Erreur de connexion:", error);
      setErrorMessage(error.response?.data?.message || 'Échec de la connexion. Veuillez réessayer.');
    } finally {
      setIsLoading(false);
    }
  };

  // Fonction pour gérer la soumission du code 2FA
  const handleTwoFactorSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');
    setIsLoading(true);

    const data = {
      email: email,
      twoFactorCode: twoFactorCode,
    };

    try {
      const response = await axios.post(`${BASE_URL}2fa/login-2fa`, data, { // Corrected URL
        headers: {
          'Content-Type': 'application/json',
        },
      });

      console.log("2FA Response:", response.data);

      if (response.status === 200) {
        // Si la 2FA est validée, stocker le token et rediriger
        const token = response.data.token;
        const decodedToken = jwtDecode(token);

        console.log("Decoded Token:", decodedToken);

        localStorage.setItem('idUser', decodedToken.nameid);
        localStorage.setItem('token', token);

        navigate('/dashboard');
      } else {
        setErrorMessage('Code 2FA invalide. Veuillez réessayer.');
      }
    } catch (error) {
      console.error("Erreur 2FA:", error);
      setErrorMessage(error.response?.data?.message || 'Échec de l\'authentification 2FA.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="bg-body-tertiary min-vh-100 d-flex flex-row align-items-center">
      <CContainer>
        <CRow className="justify-content-center">
          <CCol md={8}>
            <CCardGroup>
              <CCard className="p-4">
                <CCardBody>
                  {!twoFactorRequired ? (
                    // Formulaire de connexion standard
                    <CForm onSubmit={handleLogin}>
                      <h1>Connexion</h1>
                      <p className="text-body-secondary">Connectez-vous à votre compte</p>
                      {errorMessage && (
                        <div className="alert alert-danger" role="alert">
                          {errorMessage}
                        </div>
                      )}
                      <CInputGroup className="mb-3">
                        <CInputGroupText>
                          <CIcon icon={cilUser} />
                        </CInputGroupText>
                        <CFormInput
                          placeholder="Email"
                          autoComplete="email"
                          value={email}
                          onChange={(e) => setEmail(e.target.value)}
                          required
                        />
                      </CInputGroup>
                      <CInputGroup className="mb-4">
                        <CInputGroupText>
                          <CIcon icon={cilLockLocked} />
                        </CInputGroupText>
                        <CFormInput
                          type="password"
                          placeholder="Mot de passe"
                          autoComplete="current-password"
                          value={password}
                          onChange={(e) => setPassword(e.target.value)}
                          required
                        />
                      </CInputGroup>
                      <CRow>
                        <CCol xs={6}>
                          <CButton type="submit" color="primary" className="px-4" disabled={isLoading}>
                            {isLoading ? 'Chargement...' : 'Connexion'}
                          </CButton>
                        </CCol>
                        <CCol xs={6} className="text-right">
                          <Link to="/forgotPassword">
                            <CButton color="link" className="px-0">
                              Mot de passe oublié ?
                            </CButton>
                          </Link>
                        </CCol>
                      </CRow>
                    </CForm>
                  ) : (
                    // Formulaire pour le code 2FA
                    <CForm onSubmit={handleTwoFactorSubmit}>
                      <h1>Authentification à deux facteurs</h1>
                      <p className="text-body-secondary">Entrez votre code d'authentification</p>
                      {errorMessage && (
                        <div className="alert alert-danger" role="alert">
                          {errorMessage}
                        </div>
                      )}
                      <CInputGroup className="mb-3">
                        <CInputGroupText>
                          <CIcon icon={cilLockLocked} />
                        </CInputGroupText>
                        <CFormInput
                          type="text"
                          placeholder="Code d'authentification"
                          value={twoFactorCode}
                          onChange={(e) => setTwoFactorCode(e.target.value)}
                          required
                        />
                      </CInputGroup>
                      <CRow>
                        <CCol xs={12}>
                          <CButton type="submit" color="primary" className="px-4" disabled={isLoading}>
                            {isLoading ? 'Chargement...' : 'Valider'}
                          </CButton>
                        </CCol>
                      </CRow>
                    </CForm>
                  )}
                </CCardBody>
              </CCard>

              <CCard className="text-white bg-primary py-5" style={{ width: '44%' }}>
                <CCardBody className="text-center">
                  <div>
                    <h2>Inscription</h2>
                    <p>Ce site vous permet de gérer efficacement vos FM1 ainsi que votre stock.</p>
                    <Link to="/register">
                      <CButton color="primary" className="mt-3" active tabIndex={-1}>
                        S'inscrire maintenant !
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

export default Login;