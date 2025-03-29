import React, { useState } from 'react';
import Select from 'react-select';
import { BASE_URL } from '../../../config';
import {
  CButton,
  CCard,
  CCardBody,
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
import axios from 'axios';
import Swal from 'sweetalert2';
import { useNavigate } from 'react-router-dom';

const Register = () => {
  const [nom, setNom] = useState('');
  const [prenom, setPrenom] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [userType, setUserType] = useState(null);
  const [sharedKey, setSharedKey] = useState('');
  const [qrCodeDataUri, setQrCodeDataUri] = useState('');
  const [show2faSetup, setShow2faSetup] = useState(false);
  const navigate = useNavigate();

  const roleOptions = [
    { value: 'Expert', label: 'Expert' },
    { value: 'Magasinier', label: 'Magasinier' },
  ];

  const handleRegister = async (e) => {
    e.preventDefault();

    if (!userType) {
      Swal.fire({
        icon: 'error',
        title: 'Erreur',
        text: 'Veuillez sélectionner un type d\'utilisateur',
      });
      return;
    }

    const data = {
      nom,
      prenom,
      email,
      password,
      userType: userType.value,
    };

    try {
      Swal.fire({
        title: 'Votre demande est en cours',
        text: 'Veuillez patienter...',
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });

      // Étape 1 : Enregistrement de l'utilisateur
      const response = await axios.post(`${BASE_URL}Command/register`, data);
      Swal.close();

      if (response.status === 200) {
        Swal.fire({
          icon: 'success',
          title: 'Compte créé avec succès !',
          showConfirmButton: false,
          timer: 1500,
        });

        // Étape 2 : Attendre 2 secondes pour permettre la synchronisation
        await new Promise((resolve) => setTimeout(resolve, 2000));

        // Étape 3 : Activer la 2FA pour l'utilisateur nouvellement créé
        try {
          const enable2faResponse = await axios.post(
            `${BASE_URL}2fa/enable-2fa`,
            { email },
          );

          if (enable2faResponse.status === 200) {
            setSharedKey(enable2faResponse.data.sharedKey);
            setQrCodeDataUri(enable2faResponse.data.qrCodeDataUri);
            setShow2faSetup(true);
          } else {
            // Check specifically for password complexity error
            if (enable2faResponse.data?.message?.includes("Password must be complex")) {
              Swal.fire({
                icon: 'error',
                title: 'Erreur de mot de passe',
                text: 'Le mot de passe doit être complexe (par exemple, Exemple@12345).',
              });
            } else {
              console.error("Erreur 2FA (status non 200):", enable2faResponse.data);
               Swal.fire({
                icon: 'error',
                title: 'Erreur 2FA',
                text: enable2faResponse.data?.message || 'Erreur lors de l\'activation de la 2FA.',
              });
            }
           
          }
        } catch (error) {
             if (error.response?.data?.message?.includes("Password must be complex")) {
            Swal.fire({
              icon: 'error',
              title: 'Erreur de mot de passe',
              text: 'Le mot de passe doit être complexe (par exemple, Exemple@12345).',
            });
          }
           else {
          let errorMessage = 'Erreur lors de l\'activation de la 2FA.';
          if (error.response && error.response.data && error.response.data.message) {
            errorMessage = error.response.data.message;
          }
       
            Swal.fire({
              icon: 'error',
              title: 'Erreur 2FA',
              text: errorMessage,
            });
          }
          console.error("Erreur 2FA (exception):", error);
        }
      } else {
        Swal.fire({
          icon: 'error',
          title: 'Erreur d\'inscription',
          text: response.data?.message || 'Erreur inattendue lors de l\'inscription.',
        });
      }
    } catch (error) {
      console.error("Erreur d'inscription:", error);
      Swal.fire({
        icon: 'error',
        title: 'Erreur',
        text: error.response?.data?.message || 'Erreur inattendue lors de l\'inscription.',
      });
    }
  };

  return (
    <div className="bg-body-tertiary min-vh-100 d-flex flex-row align-items-center">
      <CContainer fluid>
        <CRow className="justify-content-center">
          <CCol md={9} lg={7} xl={6}>
            <CCard className="mx-4">
              <CCardBody className="p-4">
                {!show2faSetup ? (
                  <CForm onSubmit={handleRegister}>
                    <h1>Inscription</h1>
                    <p className="text-body-secondary">Créez votre compte</p>
                    <CRow>
                      <CCol md={6}>
                        <CInputGroup className="mb-3">
                          <CInputGroupText>
                            <CIcon icon={cilUser} />
                          </CInputGroupText>
                          <CFormInput
                            placeholder="Nom"
                            autoComplete="nom"
                            value={nom}
                            onChange={(e) => setNom(e.target.value)}
                            required
                          />
                        </CInputGroup>
                      </CCol>
                      <CCol md={6}>
                        <CInputGroup className="mb-3">
                          <CInputGroupText>
                            <CIcon icon={cilUser} />
                          </CInputGroupText>
                          <CFormInput
                            placeholder="Prénom"
                            autoComplete="prenom"
                            value={prenom}
                            onChange={(e) => setPrenom(e.target.value)}
                            required
                          />
                        </CInputGroup>
                      </CCol>
                    </CRow>
                    <CRow>
                      <CCol md={12}>
                        <CInputGroup className="mb-3">
                          <CInputGroupText>@</CInputGroupText>
                          <CFormInput
                            type="email"
                            placeholder="Email"
                            autoComplete="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                          />
                        </CInputGroup>
                      </CCol>
                    </CRow>
                    <CInputGroup className="mb-3">
                      <CInputGroupText>
                        <CIcon icon={cilLockLocked} />
                      </CInputGroupText>
                      <CFormInput
                        type="password"
                        placeholder="Mps doit etre Complex Ex: Exemple@12345"
                        autoComplete="new-password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                      />
                    </CInputGroup>
                    <CRow className="mb-3">
                      <CCol md={12}>
                        <Select
                          options={roleOptions}
                          onChange={(selectedOption) => setUserType(selectedOption)}
                          placeholder="Sélectionnez un rôle"
                          required
                        />
                      </CCol>
                    </CRow>
                    <div className="d-grid">
                      <CButton type="submit" color="success">
                        Créer un compte
                      </CButton>
                    </div>
                  </CForm>
                ) : (
                  <div>
                    <h2>Configuration de la 2FA</h2>
                    <p>Scannez ce QR code avec votre application :</p>
                    <img src={qrCodeDataUri} alt="QR Code" style={{ width: '200px', height: '200px' }} />
                    <p>Ou entrez cette clé manuellement :</p>
                    <p>{sharedKey}</p>
                    <CButton color="primary" onClick={() => navigate('/login')}>
                      Retour à la connexion
                    </CButton>
                  </div>
                )}
              </CCardBody>
            </CCard>
          </CCol>
        </CRow>
      </CContainer>
    </div>
  );
};

export default Register;