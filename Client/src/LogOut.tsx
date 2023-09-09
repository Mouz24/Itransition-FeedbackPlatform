import React from 'react';
import { useNavigate } from 'react-router-dom';
import axiosInstance from './AxiosInstance'
import { Button } from '@mui/material';

const LogoutButton: React.FC = () => {
  const navigate = useNavigate();

  const accessToken = localStorage.getItem("accessToken");

  const handleLogout = async () => {
    await axiosInstance.post(
        'token/revoke',
        { accessToken },
        {
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    navigate('/');
    window.location.reload();
  };

  return (
    <Button onClick={() => handleLogout()} size="medium" variant="outlined" color="error" style={{ flexShrink: 0 }}>
      Log out
    </Button>
  );
};

export default LogoutButton;