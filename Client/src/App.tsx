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

function App() {
  const [isLogin, setIsLogin] = useState<boolean>(false)

  return (
    <BrowserRouter>
      <Header setIsLogin={setIsLogin} isLogin={isLogin}/>
      <Routes>
        <Route path="/" element={ <MainPage />} />
        <Route path="/:userId/reviews" element={<UserReviews/>}/>
        <Route path="/:userId/reviews/:reviewId" element={<UserReview/>}/>
        <Route path="/:userId/reviews/:reviewId/edit" element={<EditReview/>}/>
        <Route path="/:userId/add-review" element={<ReviewManipulation/>}/>
        <Route path="/authorization-page/*" element={<AuthorizationPage/>}/>
      </Routes >
      </BrowserRouter>
  );
}

export default App;
