/*
LES ETAPES : 
-1) Importer des modules nécéssaire 
-2) Déclaration des composants Register :
   2-1) Déclaration des états pour gérer les champs du formulaire
   2-2) Initialisation de la fonction de navigation pour rediriger l'utilisateur 
 -Fonction de Rechargement de la page lors de soumission de formulaire 
 -Vérification si un role ete selectionner 
 -Préparer les donnés a envoyer au backend 
 -Affiche un msg de chargement pendant l'envoi des données
 -Envoie les données au backend via la methode Post 
 -Vérifier la response ( Icone succés)
 -Rédiriger l'user vers la page de connexion  ou msg d'erreur
 -Récuperer le  msg d'erreur de backend
 -Le Formulaire Pour toute les champs 
-Exporation du Composent pour qu'il puisse étre utiliser ailleurs
*/

// 1- Importation des modules nécessaires
import React, { useState } from 'react'; // useState pour gérer les états
import Select from 'react-select'; // Pour la liste déroulante des rôles
import { BASE_URL } from '../../../config'; // URL de base de l'API backend
import {
  CButton,
  CCard,
  CCardBody,
  CCol,
  CContainer,
  CForm,
  CFormInput,
  CInputGroup,
  CInputGroupText,
  CRow,
} from '@coreui/react'; // Composants UI de CoreUI
import CIcon from '@coreui/icons-react'; // Pour utiliser des icônes
import { cilLockLocked, cilUser } from '@coreui/icons'; // Icônes spécifiques
import axios from 'axios'; // Pour faire des requêtes HTTP
import Swal from 'sweetalert2'; // Pour afficher des alertes stylisées
import { useNavigate } from 'react-router-dom'; // Pour la navigation entre les pages
const Register = () => {

  // 2-1 Déclaration des états pour gérer les champs du formulaire
  const [nom, setNom] = useState(''); // État pour le nom de l'utilisateur
  const [prenom, setPrenom] = useState(''); // État pour le prénom de l'utilisateur
  const [email, setEmail] = useState(''); // État pour l'email de l'utilisateur
  const [password, setPassword] = useState(''); // État pour le mot de passe de l'utilisateur
  const [userType, setUserType] = useState(null); // État pour le rôle sélectionné (Gestionnaire ou Expert)

  // 2-2 Initialisation de la fonction de navigation pour rediriger l'utilisateur
  const navigate = useNavigate();

  // 2-3 Options pour la liste déroulante des rôles
  const roleOptions = [
    { value: 'Admin', label: 'Admin' }, // Option pour le rôle Gestionnaire
    { value: 'Expert', label: 'Expert' }, // Option pour le rôle Expert
  ];

  // 2-4 Fonction appelée lors de la soumission du formulaire
  const handleRegister = async (e) => {
    e.preventDefault(); // Empêche le rechargement de la page lors de la soumission

    // Vérifie si un rôle a été sélectionné
    if (!userType) {
      Swal.fire({
        icon: 'error', // Icône d'erreur
        title: 'Erreur', // Titre de l'alerte
        text: 'Veuillez sélectionner un type d\'utilisateur', // Message d'erreur
      });
      return; // Arrête la fonction si aucun rôle n'est sélectionné
    }

    // Préparation des données à envoyer au backend
    const data = {
      nom, // Nom de l'utilisateur
      prenom, // Prénom de l'utilisateur
      email, // Email de l'utilisateur
      password, // Mot de passe de l'utilisateur
      userType: userType.value, // Rôle sélectionné (Gestionnaire ou Expert)
    };

    try {
      // Affiche un message de chargement pendant l'envoi des données
      Swal.fire({
        title: 'Votre demande est en cours',
        text: 'Veuillez patienter...',
        allowOutsideClick: false, // Empêche la fermeture de l'alerte en cliquant à l'extérieur
        didOpen: () => {
          Swal.showLoading(); // Affiche un indicateur de chargement
        },
      });

      // Envoie les données au backend via une requête POST
      const response = await axios.post(`${BASE_URL}Command/register`, data);

      Swal.close(); // Ferme l'alerte de chargement

      // Si la requête réussit (statut 200)
      if (response.status === 200) {
        Swal.fire({
          icon: 'success', // Icône de succès
          title: 'Compte créé avec succès !', // Titre de l'alerte
          showConfirmButton: false, // Pas de bouton de confirmation
          timer: 1500, // Ferme l'alerte après 1,5 seconde
        });

        // Redirige l'utilisateur vers la page de connexion après 1,5 seconde
        setTimeout(() => {
          navigate('/login');
        }, 1500);
      } else {
        // Si la requête échoue, affiche un message d'erreur
        Swal.fire({
          icon: 'error', // Icône d'erreur
          title: 'Erreur', // Titre de l'alerte
          text: response.data.message || 'Une erreur inattendue est survenue', // Message d'erreur
        });
      }
    } catch (error) {
      Swal.close(); // Ferme l'alerte de chargement en cas d'erreur

      // Récupère le message d'erreur du backend ou affiche un message générique
      const errorMessage =
        error.response?.data?.message ||
        (error.response?.data && JSON.stringify(error.response.data)) ||
        'Une erreur inattendue est survenue';

      // Affiche un message d'erreur
      Swal.fire({
        icon: 'error', // Icône d'erreur
        title: 'Erreur', // Titre de l'alerte
        text: errorMessage, // Message d'erreur
      });
    }
  };

  // Rendu du formulaire d'inscription
  return (
    <div className="bg-body-tertiary min-vh-100 d-flex flex-row align-items-center">
      <CContainer fluid>
        <CRow className="justify-content-center">
          <CCol md={9} lg={7} xl={6}>
            <CCard className="mx-4">
              <CCardBody className="p-4">
                {/* Formulaire d'inscription */}
                <CForm onSubmit={handleRegister}>
                  <h1>Inscription</h1>
                  <p className="text-body-secondary">Créez votre compte</p>

                  {/* Champ Nom */}
                  <CRow>
                    <CCol md={6}>
                      <CInputGroup className="mb-3">
                        <CInputGroupText>
                          <CIcon icon={cilUser} /> {/* Icône utilisateur */}
                        </CInputGroupText>
                        <CFormInput
                          placeholder="Nom"
                          autoComplete="nom"
                          value={nom} // Valeur du champ nom
                          onChange={(e) => setNom(e.target.value)} // Met à jour l'état nom
                        />
                      </CInputGroup>
                    </CCol>

                    {/* Champ Prénom */}
                    <CCol md={6}>
                      <CInputGroup className="mb-3">
                        <CInputGroupText>
                          <CIcon icon={cilUser} /> {/* Icône utilisateur */}
                        </CInputGroupText>
                        <CFormInput
                          placeholder="Prénom"
                          autoComplete="prenom"
                          value={prenom} // Valeur du champ prénom
                          onChange={(e) => setPrenom(e.target.value)} // Met à jour l'état prénom
                        />
                      </CInputGroup>
                    </CCol>
                  </CRow>

                  {/* Champ Email */}
                  <CRow>
                    <CCol md={12}>
                      <CInputGroup className="mb-3">
                        <CInputGroupText>@</CInputGroupText> {/* Icône email */}
                        <CFormInput
                          placeholder="Email"
                          autoComplete="email"
                          value={email} // Valeur du champ email
                          onChange={(e) => setEmail(e.target.value)} // Met à jour l'état email
                        />
                      </CInputGroup>
                    </CCol>
                  </CRow>

                  {/* Champ Mot de passe */}
                  <CInputGroup className="mb-3">
                    <CInputGroupText>
                      <CIcon icon={cilLockLocked} /> {/* Icône mot de passe */}
                    </CInputGroupText>
                    <CFormInput
                      type="password"
                      placeholder="Mot de passe"
                      autoComplete="new-password"
                      value={password} // Valeur du champ mot de passe
                      onChange={(e) => setPassword(e.target.value)} // Met à jour l'état password
                    />
                  </CInputGroup>

                  {/* Liste déroulante pour sélectionner un rôle */}
                  <CRow className="mb-3">
                    <CCol md={12}>
                      <Select
                        options={roleOptions} // Options de la liste déroulante
                        onChange={(selectedOption) => setUserType(selectedOption)} // Met à jour l'état userType
                        placeholder="Sélectionnez un rôle" // Texte par défaut
                      />
                    </CCol>
                  </CRow>

                  {/* Bouton de soumission */}
                  <div className="d-grid">
                    <CButton type="submit" color="success">
                      Créer un compte
                    </CButton>
                  </div>
                </CForm>
              </CCardBody>
            </CCard>
          </CCol>
        </CRow>
      </CContainer>
    </div>
  );
};

// 3. Exportation du composant pour qu'il puisse être utilisé ailleurs
export default Register;