import React, { useState, useEffect, useCallback } from 'react';
import {
    Container,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Button,
    Typography,
    Box,
    CircularProgress,
    Pagination,
    Tooltip,
    IconButton,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import HistoryIcon from '@mui/icons-material/History';
import ViewListIcon from '@mui/icons-material/ViewList'; // Added
import Swal from 'sweetalert2';
import { useNavigate } from 'react-router-dom';
import { BASE_URL } from '../../config';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const Products = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 5;
    const navigate = useNavigate();
    const [userId, setUserId] = useState(null);
    const [roles, setRoles] = useState([]);
    const [isEditing, setIsEditing] = useState(false);
    const [currentProduct, setCurrentProduct] = useState(null);

    const handlePageChange = useCallback((event, value) => {
        setCurrentPage(value);
    }, []);

    const fetchUserRoles = useCallback(async (userId) => {
        try {
            const response = await axios.get(`${BASE_URL}Query/user-roles/${userId}`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                },
            });
            setRoles(response.data);
        } catch (err) {
            console.error('Error fetching user roles:', err);
            setError('Failed to fetch user roles.');
        }
    }, [BASE_URL]);

    const handleDelete = useCallback(async (productId) => {
        try {
            await axios.delete(`${BASE_URL}Voiture/${productId}`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                },
            });

            setProducts((prevProducts) => prevProducts.filter((product) => product.id !== productId));
            Swal.fire('Deleted!', 'The car has been deleted.', 'success');
        } catch (err) {
            console.error('Error deleting product:', err);
            Swal.fire('Error', `Failed to delete the car: ${err.message}`, 'error');
        }
    }, [BASE_URL]);

    const confirmDelete = useCallback((productId) => {
        Swal.fire({
            title: 'Are you sure you want to delete this car?',
            text: 'This action is irreversible!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel',
        }).then((result) => {
            if (result.isConfirmed) {
                handleDelete(productId);
            }
        });
    }, [handleDelete]);

    const handleEdit = useCallback((product) => {
        setCurrentProduct(product);
        setIsEditing(true);
    }, []);

    const handleShowHistory = useCallback((productId) => {
        navigate(`/product-history/${productId}`);
    }, [navigate]);

    const handleShowComponents = useCallback((productId) => {
        navigate(`/product-components/${productId}`);
    }, [navigate]);

    const fetchProducts = useCallback(async () => {
        setLoading(true);
        try {
            const response = await axios.get(`${BASE_URL}Query/fm1s`, {
                headers: {
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                },
            });
            setProducts(response.data || []);
            setError(null);
        } catch (err) {
            console.error('Error fetching products:', err);
            setError('Failed to retrieve data!');
        } finally {
            setLoading(false);
        }
    }, [BASE_URL]);

    useEffect(() => {
        const token = localStorage.getItem('token');

        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                const userId = decodedToken.nameid || decodedToken.sub;

                if (!userId) {
                    throw new Error('User ID not found in the token.');
                }

                setUserId(userId);
                fetchUserRoles(userId);
            } catch (error) {
                console.error('Error decoding token:', error);
                setError('Invalid token. Please log in again.');
            }
        } else {
            console.error('No token found in localStorage');
            setError('You must log in again.');
        }
    }, [fetchUserRoles]);

    useEffect(() => {
        if (userId) {
            fetchProducts();
        }
    }, [userId, fetchProducts]);

    const getPaginatedProducts = () => {
        const startIndex = (currentPage - 1) * itemsPerPage;
        const endIndex = startIndex + itemsPerPage;
        return products.slice(startIndex, endIndex);
    };

    const getStatusText = (status) => {
        switch (status) {
            case 'ccs':
                return 'En Attente';
            case 'Reparation':
                return 'En Réparation';
            case 'Reparer':
                return 'Réparé';
            default:
                return 'Inconnu';
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="200px">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Container>
                <Typography color="error">{error}</Typography>
            </Container>
        );
    }

    const paginatedProducts = getPaginatedProducts();

    return (
        <Container>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h4" gutterBottom>
                    FM1 List
                </Typography>
                <Typography variant="h6" gutterBottom>
                    Logged in User ID: {userId || 'Not available'}
                </Typography>
                <Typography variant="h6" gutterBottom>
                    Roles: {roles.join(', ') || 'No role'}
                </Typography>
            </Box>

            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Code Site</TableCell>
                            <TableCell>Device Type</TableCell>
                            <TableCell>PS SN</TableCell>
                            <TableCell>Date Entre</TableCell>
                            <TableCell>Expiration Verification</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {paginatedProducts.length === 0 ? (
                            <TableRow>
                                <TableCell colSpan={7} align="center">
                                    <Typography variant="h6" color="textSecondary">
                                        No FM1 found!
                                    </Typography>
                                </TableCell>
                            </TableRow>
                        ) : (
                            paginatedProducts.map((product) => (
                                <TableRow key={product.id} hover>
                                    <TableCell>{product.codeSite}</TableCell>
                                    <TableCell>{product.deviceType}</TableCell>
                                    <TableCell>{product.psSn}</TableCell>
                                    <TableCell>{product.dateEntre ? new Date(product.dateEntre).toLocaleDateString() : '-'}</TableCell>
                                    <TableCell>
                                        {product.expirationVerification ? new Date(product.expirationVerification).toLocaleDateString() : '-'}
                                    </TableCell>
                                    <TableCell style={{ color: product.status === 'ccs' ? 'orange' : product.status === 'Reparation' ? 'red' : 'green', fontWeight: 'bold' }}>
                                        {getStatusText(product.status)}
                                    </TableCell>
                                    <TableCell>
                                        
                                        <Tooltip title="History">
                                            <IconButton color="default" onClick={() => handleShowHistory(product.id)}>
                                                <HistoryIcon />
                                            </IconButton>
                                        </Tooltip>
                                        <Tooltip title="View Components">
                                            <IconButton color="default" onClick={() => handleShowComponents(product.id)}>
                                                <ViewListIcon />
                                            </IconButton>
                                        </Tooltip>
                                        <Tooltip title="Delete">
                                            <IconButton color="error" onClick={() => confirmDelete(product.id)}>
                                                <DeleteIcon />
                                            </IconButton>
                                        </Tooltip>
                                    </TableCell>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </TableContainer>

            <Box mt={2} display="flex" justifyContent="center">
                <Pagination
                    count={Math.ceil(products.length / itemsPerPage)}
                    page={currentPage}
                    onChange={handlePageChange}
                />
            </Box>

            <Box textAlign="center" marginTop={2}>
                <Button variant="contained" color="primary" onClick={() => navigate('/add-product')}>
                    Add FM1
                </Button>
            </Box>
        </Container>
    );
};

export default Products;