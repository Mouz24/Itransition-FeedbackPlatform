import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';
import { Route, Routes, HashRouter } from 'react-router-dom';
import LoginForm from './LoginForm';
import RegistrationForm from './RegistrationForm';
import AuthorizationPage from './AuthorizationPage';
import { useUserContext } from './UserContext';
import Header from './Header';
import { Container } from '@mui/material';
import MainPage from './MainPage';
import Header2 from './Header2';
import UserReviews from './UserReviews';
import UserReview from './UserReview';

function App() {
  const [isLogin, setIsLogin] = useState<boolean>(false)

  return (
    <>
      <Header2 setIsLogin={setIsLogin} isLogin={isLogin}/>
      <Routes>
        <Route path="/" element={ <MainPage />} />
        <Route path="/:userId/reviews" element={<UserReviews/>}/>
        <Route path="/:userId/reviews/:reviewId" element={<UserReview/>}/>
        <Route path="/authorization-page/*" element={<AuthorizationPage/>}/>
      </Routes >
      </>
  );
}

export default App;
