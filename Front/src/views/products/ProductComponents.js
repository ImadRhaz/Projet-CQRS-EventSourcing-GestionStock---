import React, { useState, useEffect, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import Swal from 'sweetalert2';
import {
    Container, Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    Paper, Button, Typography, Box, CircularProgress, Pagination,
    Dialog, DialogTitle, DialogContent, DialogActions, TextField, Select,
    MenuItem, FormControl, InputLabel, Autocomplete
} from '@mui/material';
import { BASE_URL } from '../../config';
import { jwtDecode } from 'jwt-decode';

const ProductComponents = () => {
    const { id } = useParams();
    const [composents, setComposents] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 7;

    const [newComposent, setNewComposent] = useState({
        productName: '',
        sn: '',
        urgentOrNot: 'No',
        orderOrNot: 'No',
        fM1Id: id,
    });

    const [isCommandeDialogOpen, setIsCommandeDialogOpen] = useState(false);
    const [selectedComposent, setSelectedComposent] = useState(null);
    const [raisonDeCommande, setRaisonDeCommande] = useState('');
    const [userId, setUserId] = useState(null);
    const [refresh, setRefresh] = useState(false);
    const [composentOptions, setComposentOptions] = useState([]);

    // Fetch composent options from ExcelComposents
    useEffect(() => {
        const fetchComposentOptions = async () => {
            try {
                const response = await axios.get(`${BASE_URL}ExcelFm1/get-all-composent`);
                setComposentOptions(response.data);
            } catch (error) {
                console.error('Error fetching composent options:', error);
                setError('Failed to load composent options');
            }
        };
        fetchComposentOptions();
    }, []);

    // Fetch FM1 components
    const fetchComposents = useCallback(async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${BASE_URL}Query/composents/by-fm1/${id}`, { // Corrected Endpoint
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
            });
            setComposents(response.data);

            // Log the fetched data to the console
            console.log("Fetched Composents:", response.data);
        } catch (error) {
            console.error("Error fetching components:", error);
            setError("Failed to load components");
        } finally {
            setLoading(false);
        }
    }, [id]);

    useEffect(() => {
        fetchComposents();
    }, [fetchComposents, refresh]);

    // Get user ID from token
    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token) {
            try {
                const decoded = jwtDecode(token);
                setUserId(decoded.nameid || decoded.sub);
            } catch (error) {
                console.error('Token decode error:', error);
                setError('Invalid token');
            }
        }
    }, []);

    const handleAddComposent = async () => {
        if (!newComposent.productName) {
            Swal.fire('Error', 'Please select a component', 'error');
            return;
        }

        try {
            await axios.post(`${BASE_URL}Command/add-composent`, {
                ProductName: newComposent.productName,
                SN: newComposent.sn || null,
                UrgentOrNot: newComposent.urgentOrNot,
                OrderOrNot: newComposent.orderOrNot,
                FM1Id: id
            }, {
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
            });

            Swal.fire('Success', 'Component added!', 'success');
            setRefresh(prev => !prev);
            setNewComposent({
                productName: '',
                sn: '',
                urgentOrNot: 'No',
                orderOrNot: 'No',
                fM1Id: id,
            });
        } catch (error) {
            console.error("Add component error:", error);
            Swal.fire('Error', 'Failed to add component', 'error');
        }
    };

    const handleCommande = async () => {
        if (!selectedComposent || !raisonDeCommande) {
            Swal.fire('Error', 'Please provide a reason', 'error');
            return;
        }

        try {
            await axios.post(`${BASE_URL}Command/add-commande`, {
                etatCommande: "En attente",
                dateCmd: new Date().toISOString(),
                composentId: selectedComposent.id,
                expertId: userId,
                raisonDeCommande,
                fM1Id: id,
            }, {
                headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
            });

            Swal.fire('Success', 'Order submitted!', 'success');
            setIsCommandeDialogOpen(false);
            setRaisonDeCommande('');
            setRefresh(prev => !prev);
        } catch (error) {
            console.error("Order error:", error);
            Swal.fire('Error', 'Failed to submit order', 'error');
        }
    };

    const handlePageChange = (event, value) => setCurrentPage(value);

    if (loading) return <CircularProgress />;
    if (error) return <Typography color="error">{error}</Typography>;

    return (
        <Container>
            <Typography variant="h4" gutterBottom>Components Management</Typography>

            {/* Add Component Section */}
            <Box mb={4}>
                <Typography variant="h6">Add Component</Typography>
                <Autocomplete
                    options={composentOptions}
                    getOptionLabel={(option) => `${option.composentName} (${option.anComposent})` || ''}
                    renderInput={(params) => (
                        <TextField {...params} label="Component" required />
                    )}
                    onChange={(e, newValue) => setNewComposent({
                        ...newComposent,
                        productName: newValue?.composentName || '',
                        sn: newValue?.snComposent || ''
                    })}
                    fullWidth
                />
                
                <TextField
                    label="SN (Optional)"
                    value={newComposent.sn}
                    onChange={(e) => setNewComposent({...newComposent, sn: e.target.value})}
                    fullWidth
                    margin="normal"
                />

                <FormControl fullWidth margin="normal">
                    <InputLabel>Urgent?</InputLabel>
                    <Select
                        value={newComposent.urgentOrNot}
                        onChange={(e) => setNewComposent({...newComposent, urgentOrNot: e.target.value})}
                    >
                        <MenuItem value="Yes">Yes</MenuItem>
                        <MenuItem value="No">No</MenuItem>
                    </Select>
                </FormControl>

                <Button
                    variant="contained"
                    onClick={handleAddComposent}
                    sx={{ mt: 2 }}
                >
                    Add Component
                </Button>
            </Box>

            {/* Components Table */}
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Name</TableCell>
                            <TableCell>Current SN</TableCell>
                            <TableCell>Validated SN</TableCell>
                            <TableCell>Urgent</TableCell>
                            <TableCell>Order Needed</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {composents.slice(
                            (currentPage-1)*itemsPerPage, 
                            currentPage*itemsPerPage
                        ).map(composent => (
                            <TableRow key={composent.id}>
                                <TableCell>{composent.productName}</TableCell>
                                <TableCell>{composent.sn || '-'}</TableCell>
                                <TableCell>
                                    {composent.etatCommande === "Validée" ? (
                                        composent.snDuComposentValidé ? composent.snDuComposentValidé : "No SN available"
                                    ) : "Not validated"}
                                </TableCell>

                                <TableCell>{composent.urgentOrNot}</TableCell>
                                <TableCell>{composent.orderOrNot}</TableCell>
                                <TableCell>
                                    <Box 
                                        sx={{ 
                                            color: composent.etatCommande === "Validée" 
                                                ? 'success.main' 
                                                : composent.etatCommande === "En attente" 
                                                    ? 'warning.main' 
                                                    : 'text.primary'
                                        }}
                                    >
                                        {composent.etatCommande || 'N/A'}
                                    </Box>
                                </TableCell>
                                <TableCell>
                                    <Button
                                        variant="outlined"
                                        onClick={() => {
                                            setSelectedComposent(composent);
                                            setIsCommandeDialogOpen(true);
                                        }}
                                        disabled={composent.etatCommande === "Validée" || composent.etatCommande === "En attente"}
                                        color={
                                            composent.etatCommande === "Validée"
                                                ? "success"
                                                : composent.etatCommande === "En attente"
                                                    ? "warning"
                                                    : "primary"
                                        }
                                    >
                                        {composent.etatCommande === "Validée"
                                            ? "Validated"
                                            : composent.etatCommande === "En attente"
                                                ? "Pending"
                                                : "Order"}
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>

            {/* Pagination */}
            <Pagination
                count={Math.ceil(composents.length / itemsPerPage)}
                page={currentPage}
                onChange={handlePageChange}
                sx={{ mt: 2, justifyContent: 'center' }}
            />

            {/* Order Dialog */}
            <Dialog open={isCommandeDialogOpen} onClose={() => setIsCommandeDialogOpen(false)}>
                <DialogTitle>
                    {selectedComposent?.productName}
                    {selectedComposent?.sn && ` (Current SN: ${selectedComposent.sn})`}
                    {selectedComposent?.etatCommande === "Validée" && 
                     ` | Validated SN: ${selectedComposent.snDuComposentValidé || 'No SN available'}`}
                </DialogTitle>
                <DialogContent>
                    <TextField
                        label="Order Reason"
                        multiline
                        rows={4}
                        fullWidth
                        value={raisonDeCommande}
                        onChange={(e) => setRaisonDeCommande(e.target.value)}
                        sx={{ mt: 2 }}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setIsCommandeDialogOpen(false)}>Cancel</Button>
                    <Button 
                        onClick={handleCommande}
                        variant="contained"
                        color="primary"
                        disabled={selectedComposent?.etatCommande === "Validée"}
                    >
                        {selectedComposent?.etatCommande === "Validée" ? "Already Validated" : "Confirm Order"}
                    </Button>
                </DialogActions>
            </Dialog>
        </Container>
    );
};

export default ProductComponents;