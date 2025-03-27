// import React, { useState, useEffect } from 'react';
// import axios from 'axios';
// import {
//   Container,
//   Table,
//   TableBody,
//   TableCell,
//   TableContainer,
//   TableHead,
//   TableRow,
//   Paper,
//   Button,
//   Typography,
//   Box,
//   CircularProgress,
//   Pagination,
//   TextField,
//   Dialog,
//   DialogActions,
//   DialogContent,
//   DialogTitle,
//   IconButton,
// } from '@mui/material';
// import EditIcon from '@mui/icons-material/Edit';
// import { jwtDecode } from 'jwt-decode'; // Assurez-vous d'avoir installé jwt-decode
// import { BASE_URL } from '../../config';

// const ExcelComposents = () => {
//   const [composents, setComposents] = useState([]);
//   const [filteredComposents, setFilteredComposents] = useState([]);
//   const [loading, setLoading] = useState(true);
//   const [currentPage, setCurrentPage] = useState(1);
//   const [searchTerm, setSearchTerm] = useState('');
//   const [openDialog, setOpenDialog] = useState(false);
//   const [selectedComposent, setSelectedComposent] = useState(null);
//   const [newTotalAvailable, setNewTotalAvailable] = useState('');
//   const [role, setRole] = useState(''); // Ajouter l'état du rôle
//   const itemsPerPage = 10;

//   useEffect(() => {
//     const fetchComposents = async () => {
//       setLoading(true);
//       try {
//         const response = await axios.get(`${BASE_URL}ExcelComposent/get-all`);
//         const data = response.data.$values ? response.data.$values : response.data;
//         setComposents(data);
//         setFilteredComposents(data);
//       } catch (err) {
//         console.error('Erreur lors de la récupération des composants Excel:', err);
//       } finally {
//         setLoading(false);
//       }
//     };

//     fetchComposents();
//   }, []);

//   useEffect(() => {
//     const token = localStorage.getItem('token');
//     if (token) {
//       const decodedToken = jwtDecode(token);
//       setRole(decodedToken.role); // Décodez et définissez le rôle
//     }
//   }, []); 

//   const handlePageChange = (event, value) => {
//     setCurrentPage(value);
//   };

//   const handleSearchChange = (event) => {
//     setSearchTerm(event.target.value);
//     if (event.target.value === '') {
//       setFilteredComposents(composents);
//     } else {
//       const filtered = composents.filter((composent) =>
//         composent.anComposent.toLowerCase().includes(event.target.value.toLowerCase()) ||
//         composent.composentName.toLowerCase().includes(event.target.value.toLowerCase())
//       );
//       setFilteredComposents(filtered);
//       setCurrentPage(1);
//     }
//   };

//   const getPaginatedComposents = () => {
//     const startIndex = (currentPage - 1) * itemsPerPage;
//     const endIndex = startIndex + itemsPerPage;
//     return Array.isArray(filteredComposents)
//       ? filteredComposents.slice(startIndex, endIndex)
//       : [];
//   };

//   const handleOpenDialog = (composent) => {
//     setSelectedComposent(composent);
//     setNewTotalAvailable(composent.totalAvailable.toString());
//     setOpenDialog(true);
//   };

//   const handleCloseDialog = () => {
//     setOpenDialog(false);
//     setSelectedComposent(null);
//     setNewTotalAvailable('');
//   };

//   const handleUpdateTotalAvailable = async () => {
//     if (selectedComposent) {
//       try {
//         const response = await axios.patch(`${BASE_URL}ExcelComposent/${selectedComposent.id}/update-total-available`, {
//           totalAvailable: parseFloat(newTotalAvailable),
//         });

//         const updatedComposents = composents.map((composent) =>
//           composent.id === selectedComposent.id ? { ...composent, totalAvailable: response.data.totalAvailable } : composent
//         );
//         setComposents(updatedComposents);
//         setFilteredComposents(updatedComposents);
//         handleCloseDialog();
//       } catch (err) {
//         console.error('Erreur lors de la mise à jour de TotalAvailable:', err);
//       }
//     }
//   };

//   if (loading) return <CircularProgress />;

//   const paginatedComposents = getPaginatedComposents();

//   return (
//     <Container>
//       {/* Titre et affichage du rôle */}
//       <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
//         <Typography variant="h4" gutterBottom>
//           Liste des Composants Excel
//         </Typography>

//         {/* Affichage du rôle */}
//         <Typography variant="h6" gutterBottom>
//           Role: {role === 'Technicien' ? 'Maintenancier' : role}
//         </Typography>
//       </Box>

//       <Box mb={3}>
//         <TextField
//           label="Rechercher par AnComposent ou Nom du Composant"
//           variant="outlined"
//           fullWidth
//           value={searchTerm}
//           onChange={handleSearchChange}
//         />
//       </Box>

//       <TableContainer component={Paper}>
//         <Table>
//           <TableHead>
//             <TableRow>
//               <TableCell>ID</TableCell>
//               <TableCell>AnComposent</TableCell>
//               <TableCell>Nom du Composant</TableCell>
//               <TableCell>SN Composant</TableCell>
//               <TableCell>Total Disponible</TableCell>
//               {role !== 'Technicien' && <TableCell>Actions</TableCell>}
//             </TableRow>
//           </TableHead>
//           <TableBody>
//             {paginatedComposents.length === 0 ? (
//               <TableRow>
//                 <TableCell colSpan={6} align="center">
//                   <Typography variant="h6" color="textSecondary">
//                     Aucun composant trouvé !
//                   </Typography>
//                 </TableCell>
//               </TableRow>
//             ) : (
//               paginatedComposents.map((composent) => (
//                 <TableRow key={composent.id}>
//                   <TableCell>{composent.id}</TableCell>
//                   <TableCell>{composent.anComposent}</TableCell>
//                   <TableCell>{composent.composentName}</TableCell>
//                   <TableCell>{composent.snComposent}</TableCell>
//                   <TableCell>{composent.totalAvailable}</TableCell>
//                   {role !== 'Technicien' && (
//                     <TableCell>
//                       <IconButton onClick={() => handleOpenDialog(composent)}>
//                         <EditIcon style={{ color: 'blue' }} />
//                       </IconButton>
//                     </TableCell>
//                   )}
//                 </TableRow>
//               ))
//             )}
//           </TableBody>
//         </Table>
//       </TableContainer>

//       <Box mt={2} display="flex" justifyContent="center">
//         <Pagination
//           count={Math.ceil(filteredComposents.length / itemsPerPage)}
//           page={currentPage}
//           onChange={handlePageChange}
//         />
//       </Box>

//       <Dialog open={openDialog} onClose={handleCloseDialog}>
//         <DialogTitle>Mettre à jour Total Disponible</DialogTitle>
//         <DialogContent>
//           <TextField
//             label="Total Disponible"
//             type="number"
//             fullWidth
//             value={newTotalAvailable}
//             onChange={(e) => setNewTotalAvailable(e.target.value)}
//           />
//         </DialogContent>
//         <DialogActions>
//           <Button onClick={handleCloseDialog} color="primary">
//             Annuler
//           </Button>
//           <Button onClick={handleUpdateTotalAvailable} color="primary">
//             Mettre à jour
//           </Button>
//         </DialogActions>
//       </Dialog>
//     </Container>
//   );
// };

// export default ExcelComposents;
