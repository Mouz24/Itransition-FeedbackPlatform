import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';
import { Route, Routes, BrowserRouter } from 'react-router-dom';
import LoginForm from './LoginForm';
import RegistrationForm from './RegistrationForm';
import AuthorizationPage from './AuthorizationPage';
import { useUserContext } from './UserContext';
import Header from './Header';
import { Container } from '@mui/material';
import MainPage from './MainPage';
import UserReviews from './UserReviews';
import UserReview from './UserReview';
import ReviewManipulation from './AddReview';
import EditReview from './EditReview';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { deepOrange, grey } from '@mui/material/colors';
import { darkModeTheme } from './Themes';
import { existingTheme } from './Themes';

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
        </Routes >
        </Container>
        </BrowserRouter>
      </ThemeProvider>
  );
}

export default App;
