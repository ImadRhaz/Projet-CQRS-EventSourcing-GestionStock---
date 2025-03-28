import React, { useState, useEffect, useCallback } from "react";
import axios from "axios";
import Swal from 'sweetalert2';
import {
    Container, Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    Paper, CircularProgress, Typography, TextField, Box, Pagination, Grid, IconButton
} from "@mui/material";
import { styled } from '@mui/system';
import { CheckCircle } from "@mui/icons-material";
import { BASE_URL } from "../../config";
import { jwtDecode } from 'jwt-decode';

const StyledTableCell = styled(TableCell)(({ theme }) => ({
    flexGrow: 1,
    textAlign: 'left',
}));

const StyledTableCellLarge = styled(TableCell)(({ theme }) => ({
    flexGrow: 2,
    textAlign: 'left',
}));

const StyledTableHeadCell = styled(TableCell)(({ theme }) => ({
    flexGrow: 1,
    textAlign: 'left',
    fontWeight: 'bold',
}));

const StyledTableHeadCellLarge = styled(TableCell)(({ theme }) => ({
    flexGrow: 2,
    textAlign: 'left',
    fontWeight: 'bold',
}));

const CommandesList = () => {
    const [commandes, setCommandes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(5);
    const [searchTerm, setSearchTerm] = useState("");
    const [updateLoading, setUpdateLoading] = useState(false);
    const [userId, setUserId] = useState(null);
    const [roles, setRoles] = useState([]);

    const formatErrorMessage = (error) => {
        if (typeof error === 'string') return error;
        if (error.message) return error.message;
        if (error.response?.data) {
            return typeof error.response.data === 'object'
                ? JSON.stringify(error.response.data)
                : error.response.data;
        }
        return "Erreur inconnue";
    };

    const fetchUserRoles = useCallback(async (userId) => {
        try {
            const response = await axios.get(`${BASE_URL}Query/user-roles/${userId}`, {
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
            });
            setRoles(response.data);
        } catch (err) {
            console.error('Erreur rôles:', err);
            setError(formatErrorMessage(err));
        }
    }, [BASE_URL]);

    const fetchCommandes = useCallback(async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${BASE_URL}Query/commandes`, {
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
            });
            setCommandes(response.data);
        } catch (err) {
            console.error("Erreur données:", err);
            setError(formatErrorMessage(err));
        } finally {
            setLoading(false);
        }
    }, [BASE_URL]);

    const refreshData = useCallback(async () => {
        await fetchCommandes();
    }, [fetchCommandes]);

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded = jwtDecode(token);
                const userId = decoded.nameid || decoded.sub;
                if (!userId) throw new Error('ID utilisateur non trouvé');
                setUserId(userId);
                fetchUserRoles(userId);
            } catch (error) {
                console.error('Erreur token:', error);
                setError('Session invalide. Veuillez vous reconnecter.');
            }
        } else {
            setError('Authentification requise.');
        }
    }, [fetchUserRoles]);

    useEffect(() => {
        if (userId) refreshData();
    }, [userId, refreshData]);

    const handleValidate = async (commande) => {
        setUpdateLoading(commande.id);
        try {
            const apiUrl = `${BASE_URL}Command/${commande.id}`;
            const response = await axios.patch(
                apiUrl,
                { EtatCommande: "Validée" },
                {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json',
                    },
                }
            );

            if (response.status === 204) {
                await refreshData();
                Swal.fire({
                    title: 'Succès',
                    text: 'Commande validée et stock mis à jour!',
                    icon: 'success',
                    timer: 2000
                });
            }
        } catch (err) {
            console.error("Détails de l'erreur:", {
                message: err.message,
                response: err.response,
            });

            let errorMessage = "Erreur lors de la validation";

            if (err.response && err.response.status === 400) {
                errorMessage = err.response.data || "Problème avec le stock";

                if (typeof err.response.data === 'string') {
                    if (err.response.data.includes("stock") || err.response.data.includes("Stock")) {
                        errorMessage = err.response.data;
                    }
                }
            }

            Swal.fire({
                title: 'Erreur',
                text: errorMessage,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        } finally {
            setUpdateLoading(false);
        }
    };

    const handleSearch = (e) => setSearchTerm(e.target.value.toLowerCase());
    const handlePageChange = (e, value) => setCurrentPage(value);

    const filteredCommandes = commandes.filter(cmd =>
        cmd.composentProductName?.toLowerCase().includes(searchTerm)
    );

    const paginatedCommandes = filteredCommandes.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    if (loading) return (
        <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
            <CircularProgress />
        </Box>
    );

    if (error) return (
        <Container>
            <Typography color="error" variant="h6">{error}</Typography>
        </Container>
    );

    return (
        <Container>
            <Typography variant="h4" gutterBottom>Liste des Commandes</Typography>

            <Grid container spacing={2}>
                <Grid item xs={12}>
                    <TextField
                        label="Rechercher par composant"
                        variant="outlined"
                        fullWidth
                        onChange={handleSearch}
                    />
                </Grid>
            </Grid>

            {paginatedCommandes.length === 0 ? (
                <Typography variant="h6" color="textSecondary" mt={2}>
                    {searchTerm ? "Aucun résultat trouvé" : "Aucune commande disponible"}
                </Typography>
            ) : (
                <>
                    <TableContainer component={Paper} sx={{ width: '100%', mt: 2, overflowX: 'auto' }}>
                        <Table sx={{ minWidth: 1200 }}>
                            <TableHead>
                                <TableRow>
                                    <StyledTableHeadCell>ID</StyledTableHeadCell>
                                    <StyledTableHeadCell>État</StyledTableHeadCell>
                                    <StyledTableHeadCell>Date Commande</StyledTableHeadCell>
                                    <StyledTableHeadCell>Nom Expert</StyledTableHeadCell>
                                    <StyledTableHeadCellLarge>Nom Composant</StyledTableHeadCellLarge>
                                    <StyledTableHeadCell>SN Composant</StyledTableHeadCell>
                                    <StyledTableHeadCell>Urgent Composant</StyledTableHeadCell>
                                    <StyledTableHeadCell>Order Composant</StyledTableHeadCell>
                                    <StyledTableHeadCell>Raison</StyledTableHeadCell>
                                    <StyledTableHeadCellLarge>Code Site FM1</StyledTableHeadCellLarge>
                                    <StyledTableHeadCell>Device Type FM1</StyledTableHeadCell>
                                    <StyledTableHeadCell>PS SN FM1</StyledTableHeadCell>
                                    {(roles.includes("Magasinier") || roles.includes("Admin")) && (
                                        <StyledTableHeadCell>Action</StyledTableHeadCell>
                                    )}
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {paginatedCommandes.map(cmd => (
                                    <TableRow key={cmd.id}>
                                        <StyledTableCell>{cmd.id}</StyledTableCell>
                                        <StyledTableCell>{cmd.etatCommande}</StyledTableCell>
                                        <StyledTableCell>{new Date(cmd.dateCmd).toLocaleDateString()}</StyledTableCell>
                                        <StyledTableCell>{cmd.expertNom} {cmd.expertPrenom}</StyledTableCell>
                                        <StyledTableCellLarge>{cmd.composentProductName}</StyledTableCellLarge>
                                        <StyledTableCell>{cmd.composentSN || '-'}</StyledTableCell>
                                        <StyledTableCell>{cmd.composentUrgentOrNot}</StyledTableCell>
                                        <StyledTableCell>{cmd.composentOrderOrNot}</StyledTableCell>
                                        <StyledTableCell>{cmd.raisonDeCommande}</StyledTableCell>
                                        <StyledTableCellLarge>{cmd.fM1CodeSite}</StyledTableCellLarge>
                                        <StyledTableCell>{cmd.fM1DeviceType}</StyledTableCell>
                                        <StyledTableCell>{cmd.fM1PsSn}</StyledTableCell>
                                        {(roles.includes("Magasinier") || roles.includes("Admin")) && (
                                            <StyledTableCell>
                                                {cmd.etatCommande === "Validée" ? (
                                                    <Typography color="success.main">Validée</Typography>
                                                ) : updateLoading === cmd.id ? (
                                                    <CircularProgress size={24} />
                                                ) : (
                                                    <IconButton
                                                        onClick={() => handleValidate(cmd)}
                                                        color="success"
                                                    >
                                                        <CheckCircle />
                                                    </IconButton>
                                                )}
                                            </StyledTableCell>
                                        )}
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                    <Box display="flex" justifyContent="center" mt={2}>
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