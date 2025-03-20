
// 1- Importation des modules nécessaires
import React, { useState, useEffect } from 'react'; // Hooks React
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
  TextField,
} from '@mui/material'; // Composants Material-UI
import { useParams, useNavigate } from 'react-router-dom'; // Pour la navigation et les paramètres d'URL
import ArrowBackIcon from '@mui/icons-material/ArrowBack'; // Icône de retour
import jsPDF from 'jspdf'; // Pour générer des PDF
import 'jspdf-autotable'; // Pour ajouter des tableaux au PDF
import axios from 'axios'; // Pour les requêtes HTTP
import { BASE_URL } from '../../config'; // URL de base de l'API

// 2- Déclaration du composant VoitureHistory
const VoitureHistory = () => {
  console.log("🟢 Composant VoitureHistory chargé !");

  const { id } = useParams(); // Récupère l'ID de la voiture depuis l'URL
  const [voitureHistory, setVoitureHistory] = useState(null); // État pour stocker l'historique
  const [loading, setLoading] = useState(true); // État pour le chargement
  const [error, setError] = useState(null); // État pour les erreurs
  const [currentPage, setCurrentPage] = useState(1); // État pour la pagination
  const [searchTerm, setSearchTerm] = useState(''); // État pour la recherche
  const itemsPerPage = 10; // Nombre d'éléments par page
  const navigate = useNavigate(); // Pour la navigation

  // 3- Récupération de l'historique de la voiture
  useEffect(() => {
    console.log("📢 useEffect déclenché avec ID :", id);

    const fetchVoitureHistory = async () => {
      setLoading(true);
      const url = `${BASE_URL}VoitureHistory/voiture/${id}/history`; // URL de l'API

      console.log("🚀 Tentative d’appel API :", url);

      try {
        const response = await axios.get(url);
        console.log("✅ Réponse API complète :", response);
        console.log("🔹 Données reçues depuis l'API :", response.data);
        setVoitureHistory(response.data); // Met à jour l'état avec les données reçues
      } catch (err) {
        console.error("❌ Erreur lors de l’appel API :", err);
        setError("Erreur lors de la récupération des données."); // Affiche une erreur
      } finally {
        setLoading(false); // Désactive le chargement
      }
    };

    fetchVoitureHistory();
  }, [id]);

  // 4- Gestion de la pagination
  const handlePageChange = (event, value) => {
    setCurrentPage(value); // Met à jour la page courante
  };

  // 5- Gestion de la recherche
  const handleSearchChange = (event) => {
    setSearchTerm(event.target.value); // Met à jour le terme de recherche
    setCurrentPage(1); // Réinitialise la pagination
  };

  // 6- Filtrage des commandes en fonction du terme de recherche
  const filteredCommands = voitureHistory?.commandes?.filter(
    (cmd) =>
      cmd.raisonDeCommande.toLowerCase().includes(searchTerm.toLowerCase()) ||
      cmd.etatCommande.toLowerCase().includes(searchTerm.toLowerCase())
  ) || [];

  // 7- Pagination des commandes filtrées
  const paginatedCommands = filteredCommands.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage
  );

  // 8- Téléchargement de l'historique au format PDF
  const downloadPDF = () => {
    const doc = new jsPDF(); // Crée un nouveau document PDF

    // Définition des colonnes du tableau
    const tableColumn = [
      { header: 'État de la commande', dataKey: 'etatCommande' },
      { header: 'Date de la commande', dataKey: 'dateCmd' },
      { header: 'Raison de la commande', dataKey: 'raisonDeCommande' },
      { header: 'Nom de la pièce', dataKey: 'pieceName' },
      { header: 'Numéro de série', dataKey: 'pieceNumeroSerie' },
      { header: 'Expert', dataKey: 'expert' }, // Colonne pour l'expert
    ];

    // Préparation des données pour le tableau
    const tableRows = filteredCommands.map((cmd) => ({
      etatCommande: cmd.etatCommande || '-',
      dateCmd: new Date(cmd.dateCmd).toLocaleDateString('fr-FR'),
      raisonDeCommande: cmd.raisonDeCommande || '-',
      pieceName: cmd.pieceName || '-',
      pieceNumeroSerie: cmd.pieceNumeroSerie || '-',
      expert: `${cmd.expertNom} ${cmd.expertPrenom}` || '-', // Ajout de l'expert
    }));

    // Ajout du titre et des informations de la voiture
    doc.text('Historique des Commandes', 14, 20);
    doc.text(`Voiture: ${voitureHistory?.voiture?.marque} ${voitureHistory?.voiture?.type}`, 14, 30);
    doc.text(`Matricule: ${voitureHistory?.voiture?.matricule}`, 14, 40);

    // Ajout du tableau au PDF
    doc.autoTable({
      columns: tableColumn,
      body: tableRows,
      startY: 50,
    });

    // Sauvegarde du PDF
    doc.save(`historique_voiture_${id}.pdf`);
  };

  // 9- Affichage du chargement ou des erreurs
  if (loading) return <CircularProgress />;
  if (error) return <Typography color="error">{error}</Typography>;

  // 10- Rendu de l'interface
  return (
    <Container>
      {}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
        <Typography variant="h4" gutterBottom>
          Voiture: <span style={{ color: 'blue' }}>{voitureHistory?.voiture?.marque} {voitureHistory?.voiture?.type}</span> -{' '}
          <span style={{ color: voitureHistory?.voiture?.status === 1 ? 'green' : 'red', fontWeight: 'bold' }}>
            {voitureHistory?.voiture?.status === 1 ? 'En Attente' : 'Réparée'}
          </span>
        </Typography>
      </Box>

      {}
      <Box mb={2}>
        <Button variant="outlined" color="primary" onClick={() => navigate('/voitures')}>
          <ArrowBackIcon />
        </Button>
      </Box>

      {}
      <Box mb={3}>
        <TextField
          label="Rechercher par Raison/État de la commande"
          variant="outlined"
          fullWidth
          value={searchTerm}
          onChange={handleSearchChange}
        />
      </Box>

      {}
      <Box mb={2}>
        <Button variant="contained" color="primary" onClick={downloadPDF}>
          Télécharger l'Historique
        </Button>
      </Box>

      {}
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>État de la commande</TableCell>
              <TableCell>Date de la commande</TableCell>
              <TableCell>Raison de la commande</TableCell>
              <TableCell>Nom de la pièce</TableCell>
              <TableCell>Numéro de série</TableCell>
              <TableCell>Expert</TableCell> {}
            </TableRow>
          </TableHead>
          <TableBody>
            {paginatedCommands.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography variant="h6" color="textSecondary">
                    Aucun historique trouvé !
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              paginatedCommands.map((cmd, index) => (
                <TableRow key={index}>
                  <TableCell>{cmd.etatCommande}</TableCell>
                  <TableCell>{new Date(cmd.dateCmd).toLocaleDateString('fr-FR')}</TableCell>
                  <TableCell>{cmd.raisonDeCommande}</TableCell>
                  <TableCell>{cmd.pieceName}</TableCell>
                  <TableCell>{cmd.pieceNumeroSerie}</TableCell>
                  <TableCell>{cmd.expertNom} {cmd.expertPrenom}</TableCell> {}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {}
      <Box mt={2} display="flex" justifyContent="center">
        <Pagination
          count={Math.ceil(filteredCommands.length / itemsPerPage)}
          page={currentPage}
          onChange={handlePageChange}
        />
      </Box>
    </Container>
  );
};

export default VoitureHistory; // Exporte le composant
