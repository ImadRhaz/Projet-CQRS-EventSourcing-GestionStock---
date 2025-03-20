// import React, { useState, useEffect } from 'react';
// import axios from 'axios';
// import { Container, Typography, CircularProgress, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper, TextField, Button, Grid, IconButton } from '@mui/material';
// import EditIcon from '@mui/icons-material/Edit';
// import DeleteIcon from '@mui/icons-material/Delete';
// import { useNavigate } from 'react-router-dom';

// const BASE_URL = 'http://localhost:7029/api/';

// const CommandeEnGrosList = () => {
//   const [orders, setOrders] = useState([]);
//   const [loading, setLoading] = useState(true);
//   const [error, setError] = useState(null);
//   const [newOrder, setNewOrder] = useState({
//     artNmbrComp: '',
//     nomDeComposantes: '',
//     nombreDeCommandes: '',
//     degreDeCommande: '',
//     dateDeCommande: '',
//   });
//   const [submitting, setSubmitting] = useState(false);
//   const [technicienId, setTechnicienId] = useState('');
//   const [nomDesTechniciens, setNomDesTechniciens] = useState('');
//   const [editingOrderId, setEditingOrderId] = useState(null);
//   const navigate = useNavigate();

//   useEffect(() => {
//     const fetchOrders = async () => {
//       try {
//         const response = await axios.get(`${BASE_URL}commandeengros`);
//         setOrders(response.data.$values || response.data);
//       } catch (err) {
//         console.error('Error fetching orders:', err);
//         setError('Failed to fetch orders');
//       } finally {
//         setLoading(false);
//       }
//     };

//     const token = localStorage.getItem('token');
//     if (token) {
//       try {
//         const base64Url = token.split('.')[1];
//         const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
//         const jsonPayload = decodeURIComponent(atob(base64).split('').map((c) => {
//           return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
//         }).join(''));

//         const decodedToken = JSON.parse(jsonPayload);
//         console.log('Decoded Token:', decodedToken);

//         if (decodedToken.nameid) {
//           setTechnicienId(decodedToken.nameid);
//           const fullName = `${decodedToken.given_name} ${decodedToken.family_name}`;
//           setNomDesTechniciens(fullName);
//         } else {
//           console.error('Token does not contain a technicien ID');
//           setError('Token is missing technicien ID');
//         }
//       } catch (error) {
//         console.error('Error decoding token:', error);
//         setError('Failed to decode token');
//       }
//     } else {
//       console.error('No token found');
//     }

//     fetchOrders();
//   }, []);

//   const handleChange = (e) => {
//     const { name, value } = e.target;
//     setNewOrder((prev) => ({
//       ...prev,
//       [name]: value,
//     }));
//   };

//   const handleSubmit = async (e) => {
//     e.preventDefault();
//     setSubmitting(true);

//     try {
//       const orderData = {
//         artNmbrComp: newOrder.artNmbrComp,
//         nomDeComposantes: newOrder.nomDeComposantes,
//         nombreDeCommandes: newOrder.nombreDeCommandes,
//         degreDeCommande: newOrder.degreDeCommande,
//         dateDeCommande: newOrder.dateDeCommande,
//         nombreDeCommandesEnCours: 0, // Set default value
//         nombreDeCommandesValides: 0, // Set default value
//         technicienId: technicienId, // Ensure the technicienId is passed
//         nomDesTechniciens: nomDesTechniciens, // Ensure the name is passed
//       };

//       console.log('Submitting Order Data:', orderData);

//       let response;
//       if (editingOrderId) {
//         response = await axios.put(`${BASE_URL}commandeengros/${editingOrderId}`, orderData); // Use PUT for updates
//       } else {
//         response = await axios.post(`${BASE_URL}commandeengros`, orderData); // Use POST for new entries
//       }

//       setOrders((prev) => {
//         if (editingOrderId) {
//           return prev.map(order => order.id === editingOrderId ? response.data : order);
//         } else {
//           return [...prev, response.data];
//         }
//       });

//       // Reset form and state after submission
//       setNewOrder({
//         artNmbrComp: '',
//         nomDeComposantes: '',
//         nombreDeCommandes: '',
//         degreDeCommande: '',
//         dateDeCommande: '',
//       });
//       setEditingOrderId(null); // Reset the selected order after update

//     } catch (err) {
//       console.error('Error adding/updating order:', err);
//       if (err.response) {
//         console.error('Response data:', err.response.data); // Log the server response
//       }
//       setError('Failed to add/update order');
//     } finally {
//       setSubmitting(false);
//     }
//   };

//   const handleEdit = (order) => {
//     setNewOrder({
//       artNmbrComp: order.artNmbrComp,
//       nomDeComposantes: order.nomDeComposantes,
//       nombreDeCommandes: order.nombreDeCommandes,
//       degreDeCommande: order.degreDeCommande,
//       dateDeCommande: order.dateDeCommande.split('T')[0],
//     });
//     setEditingOrderId(order.id);
//   };

//   const handleDelete = async (orderId) => {
//     try {
//       await axios.delete(`${BASE_URL}commandeengros/${orderId}`);
//       setOrders((prev) => prev.filter((order) => order.id !== orderId));
//     } catch (err) {
//       console.error('Error deleting order:', err);
//       setError('Failed to delete order');
//     }
//   };

//   if (loading) return <CircularProgress />;
//   if (error) return <Typography color="error">{error}</Typography>;

//   return (
//     <Container>
//       <Typography variant="h4" gutterBottom>
//         Liste des Commandes en Gros
//       </Typography>
//       <Typography variant="h6">Technicien ID: {technicienId}</Typography>
//       <Typography variant="h6">Technicien Name: {nomDesTechniciens}</Typography>

//       <form onSubmit={handleSubmit}>
//         <Grid container spacing={2}>
//           <Grid item xs={12} sm={6}>
//             <TextField
//               label="Art Number"
//               name="artNmbrComp"
//               value={newOrder.artNmbrComp}
//               onChange={handleChange}
//               fullWidth
//               required
//             />
//           </Grid>
//           <Grid item xs={12} sm={6}>
//             <TextField
//               label="Date de Commande"
//               name="dateDeCommande"
//               type="date"
//               value={newOrder.dateDeCommande}
//               onChange={handleChange}
//               fullWidth
//               required
//               InputLabelProps={{ shrink: true }}
//             />
//           </Grid>
//           <Grid item xs={12}>
//             <TextField
//               label="Nombre de Composantes"
//               name="nomDeComposantes"
//               value={newOrder.nomDeComposantes}
//               onChange={handleChange}
//               fullWidth
//               required
//             />
//           </Grid>
//           <Grid item xs={12}>
//             <TextField
//               label="Nombre de Commandes"
//               name="nombreDeCommandes"
//               value={newOrder.nombreDeCommandes}
//               onChange={handleChange}
//               fullWidth
//               required
//             />
//           </Grid>
//           <Grid item xs={12}>
//             <TextField
//               label="Degré de Commande"
//               name="degreDeCommande"
//               value={newOrder.degreDeCommande}
//               onChange={handleChange}
//               fullWidth
//               required
//             />
//           </Grid>
//           <Grid item xs={12}>
//             <Button type="submit" variant="contained" color="primary" disabled={submitting}>
//               {submitting ? 'Submitting...' : editingOrderId ? 'Update Order' : 'Add Order'}
//             </Button>
//           </Grid>
//         </Grid>
//       </form>

//       <TableContainer component={Paper}>
//         <Table>
//           <TableHead>
//             <TableRow>
//               <TableCell>ID</TableCell>
//               <TableCell>Art Number</TableCell>
//               <TableCell>Technicien</TableCell>
//               <TableCell>Date de Commande</TableCell>
//               <TableCell>Nombre de Composantes</TableCell>
//               <TableCell>Nombre de Commandes</TableCell>
//               <TableCell>Commandes En Cours</TableCell>
//               <TableCell>Commandes Valides</TableCell>
//               <TableCell>Degré de Commande</TableCell>
//               <TableCell>Actions</TableCell>
//             </TableRow>
//           </TableHead>
//           <TableBody>
//             {orders.map((order, index) => (
//               <TableRow key={index}>
//                 <TableCell>{order.id}</TableCell>
//                 <TableCell>{order.artNmbrComp}</TableCell>
//                 <TableCell>{order.nomDesTechniciens}</TableCell>
//                 <TableCell>{new Date(order.dateDeCommande).toLocaleDateString()}</TableCell>
//                 <TableCell>{order.nomDeComposantes}</TableCell>
//                 <TableCell>{order.nombreDeCommandes}</TableCell>
//                 <TableCell>{order.nombreDeCommandesEnCours}</TableCell>
//                 <TableCell>{order.nombreDeCommandesValides}</TableCell>
//                 <TableCell>{order.degreDeCommande}</TableCell>
//                 <TableCell>
//                   <IconButton onClick={() => handleEdit(order)} color="primary">
//                     <EditIcon />
//                   </IconButton>
//                   <IconButton onClick={() => handleDelete(order.id)} color="secondary">
//                     <DeleteIcon />
//                   </IconButton>
//                   <Button
//                     variant="contained"
//                     color="secondary"
//                     onClick={() => navigate(`/associate/${order.id}`)} // Navigate to the associate view
//                   >
//                     Associate
//                   </Button>
//                 </TableCell>
//               </TableRow>
//             ))}
//           </TableBody>
//         </Table>
//       </TableContainer>
//     </Container>
//   );
// };

// export default CommandeEnGrosList;
