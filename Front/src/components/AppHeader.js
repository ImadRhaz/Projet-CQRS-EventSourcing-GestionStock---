import React, { useEffect, useRef, useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';
import {
  CContainer,
  CDropdown,
  CDropdownItem,
  CDropdownMenu,
  CDropdownToggle,
  CHeader,
  CHeaderNav,
  CHeaderToggler,
  CNavLink,
  CNavItem,
  useColorModes,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import {
  cilBell,
  cilContrast,
  cilEnvelopeOpen,
  cilList,
  cilSettings,
  cilMenu,
  cilMoon,
  cilSun,
} from '@coreui/icons';
import { AppHeaderDropdown } from './header/index';
import './style.css';
import { Link } from 'react-router-dom';
import axios from 'axios';
import { BASE_URL } from '../config';
import { jwtDecode } from 'jwt-decode';

const AppHeader = () => {
  const headerRef = useRef();
  const navLinkRef = useRef();
  const { colorMode, setColorMode } = useColorModes('coreui-free-react-admin-template-theme');
  const dispatch = useDispatch();
  const sidebarShow = useSelector((state) => state.sidebarShow);
  const [notifications, setNotifications] = useState([]);
  const [unreadNotifications, setUnreadNotifications] = useState(0);
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const navigate = useNavigate();
  const [allNotifications, setAllNotifications] = useState([]);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userInfos, setUserInfos] = useState(null);
  const refreshIntervalRef = useRef(null);

  // Fonction pour charger les notifications
  const fetchNotifications = async () => {
    try {
      const response = await axios.get(`${BASE_URL}Notifications`);
      setAllNotifications(response.data);
      filterNotifications(response.data, userInfos?.role);
    } catch (error) {
      console.error('Erreur lors de la récupération des notifications:', error);
    }
  };

  // Configurer l'intervalle de rafraîchissement toutes les 10 secondes
  useEffect(() => {
    if (isAuthenticated) {
      // Premier chargement immédiat
      fetchNotifications();

      // Configurer l'intervalle
      refreshIntervalRef.current = setInterval(fetchNotifications, 10000);
    }

    return () => {
      if (refreshIntervalRef.current) {
        clearInterval(refreshIntervalRef.current);
      }
    };
  }, [isAuthenticated, userInfos?.role]);

  useEffect(() => {
    document.addEventListener('scroll', () => {
      headerRef.current &&
        headerRef.current.classList.toggle('shadow-sm', document.documentElement.scrollTop > 0);
    });
  }, []);

  useEffect(() => {
    const fetchUserInfos = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const decodedToken = jwtDecode(token);
          const userId = decodedToken.nameid || decodedToken.sub;
          const userRole = decodedToken.role;

          if (!userId) {
            throw new Error('ID utilisateur non trouvé dans le token.');
          }

          setUserInfos({ id: userId, role: userRole });
          setIsAuthenticated(true);
        } catch (error) {
          console.error('Erreur lors du décodage du token:', error);
          setIsAuthenticated(false);
        }
      } else {
        setIsAuthenticated(false);
      }
    };

    fetchUserInfos();
  }, []);

  const filterNotifications = (notifications, role) => {
    const filtered = notifications.filter(notification => {
      if (role === 'Magasinier') {
        return notification.message.startsWith('Une nouvelle commande a été créée (ID');
      } else if (role === 'Expert') {
        return notification.message.startsWith('La commande #');
      }
      return true;
    });

    const newCommandNotifications = filtered.filter(n => n.message.startsWith('Une nouvelle commande a été créée (ID'))
      .sort((a, b) => {
        const numA = parseInt(a.message.match(/\(ID: (\d+)\)/)?.[1] || '0', 10);
        const numB = parseInt(b.message.match(/\(ID: (\d+)\)/)?.[1] || '0', 10);
        return numB - numA;
      });

    const valideNotifications = filtered.filter(n => n.message.includes("a été validée"))
      .sort((a, b) => {
        const numA = parseInt(a.message.match(/#(\d+)/)?.[1] || '0', 10);
        const numB = parseInt(b.message.match(/#(\d+)/)?.[1] || '0', 10);
        return numB - numA;
      });

    const unreadAndOtherNotifications = filtered.filter(n =>
      !n.isRead && !n.message.startsWith('Une nouvelle commande a été créée (ID') && !n.message.includes("a été validée")
    ).sort((a, b) => b.id - a.id);

    const readAndOtherNotifications = filtered.filter(n =>
      n.isRead && !n.message.startsWith('Une nouvelle commande a été créée (ID') && !n.message.includes("a été validée")
    );

    const sortedNotifications = [...newCommandNotifications, ...valideNotifications, ...unreadAndOtherNotifications, ...readAndOtherNotifications];

    // *AFTER* sorting, calculate the number of unread notifications.
    const unreadCount = sortedNotifications.filter(n => !n.isRead).length;

    setNotifications(sortedNotifications);
    setUnreadNotifications(unreadCount); // Use the count calculated *after* sorting
  };

  const toggleDropdown = async () => {
    setDropdownOpen(!dropdownOpen);

    if (!dropdownOpen) {
      try {
        await axios.put(`${BASE_URL}Notifications/markAsRead`);
        setAllNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
        setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
      } catch (error) {
        console.error("Erreur lors du marquage des notifications comme lues :", error);
      }
    }
  };

  const handleNotificationClick = async (notification) => {
    try {
      await axios.put(`${BASE_URL}Notifications/markAsRead/${notification.id}`);
      setAllNotifications(prev => prev.map(n => n.id === notification.id ? { ...n, isRead: true } : n));
    } catch (error) {
      console.error('Erreur lors du marquage de la notification comme lue :', error);
    }
  };

  useEffect(() => {
    filterNotifications(allNotifications, userInfos?.role);
  }, [allNotifications, userInfos?.role]);

  return (
    <CHeader
      position="sticky"
      className={`p-0 ${colorMode === 'dark' ? 'header-dark' : ''}`}
      ref={headerRef}
      style={{ backgroundColor: colorMode === 'dark' ? 'black' : '#f3f4f7' }}
    >
      <CContainer className="border-bottom px-4" fluid>
        <CHeaderToggler
          onClick={() => dispatch({ type: 'set', sidebarShow: !sidebarShow })}
          style={{ marginInlineStart: '-14px' }}
        >
          <CIcon icon={cilMenu} size="lg" />
        </CHeaderToggler>
        <CHeaderNav className="d-none d-md-flex">
          <CNavItem>
            <CNavLink to="/dashboard" as={NavLink}>
              Accueil
            </CNavLink>
          </CNavItem>
        </CHeaderNav>
        <CHeaderNav className="ms-auto">
          {isAuthenticated && (
            <CDropdown variant="nav-item" placement="bottom-end" isOpen={dropdownOpen} toggle={toggleDropdown}>
              <CDropdownToggle caret={false} onClick={toggleDropdown}>
                <CIcon icon={cilBell} size="lg" />
                {unreadNotifications > 0 && (
                  <span className={`notification-counter active`}>
                    {unreadNotifications}
                  </span>
                )}
              </CDropdownToggle>
              <CDropdownMenu style={{ maxHeight: '300px', overflowY: 'scroll' }}>
                {notifications.length === 0 ? (
                  <CDropdownItem header="true">Pas de notifications</CDropdownItem>
                ) : (
                  notifications.map((notification) => (
                    <CDropdownItem
                      key={notification.id}
                      onClick={() => handleNotificationClick(notification)}
                    >
                      <div className="notification-item">
                        <div style={{ fontWeight: notification.isRead ? 'normal' : 'bold' }}>
                          {notification.message}
                        </div>
                        <div className="notification-date">
                          {new Date(notification.createdAt).toLocaleString()}
                        </div>
                      </div>
                    </CDropdownItem>
                  ))
                )}
              </CDropdownMenu>
            </CDropdown>
          )}
          <CNavItem>
            <CNavLink href="#" ref={navLinkRef}>
              <CIcon icon={cilList} size="lg" />
            </CNavLink>
          </CNavItem>
          <CNavItem>
            <CNavLink href="#">
              <CIcon icon={cilEnvelopeOpen} size="lg" />
            </CNavLink>
          </CNavItem>
          {isAuthenticated && (
            <CNavItem>
              <CDropdown variant="nav-item">
                <CDropdownToggle caret={false}>
                  <CIcon icon={cilSettings} size="lg" />
                </CDropdownToggle>
                <CDropdownMenu>
                  <CDropdownItem className="dropdown-item">
                    <Link to="/updatePassword" className="dropdown-link">
                      Modifier Mot de passe
                    </Link>
                  </CDropdownItem>
                </CDropdownMenu>
              </CDropdown>
            </CNavItem>
          )}
        </CHeaderNav>

        <CHeaderNav>
          <li className="nav-item py-1">
            <div className="vr h-100 mx-2 text-body text-opacity-75"></div>
          </li>
          <CDropdown variant="nav-item" placement="bottom-end">
            <CDropdownToggle caret={false}>
              {colorMode === 'dark' ? (
                <CIcon icon={cilMoon} size="lg" />
              ) : colorMode === 'auto' ? (
                <CIcon icon={cilContrast} size="lg" />
              ) : (
                <CIcon icon={cilSun} size="lg" />
              )}
            </CDropdownToggle>
            <CDropdownMenu>
              <CDropdownItem
                active={colorMode === 'light'}
                className="d-flex align-items-center"
                as="button"
                type="button"
                onClick={() => setColorMode('light')}
              >
                <CIcon className="me-2" icon={cilSun} size="lg" /> Light
              </CDropdownItem>
              <CDropdownItem
                active={colorMode === 'dark'}
                className="d-flex align-items-center"
                as="button"
                type="button"
                onClick={() => setColorMode('dark')}
              >
                <CIcon className="me-2" icon={cilMoon} size="lg" /> Dark
              </CDropdownItem>
              <CDropdownItem
                active={colorMode === 'auto'}
                className="d-flex align-items-center"
                as="button"
                type="button"
                onClick={() => setColorMode('auto')}
              >
                <CIcon className="me-2" icon={cilContrast} size="lg" /> Auto
              </CDropdownItem>
            </CDropdownMenu>
          </CDropdown>
          <li className="nav-item py-1">
            <div className="vr h-100 mx-2 text-body text-opacity-75"></div>
          </li>
          <AppHeaderDropdown />
        </CHeaderNav>
      </CContainer>
    </CHeader>
  );
};

export default AppHeader;