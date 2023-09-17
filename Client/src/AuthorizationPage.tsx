import React, { useState } from 'react';
import { Route, Routes, Link, Outlet } from 'react-router-dom';
import LoginForm from './LoginForm';
import RegistrationForm from './RegistrationForm';
import Container from '@mui/material/Container';
import Button from '@mui/material/Button';
import { styled } from '@mui/material/styles';
import GoogleOAuthButton from './GoogleOAuthButton';
import FacebookOAuthButton from './FacebookOAuthButton';
import { Box, CircularProgress } from '@mui/material';

const CenteredContainer = styled(Container)({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  height: '100vh',
});

const ButtonContainer = styled('div')({
  display: 'flex',
  flexDirection: 'row',
  gap: '30px',
  alignItems: 'center',
  justifyContent: 'center',
  listStyle: 'none',
  padding: 0,
  margin: 0,
});

const AuthorizationPage: React.FC = () => {
  const [isLoading, setIsLoading] = useState<boolean>(false);

  return (
    <CenteredContainer>
      {isLoading ? (
        <CircularProgress />
      ) : (
        <>
          <div>
            <nav>
              <ul style={{ paddingInlineStart: 0 }}>
                <ButtonContainer>
                  <li>
                    <Button
                      component={Link}
                      to="./login"
                      size="medium"
                      variant="outlined"
                      color="success"
                      sx={{ ":hover": { bgcolor: "#2e7d32", color: 'white' } }}
                    >
                      Log in
                    </Button>
                  </li>
                  <li>
                    <Button
                      component={Link}
                      to="./signup"
                      size="medium"
                      variant="outlined"
                      color="success"
                      sx={{ ":hover": { bgcolor: "#2e7d32", color: 'white' } }}
                    >
                      Sign up
                    </Button>
                  </li>
                </ButtonContainer>
              </ul>
            </nav>
          </div>

          <Routes>
            <Route path="login" element={<LoginForm />} />
            <Route path="signup" element={<RegistrationForm />} />
          </Routes>
          <Box sx={{ display: 'flex', alignItems: 'center', marginTop: '10px' }}>
            <FacebookOAuthButton isLoading={isLoading} setIsLoading={setIsLoading}/>
            <GoogleOAuthButton isLoading={isLoading} setIsLoading={setIsLoading} />
          </Box>
        </>
      )}
    </CenteredContainer>
  );
};

export default AuthorizationPage;
