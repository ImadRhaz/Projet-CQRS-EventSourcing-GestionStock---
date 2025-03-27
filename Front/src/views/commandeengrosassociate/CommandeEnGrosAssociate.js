// import React, { useState, useEffect } from 'react';
// import { Container, Typography, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, Select, MenuItem, Button } from '@mui/material';
// import axios from 'axios';
// import { useParams } from 'react-router-dom';

// const BASE_URL = 'http://localhost:7029/api/';

// const CommandeEnGrosAssociate = () => {
//   const { orderId } = useParams(); // Get the order ID from the URL parameters
//   const [order, setOrder] = useState(null);
//   const [rows, setRows] = useState([]);
//   const [products, setProducts] = useState([]); // Initialize as an empty array
//   const [submitting, setSubmitting] = useState(false);
//   const [error, setError] = useState(null);

//   useEffect(() => {
//     const fetchOrder = async () => {
//       try {
//         // Fetch the order details to get nombreDeCommandes
//         const orderResponse = await axios.get(`${BASE_URL}commandeengros/${orderId}`);
//         const orderData = orderResponse.data;
//         setOrder(orderData);

//         // Fetch the products list from the API
//         const productsResponse = await axios.get(`${BASE_URL}products/all`);
//         console.log('Products response:', productsResponse.data); // Log the response

//         if (productsResponse.data && productsResponse.data.$values) {
//           // Extract the products from the $values array
//           const productsData = productsResponse.data.$values.map(product => ({
//             id: product.id,
//             sn: product.sn,
//             hardId: product.hardId,
//           }));
//           setProducts(productsData);
//         } else {
//           setProducts([]); // Fallback to an empty array if the response is unexpected
//           console.error('Unexpected API response:', productsResponse.data);
//         }

//         // Initialize rows based on nombreDeCommandes
//         const initialRows = Array(orderData.nombreDeCommandes).fill().map((_, index) => ({
//           id: index + 1,
//           serieNumber: '',
//           hardId: '',
//         }));

//         setRows(initialRows);
//       } catch (err) {
//         console.error('Error fetching data:', err);
//         setError('Failed to fetch order or products details');
//       }
//     };

//     fetchOrder();
//   }, [orderId]);

//   const handleRowChange = (index, field, value) => {
//     const updatedRows = [...rows];
//     updatedRows[index][field] = value;
//     setRows(updatedRows);
//   };

//   const handleSubmit = async (index) => {
//     setSubmitting(true);
//     try {
//       const row = rows[index];
//       if (!row.serieNumber || !row.hardId) {
//         setError('Please fill in both Serial Number and Hardware ID.');
//         setSubmitting(false);
//         return;
//       }

//       const associationData = {
//         commandeEnGrosId: orderId,
//         serieNumber: row.serieNumber,
//         hardId: row.hardId,
//       };

//       await axios.post(`${BASE_URL}commandeengros/associate`, [associationData]);
//       console.log('Association saved successfully');
//     } catch (error) {
//       console.error('Error saving association:', error);
//       setError('Failed to save association');
//     } finally {
//       setSubmitting(false);
//     }
//   };

//   if (!order) return <Typography>Loading...</Typography>;
//   if (error) return <Typography color="error">{error}</Typography>;

//   return (
//     <Container>
//       <Typography variant="h4" gutterBottom>
//         Associate Serial/Hardware IDs for Order: {order.artNmbrComp}
//       </Typography>
//       <TableContainer component={Paper}>
//         <Table>
//           <TableHead>
//             <TableRow>
//               <TableCell>ID</TableCell>
//               <TableCell>Art Number</TableCell>
//               <TableCell>Serie Number</TableCell>
//               <TableCell>Hardware ID</TableCell>
//               <TableCell>Action</TableCell>
//             </TableRow>
//           </TableHead>
//           <TableBody>
//             {rows.length > 0 ? rows.map((row, index) => (
//               <TableRow key={row.id}>
//                 <TableCell>{row.id}</TableCell>
//                 <TableCell>{order.artNmbrComp}</TableCell>
//                 <TableCell>
//                   <Select
//                     value={row.serieNumber}
//                     onChange={(e) => handleRowChange(index, 'serieNumber', e.target.value)}
//                     fullWidth
//                   >
//                     {products.map((product) => (
//                       <MenuItem key={product.id} value={product.sn}>
//                         {product.sn}
//                       </MenuItem>
//                     ))}
//                   </Select>
//                 </TableCell>
//                 <TableCell>
//                   <Select
//                     value={row.hardId}
//                     onChange={(e) => handleRowChange(index, 'hardId', e.target.value)}
//                     fullWidth
//                   >
//                     {products.map((product) => (
//                       <MenuItem key={product.id} value={product.hardId}>
//                         {product.hardId}
//                       </MenuItem>
//                     ))}
//                   </Select>
//                 </TableCell>
//                 <TableCell>
//                   <Button
//                     variant="contained"
//                     color="primary"
//                     onClick={() => handleSubmit(index)}
//                     disabled={submitting || !row.serieNumber || !row.hardId}
//                   >
//                     Associate
//                   </Button>
//                 </TableCell>
//               </TableRow>
//             )) : (
//               <TableRow>
//                 <TableCell colSpan={5} align="center">No rows available</TableCell>
//               </TableRow>
//             )}
//           </TableBody>
//         </Table>
//       </TableContainer>
//     </Container>
//   );
// };

// export default CommandeEnGrosAssociate;
