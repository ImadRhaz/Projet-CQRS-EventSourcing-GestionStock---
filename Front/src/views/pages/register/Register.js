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

    // États pour la 2FA
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

            const response = await axios.post(`${BASE_URL}Command/register`, data);
            Swal.close();

            if (response.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Compte créé avec succès !',
                    showConfirmButton: false,
                    timer: 1500,
                });

                // Connexion automatique après l'inscription
                try {
                    const loginResponse = await axios.post(`${BASE_URL}account/login`, { email, password });
                    if (loginResponse.status === 200 && loginResponse.data.token) {
                        const token = loginResponse.data.token;
                        localStorage.setItem('token', token);

                        // Vérifier si le token est bien dans localStorage
                        const storedToken = localStorage.getItem('token');
                        console.log("Token trouvé dans localStorage:", storedToken); // Log pour vérifier

                        if (!storedToken) {
                            Swal.fire({
                                icon: 'error',
                                title: 'Erreur',
                                text: 'Token non trouvé. Veuillez vous reconnecter.',
                            });
                            return;
                        }

                        // Vérification de la validité du token
                        const decodedToken = JSON.parse(atob(storedToken.split('.')[1])); // Décodage du JWT
                        const tokenExpiration = decodedToken.exp * 1000; // Convertir en millisecondes
                        const currentTime = Date.now();

                        if (currentTime > tokenExpiration) {
                            Swal.fire({
                                icon: 'error',
                                title: 'Session expirée',
                                text: 'Votre session a expiré. Veuillez vous reconnecter.',
                            });
                            localStorage.removeItem('token'); // Retirer le token expiré
                            navigate('/login'); // Rediriger vers la page de connexion
                            return;
                        }

                        // Activer la 2FA
                        console.log("Token utilisé pour activer la 2FA:", storedToken); // Log pour vérifier

                        const enable2faResponse = await axios.post(
                            `${BASE_URL}account/enable-2fa`,
                            {},
                            { headers: { Authorization: `Bearer ${storedToken}` } }
                        );

                        if (enable2faResponse.status === 200) {
                            setSharedKey(enable2faResponse.data.sharedKey);
                            setQrCodeDataUri(enable2faResponse.data.qrCodeDataUri);
                            setShow2faSetup(true);
                        } else {
                            Swal.fire({
                                icon: 'error',
                                title: 'Erreur',
                                text: 'Erreur lors de l\'activation de la 2FA',
                            });
                        }
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Erreur',
                            text: 'Erreur lors de la connexion après l\'enregistrement',
                        });
                    }
                } catch (error) {
                    console.error("Erreur lors de l'activation de la 2FA:", error);

                    // Log de l'erreur complète pour voir la réponse
                    console.log("Réponse complète de l'erreur : ", error.response);

                    if (error.response && error.response.status === 401) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Token expiré ou invalide',
                            text: 'Votre session a expiré. Veuillez vous reconnecter.',
                        });
                        navigate('/login'); // Redirige l'utilisateur vers la page de connexion
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Erreur',
                            text: error.response?.data?.message || 'Erreur inconnue lors de l\'activation de la 2FA',
                        });
                    }
                }
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Erreur',
                    text: response.data.message || 'Une erreur inattendue est survenue',
                });
            }
        } catch (error) {
            Swal.close();

            const errorMessage =
                error.response?.data?.message ||
                (error.response?.data && JSON.stringify(error.response.data)) ||
                'Une erreur inattendue est survenue';

            Swal.fire({
                icon: 'error',
                title: 'Erreur',
                text: errorMessage,
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
                                                    />
                                                </CInputGroup>
                                            </CCol>
                                        </CRow>
                                        <CRow>
                                            <CCol md={12}>
                                                <CInputGroup className="mb-3">
                                                    <CInputGroupText>@</CInputGroupText>
                                                    <CFormInput
                                                        placeholder="Email"
                                                        autoComplete="email"
                                                        value={email}
                                                        onChange={(e) => setEmail(e.target.value)}
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
                                                placeholder="Mot de passe"
                                                autoComplete="new-password"
                                                value={password}
                                                onChange={(e) => setPassword(e.target.value)}
                                            />
                                        </CInputGroup>
                                        <CRow className="mb-3">
                                            <CCol md={12}>
                                                <Select
                                                    options={roleOptions}
                                                    onChange={(selectedOption) => setUserType(selectedOption)}
                                                    placeholder="Sélectionnez un rôle"
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
                                        <img src={qrCodeDataUri} alt="QR Code" />
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
