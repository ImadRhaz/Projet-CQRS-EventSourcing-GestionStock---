import React from 'react';
import CIcon from '@coreui/icons-react';
import {
  cilBuilding,
  cilContact,
  cilAccountLogout,
  cilUser,
  cilHome,
  cilEnvelopeOpen,
  cilDataTransferDown, // Replacing cilDiamond with a data-related icon
  cilCart, // Added cart icon for Commandes
} from '@coreui/icons';
import { CNavItem } from '@coreui/react';
import { FaFileExcel } from 'react-icons/fa'; // Import for the Excel icon
import { cibProductHunt } from '@coreui/icons'; // Import the icon used earlier

const iconStyle = {}; // Keep icon style as it is defined globally or adjust based on your needs

const _nav = [
  {
    component: CNavItem,
    name: 'Accueil',
    to: '/dashboard',
    icon: <CIcon icon={cilHome} customClassName="nav-icon" style={iconStyle} />,
    badge: {
      color: 'info',
    },
  },
  {
    component: CNavItem,
    name: 'Commandes',
    to: '/commandes',
    icon: <CIcon icon={cilCart} customClassName="nav-icon" style={iconStyle} />, 
    badge: {
      color: 'info',
    },
  },
  {
    component: CNavItem,
    name: 'Login',
    to: '/login',
    icon: <CIcon icon={cilUser} customClassName="nav-icon" style={iconStyle} />,
    badge: {
      color: 'info',
    },
  },
  {
    component: CNavItem,
    name: 'FM1',
    to: '/products',
    icon: <CIcon icon={cibProductHunt} customClassName="nav-icon" style={iconStyle} />,
    badge: {
      color: 'info',
    },
  },
   {
     component: CNavItem,
   name: 'ExcelComposents',
     to: '/excelComposents',
     icon: <CIcon icon={cilDataTransferDown} customClassName="nav-icon" style={iconStyle} />, // Changed to a data-related icon
     badge: {
      color: 'info',
     },
  },
  {
    component: CNavItem,
     name: 'FM1Data',
    to: '/fm1Data',
    icon: <CIcon icon={cilDataTransferDown} customClassName="nav-icon" style={iconStyle} />, // Changed to a data-related icon
     badge: {
       color: 'info',
    },
  },
  {
    component: CNavItem,
    name: 'Logout',
    to: '/logout',
    icon: <CIcon icon={cilAccountLogout} customClassName="nav-icon" style={iconStyle} />,
    badge: {
      color: 'info',
    },
  },
];

export default _nav;
