// 1- Importation des modules nécessaires
import React, { useState, useEffect, useCallback } from "react"; // Hooks React
import axios from "axios"; // Pour les requêtes HTTP
import Swal from 'sweetalert2'; // Pour les alertes stylisées
import {
    Container,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    CircularProgress,
    Typography,
    TextField,
    Box,
    Pagination,
    Grid,
    IconButton,
} from "@mui/material"; // Composants Material-UI
import { styled } from "@mui/system"; // Pour les styles personnalisés
import { CheckCircle } from "@mui/icons-material"; // Icône de validation
import { BASE_URL } from "../../config"; // URL de base de l'API
import { jwtDecode } from 'jwt-decode'; // Pour décoder le token JWT

// 2- Styles personnalisés avec styled de Material-UI
const StyledTableContainer = styled(TableContainer)(({ theme }) => ({
    marginTop: theme.spacing(2),
    boxShadow: '0px 0px 5px rgba(0, 0, 0, 0.2)',
    width: '100%',
    overflowX: 'auto',
}));

const StyledTextField = styled(TextField)(({ theme }) => ({
    marginBottom: theme.spacing(2),
    width: '100%',
}));

const StyledTable = styled(Table)(({ theme }) => ({
    width: 'max-content',
    minWidth: '100%',
    tableLayout: 'auto',
}));

const StyledTableCell = styled(TableCell)(({ theme }) => ({
    whiteSpace: 'nowrap',
}));

// 3- Déclaration du composant CommandesList
const CommandesList = () => {
    // États pour gérer les données, le chargement, les erreurs, etc.
    const [commandes, setCommandes] = useState([]); // État pour stocker les commandes
    const [loading, setLoading] = useState(true); // État pour le chargement
    const [error, setError] = useState(null); // État pour les erreurs
    const [currentPage, setCurrentPage] = useState(1); // État pour la pagination
    const [itemsPerPage] = useState(5); // Nombre d'éléments par page
    const [searchTerm, setSearchTerm] = useState(""); // État pour la recherche
    const [updateLoading, setUpdateLoading] = useState(false); // État pour le chargement lors de la validation

    // États pour l'ID de l'utilisateur et ses rôles
    const [userId, setUserId] = useState(null); // État pour l'ID de l'utilisateur
    const [roles, setRoles] = useState([]); // État pour les rôles de l'utilisateur

    // 4- Fonction pour récupérer les rôles de l'utilisateur via l'API
    const fetchUserRoles = useCallback(async (userId) => {
        try {
            const response = await axios.get(`${BASE_URL}Query/user-roles/${userId}`, {
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
            });
            setRoles(response.data); // Stocker les rôles dans l'état
        } catch (err) {
            console.error('Erreur lors de la récupération des rôles:', err);
            setError('Échec de la récupération des rôles.');
        }
    }, [BASE_URL]);

    // 5- Décodage du token pour récupérer l'ID de l'utilisateur
    useEffect(() => {
        const token = localStorage.getItem('token'); // Récupérer le token depuis le localStorage
        if (token) {
            try {
                const decodedToken = jwtDecode(token); // Décoder le token
                const userId = decodedToken.nameid || decodedToken.sub; // Récupérer l'ID de l'utilisateur
                if (!userId) {
                    throw new Error('ID utilisateur non trouvé dans le token.');
                }
                setUserId(userId); // Stocker l'ID dans l'état
                fetchUserRoles(userId); // Récupérer les rôles de l'utilisateur
            } catch (error) {
                console.error('Erreur lors du décodage du token:', error);
                setError('Token invalide. Veuillez vous reconnecter.');
            }
        } else {
            console.error('Aucun token trouvé dans localStorage');
            setError('Vous devez vous reconnecter.');
        }
    }, [fetchUserRoles]);

    // 6- Fonction pour récupérer les commandes depuis l'API
    const fetchCommandes = useCallback(async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${BASE_URL}Query/commandes`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                },
            });
            console.log("Données reçues de l'API :", response.data);
            setCommandes(response.data); // Mettre à jour l'état avec les données reçues
        } catch (err) {
            console.error("Erreur lors de la récupération des données :", err);
            setError(err.message || "Erreur lors de la récupération des données");
        } finally {
            setLoading(false); // Désactiver le chargement
        }
    }, [BASE_URL]);

    // 7- Utilisation de useEffect pour charger les données au montage du composant
    useEffect(() => {
        if (userId) {
            fetchCommandes();
        }
    }, [userId, fetchCommandes]);

    // 8- Gestion de la recherche
    const handleSearch = (event) => {
        setSearchTerm(event.target.value); // Mettre à jour le terme de recherche
    };

    // 9- Fonction pour valider une commande
    const handleValidate = async (commande) => {
        setUpdateLoading(commande.id); // Activer le chargement pour cette commande
        try {
            console.log("Validation de la commande :", commande);

            // Requête PATCH pour mettre à jour l'état de la commande
            const response = await axios.patch(
                `${BASE_URL}Command/${commande.id}`,
                { etatCommande: "Validée" }, // Corps de la requête avec le nouvel état
                {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json', // Important pour envoyer des données JSON
                    },
                }
            );

            console.log("Réponse du serveur :", response.data);

            // Mettre à jour l'état des commandes
            setCommandes((prevCommandes) =>
                prevCommandes.map((c) =>
                    c.id === commande.id ? { ...c, etatCommande: "Validée" } : c
                )
            );

            Swal.fire('Succès', 'La commande a été validée avec succès !', 'success');
        } catch (err) {
            console.error("Erreur lors de la validation de la commande :", err);

            if (err.response) {
                console.error("Réponse du serveur :", err.response.data);
                Swal.fire(
                    'Erreur',
                    `Erreur du serveur : ${err.response.data.message ||
                    "Requête invalide"}`,
                    'error'
                );
            } else {
                Swal.fire(
                    'Erreur',
                    'Une erreur est survenue lors de la validation de la commande.',
                    'error'
                );
            }
        } finally {
            setUpdateLoading(false); // Désactiver le chargement
        }
    };

    // 10- Filtrer les commandes en fonction du terme de recherche
    const filteredCommandes = commandes.filter((commande) => {
        const searchLower = searchTerm.toLowerCase();
        return commande.composentProductName?.toLowerCase().includes(searchLower);
    });

    // 11- Pagination des commandes filtrées
    const paginatedCommandes = filteredCommandes.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    // 12- Gestion du changement de page
    const handlePageChange = (event, value) => {
        setCurrentPage(value); // Mettre à jour la page courante
    };

    // 13- Affichage du chargement
    if (loading) return (
        <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
            <CircularProgress />
        </Box>
    );

    // 14- Affichage des erreurs
    if (error) return <Typography color="error">{error}</Typography>;

    // 15- Rendu du composant
    return (
        <Container>
            <Typography variant="h4" gutterBottom>
                Liste des Commandes
            </Typography>

            {/* Afficher l'ID de l'utilisateur et ses rôles */}
            <Typography variant="h6" gutterBottom>
                ID Utilisateur: {userId || 'Non disponible'}
            </Typography>
            <Typography variant="h6" gutterBottom>
                Rôles: {roles.join(', ') || 'Aucun rôle'}
            </Typography>

            {/* Barre de recherche */}
            <Grid container spacing={2} alignItems="center">
                <Grid item xs={12}>
                    <StyledTextField
                        variant="outlined"
                        placeholder="Rechercher par nom du composant..."
                        fullWidth
                        value={searchTerm}
                        onChange={handleSearch}
                    />
                </Grid>
            </Grid>

            {/* Affichage des commandes */}
            {paginatedCommandes.length === 0 ? (
                <Typography variant="h6" color="textSecondary">
                    Aucune commande trouvée !
                </Typography>
            ) : (
                <>
                    <StyledTableContainer component={Paper}>
                        <StyledTable>
                            <TableHead>
                                <TableRow>
                                    <StyledTableCell>ID</StyledTableCell>
                                    <StyledTableCell>État</StyledTableCell>
                                    <StyledTableCell>Date Commande</StyledTableCell>
                                    <StyledTableCell>Nom Expert</StyledTableCell>
                                    <StyledTableCell>Nom Composant</StyledTableCell>
                                    <StyledTableCell>SN Composant</StyledTableCell>
                                    <StyledTableCell>Urgent Composant</StyledTableCell>
                                    <StyledTableCell>Order Composant</StyledTableCell>
                                    <StyledTableCell>Raison</StyledTableCell>
                                    <StyledTableCell>Code Site FM1</StyledTableCell>
                                    <StyledTableCell>Device Type FM1</StyledTableCell>
                                    <StyledTableCell>PS SN FM1</StyledTableCell>
                                    {/* Masquer la colonne Action si l'utilisateur est un Expert */}
                                    {!roles.includes("Expert") && (
                                        <StyledTableCell>Action</StyledTableCell>
                                    )}
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {paginatedCommandes.map((commande) => (
                                    <TableRow key={commande.id}>
                                        <StyledTableCell>{commande.id}</StyledTableCell>
                                        <StyledTableCell>
                                            {commande.etatCommande}
                                        </StyledTableCell>
                                        <StyledTableCell>
                                            {new Date(commande.dateCmd).toLocaleDateString()}
                                        </StyledTableCell>
                                        <StyledTableCell>{commande.expertNom}</StyledTableCell>
                                        <StyledTableCell>{commande.composentProductName}</StyledTableCell>
                                        <StyledTableCell>{commande.composentSN}</StyledTableCell>
                                        <StyledTableCell>{commande.composentUrgentOrNot}</StyledTableCell>
                                        <StyledTableCell>{commande.composentOrderOrNot}</StyledTableCell>
                                        <StyledTableCell>{commande.raisonDeCommande}</StyledTableCell>
                                        <StyledTableCell>{commande.fM1CodeSite}</StyledTableCell>
                                        <StyledTableCell>{commande.fM1DeviceType}</StyledTableCell>
                                        <StyledTableCell>{commande.fM1PsSn}</StyledTableCell>
                                        {/* Masquer la colonne Action si l'utilisateur est un Expert */}
                                        {roles.includes("Magasinier") && (
                                            <StyledTableCell>
                                                {updateLoading === commande.id ? (
                                                    <CircularProgress size={20} />
                                                ) : (
                                                    commande.etatCommande !== "Validée" ? (
                                                        <IconButton
                                                            onClick={() => handleValidate(commande)}
                                                            disabled={updateLoading === commande.id}
                                                        >
                                                            <CheckCircle color="success" />
                                                        </IconButton>
                                                    ) : (
                                                        <Typography variant="body2" color="green">Validée</Typography>
                                                    )
                                                )}
                                            </StyledTableCell>
                                        )}
                                    </TableRow>
                                ))}
                            </TableBody>
                        </StyledTable>
                    </StyledTableContainer>
                    <Box mt={2} display="flex" justifyContent="center">
                        <Pagination
                            count={Math.ceil(filteredCommandes.length / itemsPerPage)}
                            page={currentPage}
                            onChange={handlePageChange}
                            color="primary"
                        />
                    </Box>
                </>
            )}
        </Container>
    );
};

export default CommandesList;