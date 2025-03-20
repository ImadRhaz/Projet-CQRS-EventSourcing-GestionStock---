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
//   CircularProgress,
//   Typography,
//   Box,
//   TextField,
//   Pagination,
// } from '@mui/material';
// import { BASE_URL } from '../../config'; // Ensure BASE_URL is correctly set to your backend API

// const ExcelFm1List = () => {
//   const [excelFm1Entries, setExcelFm1Entries] = useState([]);
//   const [filteredEntries, setFilteredEntries] = useState([]);
//   const [loading, setLoading] = useState(true);
//   const [currentPage, setCurrentPage] = useState(1);
//   const [searchTerm, setSearchTerm] = useState('');
//   const itemsPerPage = 10; // Set the number of items to display per page

//   useEffect(() => {
//     const fetchExcelFm1Entries = async () => {
//       setLoading(true);
//       try {
//         const response = await axios.get(`${BASE_URL}ExcelFm1/get-all`);
//         const data = response.data.$values ? response.data.$values : response.data;
//         setExcelFm1Entries(data);
//         setFilteredEntries(data); // Initially, show all data
//       } catch (err) {
//         console.error('Erreur lors de la récupération des entrées ExcelFm1:', err);
//       } finally {
//         setLoading(false);
//       }
//     };

//     fetchExcelFm1Entries();
//   }, []);

//   // Handle pagination page change
//   const handlePageChange = (event, value) => {
//     setCurrentPage(value);
//   };

//   // Handle search term changes and filter the entries
//   const handleSearchChange = (event) => {
//     const searchValue = event.target.value.toLowerCase();
//     setSearchTerm(searchValue);

//     // Filter based on the search term
//     const filtered = excelFm1Entries.filter(
//       (entry) =>
//         entry.siteCode.toLowerCase().includes(searchValue) ||
//         entry.typeDevice.toLowerCase().includes(searchValue) ||
//         entry.snPs.toLowerCase().includes(searchValue)
//     );

//     setFilteredEntries(filtered);
//     setCurrentPage(1); // Reset to the first page after filtering
//   };

//   // Get the paginated entries to display on the current page
//   const getPaginatedEntries = () => {
//     const startIndex = (currentPage - 1) * itemsPerPage;
//     const endIndex = startIndex + itemsPerPage;
//     return Array.isArray(filteredEntries) ? filteredEntries.slice(startIndex, endIndex) : [];
//   };

//   // Display loading indicator when fetching data
//   if (loading) return <CircularProgress />;

//   // Get the entries to display on the current page
//   const paginatedEntries = getPaginatedEntries();

//   return (
//     <Container>
//       <Typography variant="h4" gutterBottom>
//         Liste des Entrées ExcelFm1
//       </Typography>

//       {/* Search Field */}
//       <Box mb={3}>
//         <TextField
//           label="Rechercher par Code du Site, Type de Dispositif ou SN PS"
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
//               <TableCell>Code du Site</TableCell>
//               <TableCell>Type de Dispositif</TableCell>
//               <TableCell>SN PS</TableCell>
//             </TableRow>
//           </TableHead>
//           <TableBody>
//             {paginatedEntries.length === 0 ? (
//               <TableRow>
//                 <TableCell colSpan={4} align="center">
//                   <Typography variant="h6" color="textSecondary">
//                     Aucun enregistrement trouvé !
//                   </Typography>
//                 </TableCell>
//               </TableRow>
//             ) : (
//               paginatedEntries.map((entry) => (
//                 <TableRow key={entry.id}>
//                   <TableCell>{entry.id}</TableCell>
//                   <TableCell>{entry.siteCode}</TableCell>
//                   <TableCell>{entry.typeDevice}</TableCell>
//                   <TableCell>{entry.snPs}</TableCell>
//                 </TableRow>
//               ))
//             )}
//           </TableBody>
//         </Table>
//       </TableContainer>

//       {/* Pagination Controls */}
//       <Box mt={2} display="flex" justifyContent="center">
//         <Pagination
//           count={Math.ceil(filteredEntries.length / itemsPerPage)}
//           page={currentPage}
//           onChange={handlePageChange}
//         />
//       </Box>
//     </Container>
//   );
// };

// export default ExcelFm1List;
