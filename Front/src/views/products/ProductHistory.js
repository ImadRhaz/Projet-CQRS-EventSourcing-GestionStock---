import React, { useState, useEffect } from 'react';
import {
    Container, Table, TableBody, TableCell, TableContainer, TableHead,
    TableRow, Paper, Button, Typography, Box, CircularProgress, TextField,
    Pagination
} from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import jsPDF from 'jspdf';
import 'jspdf-autotable';
import axios from 'axios';
import { BASE_URL } from '../../config';

const ProductHistory = () => {
    const { id } = useParams();
    const [fm1History, setFm1History] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [page, setPage] = useState(1);
    const [rowsPerPage] = useState(5); // ou la valeur que vous souhaitez
    const navigate = useNavigate();

    useEffect(() => {
        const fetchFM1History = async () => {
            setLoading(true);
            setError(null);
            try {
                const response = await axios.get(`${BASE_URL}Query/fm1histories/${id}`);
                setFm1History(response.data);
            } catch (err) {
                console.error("Erreur API:", err);
                if (err.response && err.response.status === 404) {
                    setError("Aucun historique trouvé pour ce produit.");
                } else {
                    setError("Erreur lors du chargement de l'historique.");
                }
            } finally {
                setLoading(false);
            }
        };

        fetchFM1History();
    }, [id]);

    const handleSearchChange = (event) => {
        setSearchTerm(event.target.value);
        setPage(1); // Réinitialiser à la page 1 après une recherche
    };

    const handleChangePage = (event, newPage) => {
        setPage(newPage);
    };

    const downloadPDF = () => {
        const doc = new jsPDF();
        const headers = [["Code Site", "Device Type", "P/S S/N", "Piece Name", "Piece S/N", "Urgent?", "Order Status", "Commande Status", "Commande Date", "Commande Reason", "Expert Name"]];
        const data = fm1History.commandes.map(commande => [
            fm1History.fM1CodeSite,
            fm1History.fM1DeviceType,
            fm1History.fM1PsSn,
            commande.composantProductName,
            commande.composantSN,
            commande.composantUrgentOrNot,
            commande.composantOrderOrNot,
            commande.etatCommande,
            new Date(commande.dateCmd).toLocaleDateString(),
            commande.raisonDeCommande,
            `${commande.expertNom} ${commande.expertPrenom}`
        ]);
        doc.autoTable({ head: headers, body: data });
        doc.save('fm1history.pdf');
    };

    if (loading) {
        return <Box display="flex" justifyContent="center" alignItems="center" height="100vh"><CircularProgress /></Box>;
    }

    if (error) {
        return <Typography color="error">{error}</Typography>;
    }

    if (!fm1History || !fm1History.commandes) {
        return <Typography>Aucune donnée disponible.</Typography>;
    }

    const filteredCommandes = fm1History.commandes.filter(commande => {
        const searchRegex = new RegExp(searchTerm, 'i');
        return (
            searchRegex.test(commande.composantProductName) ||
            searchRegex.test(commande.composantSN) ||
            searchRegex.test(commande.composantUrgentOrNot) ||
            searchRegex.test(commande.composantOrderOrNot) ||
            searchRegex.test(commande.etatCommande) ||
            searchRegex.test(commande.raisonDeCommande) ||
            searchRegex.test(commande.expertNom) ||
            searchRegex.test(commande.expertPrenom)
        );
    });

    const startIndex = (page - 1) * rowsPerPage;
    const endIndex = startIndex + rowsPerPage;
    const paginatedCommandes = filteredCommandes.slice(startIndex, endIndex);

    return (
        <Container>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h4" gutterBottom>Product History</Typography>
            </Box>
            <Box mb={2}>
                <Button variant="outlined" color="primary" onClick={() => navigate('/fm1s')}><ArrowBackIcon /></Button>
            </Box>
            <Box mb={3}>
                <TextField label="Rechercher" variant="outlined" fullWidth value={searchTerm} onChange={handleSearchChange} />
            </Box>
            <Box mb={2}>
                <Button variant="contained" color="primary" onClick={downloadPDF}>Télécharger l'Historique</Button>
            </Box>

            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Code Site</TableCell>
                            {/* ... autres en-têtes ... */}
                            <TableCell>Expert Name</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {paginatedCommandes.map(commande => (
                            <TableRow key={commande.id}>
                                <TableCell>{fm1History.fM1CodeSite}</TableCell>
                                <TableCell>{fm1History.fM1DeviceType}</TableCell>
                                <TableCell>{fm1History.fM1PsSn}</TableCell>
                                <TableCell>{commande.composantProductName}</TableCell>
                                <TableCell>{commande.composantSN}</TableCell>
                                <TableCell>{commande.composantUrgentOrNot}</TableCell>
                                <TableCell>{commande.composantOrderOrNot}</TableCell>
                                <TableCell>{commande.etatCommande}</TableCell>
                                <TableCell>{new Date(commande.dateCmd).toLocaleDateString()}</TableCell>
                                <TableCell>{commande.raisonDeCommande}</TableCell>
                                <TableCell>{commande.expertNom} {commande.expertPrenom}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            <Box mt={2} display="flex" justifyContent="center">
                <Pagination
                    count={Math.ceil(filteredCommandes.length / rowsPerPage)}
                    page={page}
                    onChange={handleChangePage}
                    color="primary"
                    showFirstButton
                    showLastButton
                />
            </Box>
        </Container>
    );
};

export default ProductHistory;