import axios, { isAxiosError } from 'axios';
import React, { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axiosInstance from './AxiosInstance';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import { LoadingProps } from './Props/LoadingProps';
import { useUserContext } from './UserContext';

interface LoginData {
  formData: LoginFormValues;
}

interface LoginFormValues {
  username: string;
  password: string;
  error: string;
}

const LoginForm: React.FC<LoadingProps> = ({isLoading, setIsLoading}) => {
  const [formData, setFormData] = useState<LoginFormValues>({
    username: '',
    password: '',
    error: ''
  });

  const navigate = useNavigate();
  const { setLoggedInUser } = useUserContext();

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setFormData((prevFormData) => ({
      ...prevFormData,
      [name]: value,
      error: '',
    }));
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (isLoading) return;

    setIsLoading(true);

    const { username, password } = formData;
    if (!username || !password) {
      setFormData((prevFormData) => ({ ...prevFormData, error: 'Please enter both username and password.' }));
      return;
    }
     try {
      const response = await axiosInstance.post('login', formData);
      const { accessToken, refreshToken, role, userName, userId, avatar } = response.data;
      
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('refreshToken', refreshToken);

      setLoggedInUser({
        username: userName,
        id: userId,
        role: role,
        avatar: avatar
      });

      setIsLoading(false);
      navigate('/');
      } catch (error) {
        if (axios.isAxiosError(error) && error.response && error.response.status === 401){
            setFormData((prevFormData) => ({ ...prevFormData, error: 'Incorrect username or password.'}));
        } else {
            setFormData((prevFormData) => ({ ...prevFormData, error: 'An error occurred during login.'}));
        }
      }
  };

  return (
    <Box component="form" sx={{
      '& .MuiTextField-root': { m: 1, width: '25ch' }, 
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center'
    }}
    noValidate autoComplete="on" 
    onSubmit={handleSubmit}>
      <div>
      <TextField
        label={formData.error ? 'Error' : 'Username'}
        variant="outlined"
        type="text"
        name="username"
        value={formData.username}
        onChange={handleChange}
        error={Boolean(formData.error)}
        helperText={formData.error || ''}
      />
      </div>
      <div>
        <TextField
          label={formData.error ? 'Error' : 'Password'}
          variant="outlined"
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          error={Boolean(formData.error)}
          helperText={formData.error || ''}
        />
      </div>
      <div>
        <Button variant="contained" color="success" type="submit">
          Log in
        </Button>
      </div>
    </Box>
  );
};

export default LoginForm;