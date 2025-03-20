/*
Résumé des étapes:

- Importation des modules : Importation des composants, icônes, et bibliothèques nécessaires.
- Déclaration du composant : Déclaration des états et des fonctions.
- Validation des champs : Vérification que tous les champs requis sont remplis.
- Soumission du formulaire : Envoi des données du formulaire au backend pour mettre à jour une voiture.
- Rendu du formulaire : Affichage des champs du formulaire avec gestion des erreurs de validation.
*/

// 1- Importation des modules nécessaires 
/*
import React, { useState } from 'react'; // Hooks React
import {
    Container,
    TextField,
    Button,
    Typography,
    Box,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
} from '@mui/material'; // Composants Material-UI
import Swal from 'sweetalert2'; // Pour les alertes stylisées
import { BASE_URL } from '../../config'; // URL de base de l'API
import axios from 'axios'; // Pour les requêtes HTTP
import { useNavigate } from 'react-router-dom'; // Pour la navigation

// 2- Déclaration du composant EditProduct
const EditProduct = ({ product, onSave, onCancel }) => {
    // États pour les champs du formulaire
    const [marque, setMarque] = useState(product?.marque || ''); // Marque de la voiture
    const [type, setType] = useState(product?.type || ''); // Type de la voiture
    const [matricule, setMatricule] = useState(product?.matricule || ''); // Matricule de la voiture
    const [status, setStatus] = useState(product?.status || 1); // Statut de la voiture (1 par défaut : En Attente)
    const [error, setError] = useState(null); // Erreur
    const [loadingSubmit, setLoadingSubmit] = useState(false); // Chargement lors de la soumission
    const [validationErrors, setValidationErrors] = useState({}); // Erreurs de validation
    const navigate = useNavigate(); // Pour la navigation

    // 3- Validation des champs du formulaire
    const validateForm = () => {
        let errors = {};
        if (!marque.trim()) errors.marque = 'La marque est requise'; // Vérifie si la marque est vide
        if (!type.trim()) errors.type = 'Le type est requis'; // Vérifie si le type est vide
        if (!matricule.trim()) errors.matricule = 'Le matricule est requis'; // Vérifie si le matricule est vide
        if (!status) errors.status = 'Le statut est requis'; // Vérifie si le statut est vide
        return errors; // Retourne les erreurs
    };

    // 4- Soumission du formulaire
    const handleSubmit = async (e) => {
        e.preventDefault(); // Empêche le rechargement de la page
        setError(null); // Réinitialise les erreurs

        // Valider les champs du formulaire
        const errors = validateForm();
        setValidationErrors(errors); // Met à jour les erreurs de validation

        // Ne pas continuer s'il y a des erreurs de validation
        if (Object.keys(errors).length > 0) {
            return;
        }

        setLoadingSubmit(true); // Active l'état de chargement

        // Préparer les données à envoyer
        const payload = {
            marque: marque.trim(),
            type: type.trim(),
            matricule: matricule.trim(),
            status,
        };

        // Afficher une confirmation avant de modifier
        Swal.fire({
            title: 'Êtes-vous sûr de vouloir modifier cette voiture ?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Oui, modifier',
            cancelButtonText: 'Annuler',
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    // Envoyer une requête PUT pour mettre à jour la voiture
                    const response = await axios.put(`${BASE_URL}Voiture/${product.id}`, payload);
                    if (response.status === 204) { // 204 No Content est la réponse attendue
                        Swal.fire('Modifié!', 'La voiture a été modifiée avec succès.', 'success');
                        onSave(); // Notifier le composant parent
                        navigate('/products'); // Rediriger vers la page des produits
                    }
                } catch (err) {
                    // Gérer les erreurs
                    const message =
                        err.response?.data?.message ||
                        err.response?.data ||
                        'Erreur lors de la modification de la voiture';
                    setError(message); // Met à jour l'état d'erreur
                    Swal.fire('Erreur', message, 'error'); // Affiche une alerte d'erreur
                } finally {
                    setLoadingSubmit(false); // Désactive l'état de chargement
                }
            } else {
                setLoadingSubmit(false); // Désactive l'état de chargement si l'utilisateur annule
            }
        });
    };

    // 5- Rendu du formulaire
    return (
        <Container>
            <Typography variant="h4" gutterBottom>
                Modifier une Voiture
            </Typography>
            <form onSubmit={handleSubmit}>
                {}
                <Box mb={2}>
                    <TextField
                        label="Marque"
                        variant="outlined"
                        fullWidth
                        required
                        value={marque}
                        onChange={(e) => setMarque(e.target.value)}
                        error={!!validationErrors.marque}
                        helperText={validationErrors.marque}
                    />
                </Box>

                {}
                <Box mb={2}>
                    <TextField
                        label="Type"
                        variant="outlined"
                        fullWidth
                        required
                        value={type}
                        onChange={(e) => setType(e.target.value)}
                        error={!!validationErrors.type}
                        helperText={validationErrors.type}
                    />
                </Box>

                {}
                <Box mb={2}>
                    <TextField
                        label="Matricule"
                        variant="outlined"
                        fullWidth
                        required
                        value={matricule}
                        onChange={(e) => setMatricule(e.target.value)}
                        error={!!validationErrors.matricule}
                        helperText={validationErrors.matricule}
                    />
                </Box>

                {}
                <Box mb={2}>
                    <FormControl variant="outlined" fullWidth required error={!!validationErrors.status}>
                        <InputLabel>État</InputLabel>
                        <Select
                            value={status}
                            onChange={(e) => setStatus(e.target.value)}
                            label="Status"
                        >
                            <MenuItem value={1}>EnAttente</MenuItem>
                            <MenuItem value={2}>EnReparation</MenuItem>
                            <MenuItem value={3}>Reparee</MenuItem>
                        </Select>
                        {validationErrors.status && (
                            <Typography color="error" variant="caption">{validationErrors.status}</Typography>
                        )}
                    </FormControl>
                </Box>

                {}
                {error && (
                    <Typography color="error" gutterBottom>
                        {error}
                    </Typography>
                )}

                {}
                <Box mt={2} display="flex" justifyContent="space-between">
                    <Button variant="contained" color="primary" type="submit" disabled={loadingSubmit}>
                        {loadingSubmit ? 'Modification en cours...' : 'Modifier'}
                    </Button>
                    <Button
                        variant="outlined"
                        color="secondary"
                        onClick={onCancel}
                    >
                        Annuler
                    </Button>
                </Box>
            </form>
        </Container>
    );
};

export default EditProduct; // Exporte le composant
*/