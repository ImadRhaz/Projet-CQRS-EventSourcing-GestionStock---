// AddProduct.js
import React, { useState, useEffect } from 'react';
import {
    Container,
    TextField,
    Button,
    Typography,
    Box,
} from '@mui/material';
import Swal from 'sweetalert2';
import { useNavigate } from 'react-router-dom';
import { BASE_URL } from '../../config';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const AddProduct = () => {
    const [codeSite, setCodeSite] = useState('');
    const [deviceType, setDeviceType] = useState('');
    const [psSn, setPsSn] = useState('');
    const [dateEntre, setDateEntre] = useState('');
    const [expirationVerification, setExpirationVerification] = useState('');
    const [status, setStatus] = useState('ccs'); // Default to "En Attente"
    const [error, setError] = useState(null);
    const [loadingSubmit, setLoadingSubmit] = useState(false);
    const [expertId, setExpertId] = useState(null);
    const [roles, setRoles] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                const userId = decodedToken.nameid;
                setExpertId(userId);
                fetchUserRoles(userId);
            } catch (error) {
                console.error('Erreur lors du décodage du token:', error);
                setError('Token invalide. Veuillez vous reconnecter.');
            }
        } else {
            console.error('Aucun token trouvé dans localStorage');
            setError('Vous devez vous reconnecter.');
        }
    }, []);

    const fetchUserRoles = async (userId) => {
        try {
            const response = await axios.get(`${BASE_URL}Query/user-roles/${userId}`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                },
            });
            console.log('Rôles récupérés depuis l\'API:', response.data);
            setRoles(response.data);
        } catch (error) {
            console.error('Erreur lors de la récupération des rôles:', error);
            setError('Erreur lors de la récupération des rôles.');
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);
        setLoadingSubmit(true);

        Swal.fire({
            title: 'Êtes-vous sûr de vouloir sauvegarder cet FM1 ?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Oui, sauvegarder',
            cancelButtonText: 'Annuler',
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    const token = localStorage.getItem('token');
                    const payload = {
                        codeSite: codeSite.trim(),
                        deviceType: deviceType.trim(),
                        psSn: psSn.trim(),
                        dateEntre: new Date(dateEntre).toISOString(),
                        expirationVerification: new Date(expirationVerification).toISOString(),
                        status: status,
                        expertId: expertId,
                    };

                    console.log('Payload sent to API:', payload);

                    const response = await axios.post(`${BASE_URL}Command/add-fm1`, payload, {
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`,
                        },
                    });

                    console.log('Réponse de l\'API:', response);

                    if (response.status === 200 || response.status === 201) {
                        Swal.fire('Succès!', 'FM1 a été sauvegardé avec succès.', 'success');
                        setTimeout(() => {
                            navigate('/products');
                        }, 2000);
                    }
                } catch (err) {
                  console.error('API Error:', err);
                  let message = 'Erreur lors de la sauvegarde de FM1';

                    if (err.response?.data) {
                        if (typeof err.response.data === 'string') {
                            message = err.response.data;
                        } else if (err.response.data.message) {
                            message = err.response.data.message;
                        }  else if (err.response.data.errors) {

      console.log('Validation errors:', err.response.data.errors);

           const errorMessages = Object.values(err.response.data.errors)
                .map(errorArray => errorArray.join('\n'))
                .join('\n');
           message = `Validation errors:\n${errorMessages}`;
              }else if (err.response.data.Error) {
                            message = err.response.data.Error;
                        } else {
                            message = JSON.stringify(err.response.data);
                        }
                    } else if (err.message) {
                        message = err.message;
                    }
                     setError(message);

                      Swal.fire('Erreur', message, 'error');
                  }

                  finally {

           setLoadingSubmit(false);
         }
       } else {
                setLoadingSubmit(false);
            }
        });
    };

    return (
        <Container>
            <Box mb={2}>
                <Typography variant="body1" gutterBottom>
                    Expert ID: {expertId || 'Non disponible'}
                </Typography>
                <Typography variant="body1" gutterBottom>
                    Rôles: {roles.join(', ') || 'Aucun rôle'}
                </Typography>
            </Box>

            <form onSubmit={handleSubmit}>
                <Box mb={2}>
                    <TextField
                        label="Code Site"
                        variant="outlined"
                        fullWidth
                        required
                        value={codeSite}
                        onChange={(e) => setCodeSite(e.target.value)}
                    />
                </Box>

                <Box mb={2}>
                    <TextField
                        label="Device Type"
                        variant="outlined"
                        fullWidth
                        required
                        value={deviceType}
                        onChange={(e) => setDeviceType(e.target.value)}
                    />
                </Box>

                <Box mb={2}>
                    <TextField
                        label="PS SN"
                        variant="outlined"
                        fullWidth
                        required
                        value={psSn}
                        onChange={(e) => setPsSn(e.target.value)}
                    />
                </Box>

                <Box mb={2}>
                    <TextField
                        label="Date d'entrée"
                        type="date"
                        variant="outlined"
                        fullWidth
                        required
                        InputLabelProps={{ shrink: true }}
                        value={dateEntre}
                        onChange={(e) => setDateEntre(e.target.value)}
                    />
                </Box>

                <Box mb={2}>
                    <TextField
                        label="Date Verification"
                        type="date"
                        variant="outlined"
                        fullWidth
                        required
                        InputLabelProps={{ shrink: true }}
                        value={expirationVerification}
                        onChange={(e) => setExpirationVerification(e.target.value)}
                    />
                </Box>
                 {/* Champ hidden status if status is not from users*/}
                 <TextField
                         type="hidden"
                         value={status}
                       />

                {error && (
                    <Typography color="error" gutterBottom>
                        {error}
                    </Typography>
                )}

                <Button
                    variant="contained"
                    color="primary"
                    type="submit"
                    disabled={loadingSubmit}
                >
                    {loadingSubmit ? 'Sauvegarde en cours...' : 'Sauvegarder'}
                </Button>
            </form>
        </Container>
    );
};

export default AddProduct;