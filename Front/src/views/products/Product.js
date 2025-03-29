import React, { useState, useEffect, useCallback } from 'react';
import {
  Container, Table, TableBody, TableCell, TableContainer, TableHead,
  TableRow, Paper, Button, Typography, Box, CircularProgress,
  Pagination, Tooltip, IconButton,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import HistoryIcon from '@mui/icons-material/History';
import ViewListIcon from '@mui/icons-material/ViewList';
import Swal from 'sweetalert2';
import { useNavigate, useParams } from 'react-router-dom';
import { BASE_URL } from '../../config';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const Products = () => {
  const { id } = useParams();
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;
  const navigate = useNavigate();
  const [userId, setUserId] = useState(null);
  const [roles, setRoles] = useState([]);

  const handlePageChange = useCallback((event, value) => {
    setCurrentPage(value);
  }, []);

  const fetchUserRoles = useCallback(async (userId) => {
    try {
      const response = await axios.get(`${BASE_URL}Query/user-roles/${userId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
      });
      setRoles(response.data);
      console.log('Le rôle est :', response.data); // Affiche les rôles dans la console
    } catch (err) {
      console.error('Erreur lors de la récupération des rôles utilisateur :', err);
      setError('Échec de la récupération des rôles utilisateur.');
    }
  }, [BASE_URL]);

  const handleDelete = useCallback(async (productId) => {
    try {
      const response = await axios.delete(`${BASE_URL}Query/fm1/${productId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
      });
      // Suppression réussie
      setProducts(prevProducts => prevProducts.filter(product => product.id !== productId));
      Swal.fire('Supprimé !', 'Le fm1 a été supprimé.', 'success');
    } catch (error) {
      if (error.response && error.response.status === 500) {  // Vérifie le code de statut Conflict (409)
        Swal.fire('Erreur', 'Ce FM1 a des commandes associées et ne peut pas être supprimé.', 'error');
      } else {
        // Gère les autres erreurs (par exemple, 500 ou erreurs réseau)
        console.error('Erreur lors de la suppression du produit :', error);
        Swal.fire('Erreur', `Échec de la suppression du FM1 : ${error.message}`, 'error'); // Message d'erreur générique
      }
    }
  }, [BASE_URL]);

  const confirmDelete = useCallback((productId) => {
    Swal.fire({
      title: 'Êtes-vous sûr de vouloir supprimer ce fm1 ?',
      text: 'Cette action est irréversible !',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Oui, supprimez-le !',
      cancelButtonText: 'Annuler',
    }).then((result) => {
      if (result.isConfirmed) {
        handleDelete(productId);
      }
    });
  }, [handleDelete]);

  const handleShowHistory = useCallback((productId) => {
    navigate(`/product-history/${productId}`);
  }, [navigate]);

  const handleShowComponents = useCallback((productId) => {
    navigate(`/product-components/${productId}`);
  }, [navigate]);

  const fetchProducts = useCallback(async () => {
    setLoading(true);
    try {
      const endpoint = id ? `${BASE_URL}Query/fm1s/${id}` : `${BASE_URL}Query/fm1s`;
      const response = await axios.get(endpoint, {
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
      });
      setProducts(id ? [response.data] : response.data || []);
      setError(null);
    } catch (err) {
      console.error('Erreur lors de la récupération des produits :', err);
      setError('Échec de la récupération des données !');
    } finally {
      setLoading(false);
    }
  }, [BASE_URL, id]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        const userId = decodedToken.nameid || decodedToken.sub;
        const decodedRoles = decodedToken.role || []; // Récupère les rôles du jeton

        if (!userId) {
          throw new Error('ID utilisateur introuvable dans le jeton.');
        }
        setUserId(userId);
        setRoles(Array.isArray(decodedRoles) ? decodedRoles : [decodedRoles]); // Assure que les rôles sont un tableau
        console.log('Rôles décodés :', Array.isArray(decodedRoles) ? decodedRoles : [decodedRoles]); // Affiche les rôles décodés

      } catch (error) {
        console.error('Erreur lors du décodage du jeton :', error);
        setError('Jeton invalide. Veuillez vous reconnecter.');
      }
    } else {
      console.error('Aucun jeton trouvé dans localStorage');
      setError('Vous devez vous reconnecter.');
    }
  }, []);

  useEffect(() => {
    if (userId) {
      fetchProducts();
    }
  }, [userId, fetchProducts, id]);

  const getPaginatedProducts = () => {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return products.slice(startIndex, endIndex);
  };

  const getStatusText = (status) => {
    switch (status) {
      case 'ccs': return 'En Attente';
      case 'Reparation': return 'En Réparation';
      case 'Reparer': return 'Réparé';
      default: return 'Inconnu';
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
      <Container><Typography color="error">{error}</Typography></Container>
    );
  }

  const paginatedProducts = getPaginatedProducts();

  return (
    <Container>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h4" gutterBottom>
          Liste FM1 {id ? `(ID : ${id})` : ""}
        </Typography>
        <Typography variant="h6" gutterBottom>
          ID utilisateur connecté : {userId || 'Non disponible'}
        </Typography>
        <Typography variant="h6" gutterBottom>
          Rôles : {roles.join(', ') || 'Aucun rôle'}
        </Typography>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Code Site</TableCell>
              <TableCell>Type d'appareil</TableCell>
              <TableCell>PS SN</TableCell>
              <TableCell>Date d'entrée</TableCell>
              <TableCell>Date de dernière Verification</TableCell>
              <TableCell>Statut</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedProducts.length === 0 ? (
              <TableRow>
                <TableCell colSpan={7} align="center">
                  <Typography variant="h6" color="textSecondary">
                    Aucun FM1 trouvé !
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              paginatedProducts.map((product) => (
                <TableRow key={product.id} hover>
                  <TableCell>{product.codeSite}</TableCell>
                  <TableCell>{product.deviceType}</TableCell>
                  <TableCell>{product.psSn}</TableCell>
                  <TableCell>
                    {product.dateEntre ? new Date(product.dateEntre).toLocaleDateString() : '-'}
                  </TableCell>
                  <TableCell>
                    {product.expirationVerification ? new Date(product.expirationVerification).toLocaleDateString() : '-'}
                  </TableCell>
                  <TableCell style={{
                    color: product.status === 'ccs' ? 'orange' : product.status === 'Reparation' ? 'red' : 'green',
                    fontWeight: 'bold'
                  }}>
                    {getStatusText(product.status)}
                  </TableCell>
                  <TableCell>
                    <Tooltip title="Historique">
                      <IconButton color="default" onClick={() => handleShowHistory(product.id)}>
                        <HistoryIcon />
                      </IconButton>
                    </Tooltip>
                    {roles.includes('Expert') && ( // Vérifie si "Magasinier" est dans le tableau des rôles
                      <Tooltip title="Afficher les composants">
                        <IconButton color="default" onClick={() => handleShowComponents(product.id)}>
                          <ViewListIcon />
                        </IconButton>
                      </Tooltip>
                    )}
                    <Tooltip title="Supprimer">
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

      {!id && (
        <Box mt={2} display="flex" justifyContent="center">
          <Pagination
            count={Math.ceil(products.length / itemsPerPage)}
            page={currentPage}
            onChange={handlePageChange}
          />
        </Box>
      )}

      {!roles.includes('Expert') && (
        <Box textAlign="center" marginTop={2}>
          <Button variant="contained" color="primary" onClick={() => navigate('/add-product')}>
            Ajouter un FM1
          </Button>
        </Box>
      )}
    </Container>
  );
};

export default Products;