// 1- Importation des modules nécessaires
import React, { useState, useEffect } from "react"; // Hooks React
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

const StyledTable = styled(Table)(({ theme}) => ({
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

    // 4- Décodage du token pour récupérer l'ID de l'utilisateur et les rôles
    useEffect(() => {
        const token = localStorage.getItem('token'); // Récupérer le token depuis le localStorage
        if (token) {
            try {
                const decodedToken = jwtDecode(token); // Décoder le token
                const userId = decodedToken.sub; // Récupérer l'ID de l'utilisateur (sub)
                const roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']; // Récupérer les rôles
                console.log('Decoded roles:', roles);
                console.log('ID récupéré du token:', userId);
                setUserId(userId); // Stocker l'ID dans l'état
                setRoles(Array.isArray(roles) ? roles : [roles]); // Stocker les rôles dans l'état (convertir en tableau si ce n'est pas déjà le cas)

            } catch (error) {
                console.error('Erreur lors du décodage du token:', error);
                setError('Token invalide. Veuillez vous reconnecter.');
            }
        } else {
            console.error('Aucun token trouvé dans localStorage');
            setError('Vous devez vous reconnecter.');
        }
    }, []);

    // 5- Fonction pour récupérer les commandes depuis l'API
    const fetchCommandes = async () => {
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
    };

    // 6- Utilisation de useEffect pour charger les données au montage du composant
    useEffect(() => {
        fetchCommandes();
    }, []);

    // 7- Gestion de la recherche
    const handleSearch = (event) => {
        setSearchTerm(event.target.value); // Mettre à jour le terme de recherche
    };

    // 8- Fonction pour valider une commande
    const handleValidate = async (commande) => {
        setUpdateLoading(commande.id); // Activer le chargement pour cette commande
        try {
            console.log("Validation de la commande :", commande);

            // Make the PATCH request to update the EtatCommande
            const response = await axios.patch(
                `${BASE_URL}Command/${commande.id}`,
                { etatCommande: "Validée" }, // Request body with the new state
                {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json', // Important for sending JSON data
                    },
                }
            );

            console.log("Réponse du serveur :", response.data);

            // Update the state of the commandes
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

    // 9- Filtrer les commandes en fonction du terme de recherche
    const filteredCommandes = commandes.filter((commande) => {
        const searchLower = searchTerm.toLowerCase();
        return commande.composentProductName?.toLowerCase().includes(searchLower);
    });

    // 10- Pagination des commandes filtrées
    const paginatedCommandes = filteredCommandes.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    // 11- Gestion du changement de page
    const handlePageChange = (event, value) => {
        setCurrentPage(value); // Mettre à jour la page courante
    };

    // 12- Affichage du chargement
    if (loading) return (
        <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
            <CircularProgress />
        </Box>
    );

    // 13- Affichage des erreurs
    if (error) return <Typography color="error">{error}</Typography>;

    // 14- Rendu du composant
    return (
        <Container>
            <Typography variant="h4" gutterBottom>
                Liste des Commandes
            </Typography>

            {/* Afficher l'ID de l'utilisateur et ses rôles */}
            <Box mb={2}>
                <Typography variant="h6">
                    ID de l'Utilisateur Connecté: {userId || 'Non disponible'}
                </Typography>
                <Typography variant="h6">
                    Rôles: {roles.join(', ') || 'Aucun rôle'}
                </Typography>
            </Box>

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
                                    {/*<StyledTableCell>ID FM1 History</StyledTableCell>*/} {/* Hide ID FM1 History Column */}
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
                                        {/*<StyledTableCell>{commande.fM1HistoryId}</StyledTableCell>*/} {/* Hide ID FM1 History Data */}
                                        {/* Masquer la colonne Action si l'utilisateur est un Expert */}
                                        {!roles.includes("Expert") && (
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