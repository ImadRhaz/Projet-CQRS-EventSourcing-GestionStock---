import React, { useState, useEffect } from 'react';
import axios from 'axios';
import {
  Container,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Typography,
  Box,
  CircularProgress,
  Pagination,
  TextField,
} from '@mui/material';
import { jwtDecode } from 'jwt-decode';
import { BASE_URL } from '../../config';

const ExcelComposents = () => {
  const [composents, setComposents] = useState([]);
  const [filteredComposents, setFilteredComposents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [role, setRole] = useState('');
  const itemsPerPage = 10;
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchComposents = async () => {
      setLoading(true);
      try {
        const response = await axios.get(`${BASE_URL}ImportExcel/get-all-composent`); // Updated URL
        setComposents(response.data);
        setFilteredComposents(response.data);
      } catch (error) {
        console.error('Error fetching composents:', error);
        setError('Failed to load composents.');
      } finally {
        setLoading(false);
      }
    };

    fetchComposents();
  }, []);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        setRole(decodedToken.role);
      } catch (error) {
        console.error("Error decoding token:", error);
        localStorage.removeItem('token');
        setRole('');
      }
    }
  }, []);

  const handlePageChange = (event, value) => {
    setCurrentPage(value);
  };

  const handleSearchChange = (event) => {
    const searchTermValue = event.target.value;
    setSearchTerm(searchTermValue);

    if (searchTermValue === '') {
      setFilteredComposents(composents);
      setCurrentPage(1);
    } else {
      const filtered = composents.filter(
        (composent) =>
          composent.anComposent?.toLowerCase().includes(searchTermValue.toLowerCase()) ||
          composent.composentName?.toLowerCase().includes(searchTermValue.toLowerCase())
      );
      setFilteredComposents(filtered);
      setCurrentPage(1);
    }
  };

  const getPaginatedComposents = () => {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    return filteredComposents.slice(startIndex, endIndex);
  };

  if (loading)
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <CircularProgress />
      </Box>
    );

  if (error) return <Typography color="error">{error}</Typography>;

  const paginatedComposents = getPaginatedComposents();

  return (
    <Container>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h4" gutterBottom>
          Liste des Composants Excel
        </Typography>
        <Typography variant="h6" gutterBottom>
          Role: {role === 'Technicien' ? 'Maintenancier' : role}
        </Typography>
      </Box>

      <Box mb={3}>
        <TextField
          label="Rechercher par AnComposent ou Nom du Composant"
          variant="outlined"
          fullWidth
          value={searchTerm}
          onChange={handleSearchChange}
        />
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>AnComposent</TableCell>
              <TableCell>Nom du Composant</TableCell>
              <TableCell>SN Composant</TableCell>
              <TableCell>Total Disponible</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedComposents.length === 0 ? (
              <TableRow>
                <TableCell colSpan={5} align="center">
                  <Typography variant="h6" color="textSecondary">
                    Aucun composant trouvé !
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              paginatedComposents.map((composent) => (
                <TableRow key={composent.id}>
                  <TableCell>{composent.id}</TableCell>
                  <TableCell>{composent.anComposent}</TableCell>
                  <TableCell>{composent.composentName}</TableCell>
                  <TableCell>{composent.snComposent}</TableCell>
                  <TableCell>{composent.totalAvailable}</TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <Box mt={2} display="flex" justifyContent="center">
        <Pagination
          count={Math.ceil(filteredComposents.length / itemsPerPage)}
          page={currentPage}
          onChange={handlePageChange}
        />
      </Box>
    </Container>
  );
};

export default ExcelComposents;