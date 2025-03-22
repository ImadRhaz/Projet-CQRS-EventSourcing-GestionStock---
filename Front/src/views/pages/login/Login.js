import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

// CoreUI components
import {
  CButton,
  CCard,
  CCardBody,
  CCardGroup,
  CCol,
  CContainer,
  CForm,
  CFormInput,
  CInputGroup,
  CInputGroupText,
  CRow,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilLockLocked, cilUser } from '@coreui/icons';
import { BASE_URL } from '../../../config';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [twoFactorCode, setTwoFactorCode] = useState(''); // New state for 2FA code
  const [errorMessage, setErrorMessage] = useState('');
  const [twoFactorRequired, setTwoFactorRequired] = useState(false); // New state for 2FA requirement

  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();

    const data = { email, password };

    try {
      const response = await axios.post(`${BASE_URL}account/login`, data, { // Changed endpoint
        headers: {
          'Content-Type': 'application/json',
        },
      });

      console.log("Login Response:", response.data);

      if (response.status === 200) {
        if (response.data.twoFactorRequired) {
          // 2FA required, show 2FA input
          setTwoFactorRequired(true);
        } else if (response.data.token) {
          // Login successful, store token and navigate
          const token = response.data.token;
          const decodedToken = jwtDecode(token);

          console.log("Decoded Token:", decodedToken);

          localStorage.setItem('idUser', decodedToken.nameid);
          localStorage.setItem('token', token);

          navigate('/dashboard');
        } else {
          setErrorMessage('Unexpected response from server.');
        }
      } else {
        setErrorMessage('Login failed. Please check your credentials.');
      }
    } catch (error) {
      console.error("Login Error:", error);
      setErrorMessage(error.response?.data || 'Login failed. Please try again.');
    }
  };

  const handleTwoFactorSubmit = async (e) => {
    e.preventDefault();

    const data = {
      email: email, // Assuming you still have access to the email state
      twoFactorCode: twoFactorCode,
    };

    try {
      const response = await axios.post(`${BASE_URL}account/login-2fa`, data, { // Changed endpoint
        headers: {
          'Content-Type': 'application/json',
        },
      });

      console.log("2FA Response:", response.data);

      if (response.status === 200) {
        const token = response.data.token;
        const decodedToken = jwtDecode(token);

        console.log("Decoded Token:", decodedToken);

        localStorage.setItem('idUser', decodedToken.nameid);
        localStorage.setItem('token', token);

        navigate('/dashboard');
      } else {
        setErrorMessage('Invalid 2FA code. Please try again.');
      }
    } catch (error) {
      console.error("2FA Error:", error);
      setErrorMessage(error.response?.data || '2FA authentication failed.');
    }
  };

  return (
    <div className="bg-body-tertiary min-vh-100 d-flex flex-row align-items-center">
      <CContainer>
        <CRow className="justify-content-center">
          <CCol md={8}>
            <CCardGroup>
              <CCard className="p-4">
                <CCardBody>

                  {!twoFactorRequired ? (
                    <CForm onSubmit={handleLogin}>
                      <h1>Login</h1>
                      <p className="text-body-secondary">Sign In to your account</p>
                      {errorMessage && (
                        <div className="alert alert-danger" role="alert">
                          {errorMessage}
                        </div>
                      )}
                      <CInputGroup className="mb-3">
                        <CInputGroupText>
                          <CIcon icon={cilUser} />
                        </CInputGroupText>
                        <CFormInput
                          placeholder="Email"
                          autoComplete="email"
                          value={email}
                          onChange={(e) => setEmail(e.target.value)}
                          required
                        />
                      </CInputGroup>
                      <CInputGroup className="mb-4">
                        <CInputGroupText>
                          <CIcon icon={cilLockLocked} />
                        </CInputGroupText>
                        <CFormInput
                          type="password"
                          placeholder="Password"
                          autoComplete="current-password"
                          value={password}
                          onChange={(e) => setPassword(e.target.value)}
                          required
                        />
                      </CInputGroup>
                      <CRow>
                        <CCol xs={6}>
                          <CButton type="submit" color="primary" className="px-4">
                            Login
                          </CButton>
                        </CCol>
                        <CCol xs={6} className="text-right">
                          <Link to="/forgotPassword">
                            <CButton color="link" className="px-0">
                              Forgot password?
                            </CButton>
                          </Link>
                        </CCol>
                      </CRow>
                    </CForm>
                  ) : (
                    // Render the 2FA form if 2FA is required
                    <CForm onSubmit={handleTwoFactorSubmit}>
                      <h1>Two-Factor Authentication</h1>
                      <p className="text-body-secondary">Enter your authentication code</p>
                      {errorMessage && (
                        <div className="alert alert-danger" role="alert">
                          {errorMessage}
                        </div>
                      )}
                      <CInputGroup className="mb-3">
                        <CInputGroupText>
                          <CIcon icon={cilLockLocked} />
                        </CInputGroupText>
                        <CFormInput
                          type="text"
                          placeholder="Authentication Code"
                          value={twoFactorCode}
                          onChange={(e) => setTwoFactorCode(e.target.value)}
                          required
                        />
                      </CInputGroup>
                      <CRow>
                        <CCol xs={12}>
                          <CButton type="submit" color="primary" className="px-4">
                            Submit
                          </CButton>
                        </CCol>
                      </CRow>
                    </CForm>
                  )}
                </CCardBody>
              </CCard>

              <CCard className="text-white bg-primary py-5" style={{ width: '44%' }}>
                <CCardBody className="text-center">
                  <div>
                    <h2>Sign up</h2>
                    <p>This site allows you to effectively manage Your FM1 as well as your Stock.</p>
                    <Link to="/register">
                      <CButton color="primary" className="mt-3" active tabIndex={-1}>
                        Register Now!
                      </CButton>
                    </Link>
                  </div>
                </CCardBody>
              </CCard>
            </CCardGroup>
          </CCol>
        </CRow>
      </CContainer>
    </div>
  );
};

export default Login;