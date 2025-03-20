import React, { Suspense, useEffect } from 'react';
import { HashRouter, Route, Routes } from 'react-router-dom';
import { useSelector } from 'react-redux';

import { CSpinner, useColorModes } from '@coreui/react';
import './scss/style.scss';
// import CommandeEnGrosAssociate from "src/views/commandeengrosassociate/CommandeEnGrosAssociate";
// import CommandeEnGrosList from "src/views/commande-en-gros/CommandeEnGrosList";

// Containers
const DefaultLayout = React.lazy(() => import('./layout/DefaultLayout'));

// Pages

const Login = React.lazy(() => import('./views/pages/login/Login'));
const Register = React.lazy(() => import('./views/pages/register/Register'));
const Logout = React.lazy(() => import('./components/Logout'));
const ForgotPass = React.lazy(() => import('./views/pages/forgotPassword/ForgotPasswordForm'));
// Dashboard
const Dashboard = React.lazy(() => import('./views/dashboard/Dashboard'));

const App = () => {
  const { isColorModeSet, setColorMode } = useColorModes('coreui-free-react-admin-template-theme');
  const storedTheme = useSelector((state) => state.theme);

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.href.split('?')[1]);
    const theme = urlParams.get('theme') && urlParams.get('theme').match(/^[A-Za-z0-9\s]+/)[0];
    if (theme) {
      setColorMode(theme);
    }

    if (isColorModeSet()) {
      return;
    }

    setColorMode(storedTheme);
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  return (
    <HashRouter>
      <Suspense
        fallback={
          <div className="pt-3 text-center">
            <CSpinner color="primary" variant="grow" />
          </div>
        }
      >
        <Routes>
          {/* <Route exact path="/login" name="Login Page" element={<Login />} /> */}
          {/* <Route exact path="/register" name="Register Page" element={<Register />} /> */}
          {/* <Route path="/associate/:orderId" element={<CommandeEnGrosAssociate />} /> */}
          <Route exact path="/logout" name="Logout" element={<Logout />} />
          {/* <Route exact path="/forgotPassword" name="ForgotPassword" element={<ForgotPass />} /> */}
          {/* Unprotected Route for Dashboard */}

          {/* Protected Routes */}
          <Route
            path="*"
            name="Home"
            element={
              <Suspense fallback={<div>Loading...</div>}>
                <DefaultLayout />
              </Suspense>
            }
          />
        </Routes>
      </Suspense>
    </HashRouter>

  );
};

export default App;
