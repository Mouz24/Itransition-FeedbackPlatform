import React from 'react';
import ReactDOM from 'react-dom';
import { UserProvider } from './components/UserContext';
import App from './App';

ReactDOM.render(
    <UserProvider>
      <App />
    </UserProvider>,
  
  document.getElementById('root')
);
