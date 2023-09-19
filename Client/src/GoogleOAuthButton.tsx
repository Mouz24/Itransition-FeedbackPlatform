import React from 'react';
import { GoogleLogin, GoogleOAuthProvider } from '@react-oauth/google';
import axiosInstance from './AxiosInstance';
import { useUserContext } from './UserContext';
import { useNavigate } from 'react-router-dom';
import { LoadingProps } from './Props/LoadingProps';

const GoogleOAuthButton: React.FC<LoadingProps> = ({isLoading, setIsLoading}) => {
  const { setLoggedInUser } = useUserContext();
  const navigate = useNavigate();

  const onSuccess = async (response : any) => {
    if (isLoading) return;
    setIsLoading(true);

    const userCredentials = await axiosInstance.post('external-login', {
      provider: 'Google', 
      idToken: response.credential 
    });

    const { accessToken, refreshToken, role, userName, userId, avatar } = userCredentials.data;

    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);

    setLoggedInUser({
      userName: userName,
      id: userId,
      role: role,
      avatar: avatar,
      isDarkMode: false
    });

    setIsLoading(false);
    navigate('/');
  };

  const onFailure = (error: Error) => {
    console.error('Google OAuth error:', error);
  };

  return (
    <div>
      <GoogleOAuthProvider clientId='880479689572-svhuujvi54jpk9ku83e4guqicfb81j38.apps.googleusercontent.com'>
      <GoogleLogin
        onSuccess={onSuccess}
        onError={() => onFailure}
        useOneTap
      />
      </GoogleOAuthProvider>
    </div>
  );
};

export default GoogleOAuthButton;
