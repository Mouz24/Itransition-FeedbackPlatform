import React, { useState } from 'react';
import logo from './logo.svg';
import './styles/App.css';
import { Route, Routes, BrowserRouter } from 'react-router-dom';
import LoginForm from './components/LoginForm';
import RegistrationForm from './components/RegistrationForm';
import AuthorizationPage from './pages/AuthorizationPage';
import { useUserContext } from './components/UserContext';
import Header from './components/Header';
import { Container } from '@mui/material';
import MainPage from './pages/MainPage';
import UserReviews from './pages/UserReviews';
import UserReview from './pages/UserReview';
import ReviewManipulation from './pages/AddReview';
import EditReview from './pages/EditReview';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { deepOrange, grey } from '@mui/material/colors';
import { darkModeTheme } from './styles/Themes';
import { existingTheme } from './styles/Themes';
import UserManagementTable from './pages/UserManagementTable';
import AvatarUpload from './pages/AvatarUpload';

function App() {
  const [isLogin, setIsLogin] = useState<boolean>(false);
  const { loggedInUser } = useUserContext();
  
  const getTheme = () => {
    return loggedInUser?.isDarkMode ? darkModeTheme : existingTheme;
  };

  return (
    <ThemeProvider theme={getTheme()}>
      <CssBaseline />
      <BrowserRouter>
        <Header setIsLogin={setIsLogin} isLogin={isLogin}/>
        <Container maxWidth='xl'>
        <Routes>
          <Route path="/" element={ <MainPage/>} />
          <Route path="/:userId/reviews" element={<UserReviews/>}/>
          <Route path="/:userId/reviews/:reviewId" element={<UserReview/>}/>
          <Route path="/:userId/reviews/:reviewId/edit" element={<EditReview/>}/>
          <Route path="/:userId/add-review" element={<ReviewManipulation/>}/>
          <Route path="/authorization-page/*" element={<AuthorizationPage/>}/>
          <Route path="/admin-panel" element={<UserManagementTable />}/>
          <Route path="/avatar-upload" element={<AvatarUpload />}/>
        </Routes >
        </Container>
        </BrowserRouter>
      </ThemeProvider>
  );
}

export default App;
