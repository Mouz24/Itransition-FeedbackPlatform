import React from 'react';
import { LoginSocialFacebook } from 'reactjs-social-login';
import { FacebookLoginButton } from 'react-social-login-buttons';
import axiosInstance from './AxiosInstance';
import { useUserContext } from './UserContext';
import { useNavigate } from 'react-router-dom';
import { LoadingProps } from './Props/LoadingProps';

const FacebookOAuthButton: React.FC<LoadingProps> = ({isLoading, setIsLoading}) => {
  const { setLoggedInUser } = useUserContext();
  const navigate = useNavigate();

  const onSuccess = async (response: any) => {
    if (isLoading) return;
    setIsLoading(true);

    const userCredentials = await axiosInstance.post('external-login', {
      provider: 'Facebook',
      idToken: response.data.accessToken,
      email: response.data.email
    });

    const { accessToken, refreshToken, role, userName, userId } =
      userCredentials.data;

    console.log(userCredentials.data);

    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);

    setLoggedInUser({
      role: role,
      userName: userName,
      id: userId,
      avatar: response.picture.data.url 
    });

    setIsLoading(false);
    navigate('/');
  };

  const onFailure = (error: Error) => {
    console.error('Facebook OAuth error:', error);
  };

  return (
    <div>
      <LoginSocialFacebook
        appId="680857580579141"
        onResolve={onSuccess}
        onReject={() => onFailure}
      >
        <FacebookLoginButton size='39px'/>
      </LoginSocialFacebook>
    </div>
  );
};

export default FacebookOAuthButton;
