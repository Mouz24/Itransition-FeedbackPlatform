import React from 'react';
import ReactDOM from 'react-dom';
import { UserProvider } from './UserContext';
import App from './App';
import { Box, Container } from '@mui/material';
import { HashRouter } from 'react-router-dom';

ReactDOM.render(
  <React.StrictMode>
    <HashRouter>
    <UserProvider>
      <App />
    </UserProvider>
    </HashRouter>
  </React.StrictMode>,
  
  document.getElementById('root')
);
