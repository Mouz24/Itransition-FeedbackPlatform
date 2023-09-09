import { isAxiosError } from 'axios';
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axiosInstance from './AxiosInstance';
import { Box, Button, TextField } from '@mui/material';
import { LoadingProps } from './Props/LoadingProps';

interface RegistrationFormData {
  email: string;
  username: string;
  password: string;
  error: string;
  role: string[];
}

const RegistrationForm: React.FC<LoadingProps> = ({isLoading, setIsLoading}) => {
  const [formData, setFormData] = useState<RegistrationFormData>({
    username: '',
    email: '',
    password: '',
    role: ['User'],
    error: '',
  });

  const navigate = useNavigate();

  const [fieldErrors, setFieldErrors] = useState({
    username: '',
    email: '',
    password: '',
  });

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setFormData((prevFormData) => ({
      ...prevFormData,
      [name]: value,
      error: '',
    }));

    setFieldErrors((prevFieldErrors) => ({
      ...prevFieldErrors,
      [name]: '',
    }));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (isLoading) return;
    setIsLoading(true);

    try {
      await axiosInstance.post('signup', formData);
      setIsLoading(false);
      navigate('/authorization-page/login');
    } catch (error: any) {
      if (isAxiosError(error)) {
        if (error.response) {
          const responseData = error.response.data;
          if (responseData) {
            setFieldErrors({
              username: responseData.UserName?.[0] || responseData.DuplicateUserName?.[0] || '',
              email: responseData.Email?.[0] || responseData.DuplicateEmail?.[0] || '',
              password: responseData.Password?.[0] 
              || responseData.PasswordTooShort?.[0] 
              || responseData.PasswordRequiresDigit?.[0] 
              || ''
            });
          }
        } else {
          console.error('Unknown Error:', 'An unknown error occurred during registration.');
          setFormData((prevFormData) => ({ ...prevFormData, error: 'An unknown error occurred during registration.' }));
        }
      }
    }
  };

  return (
    <Box
      component="form"
      sx={{
        '& .MuiTextField-root': { m: 1, width: '25ch' },
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
      }}
      noValidate
      autoComplete="on"
      onSubmit={handleSubmit}
    >
      <div>
        <TextField
          label={fieldErrors.username ? 'Error' : 'Username'}
          variant="outlined"
          type="text"
          name="username"
          value={formData.username}
          onChange={handleChange}
          error={Boolean(fieldErrors.username)}
          helperText={fieldErrors.username || ''}
        />
      </div>
      <div>
        <TextField
          label={fieldErrors.email ? 'Error' : 'Email'}
          variant="outlined"
          type="text"
          name="email"
          value={formData.email}
          onChange={handleChange}
          error={Boolean(fieldErrors.email)}
          helperText={fieldErrors.email || ''}
        />
      </div>
      <div>
        <TextField
          label={fieldErrors.password ? 'Error' : 'Password'}
          variant="outlined"
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          error={Boolean(fieldErrors.password)}
          helperText={fieldErrors.password || ''}
        />
      </div>
      <div>
        <Button variant="contained" color="success" type="submit">
          Sign up
        </Button>
      </div>
    </Box>
  );
};

export default RegistrationForm;

