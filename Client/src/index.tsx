import React from 'react';
import ReactDOM from 'react-dom';
import { UserProvider } from './UserContext';
import App from './App';
import { Box, Container } from '@mui/material';

ReactDOM.render(
  <React.StrictMode>
    <UserProvider>
      <App />
    </UserProvider>
  </React.StrictMode>,
  
  document.getElementById('root')
);
