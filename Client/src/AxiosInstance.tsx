import axios,  { AxiosError, AxiosRequestConfig, AxiosResponse, AxiosRequestHeaders } from 'axios';
import { useUserContext } from './UserContext';

interface AdaptAxiosRequestConfig extends AxiosRequestConfig {
  headers: AxiosRequestHeaders
}

interface RefreshErrorResponse {
  response: {
    status: number;
    data: {
      error: string;
    };
  };
}

const instance = axios.create({
  baseURL: 'http://localhost:5164/api/',
});

let isRefreshing = false;
let refreshSubscribers: ((accessToken: string) => void)[] = [];

const subscribeTokenRefresh = (cb: (accessToken: string) => void) => {
  refreshSubscribers.push(cb);
};


const onRrefreshed = (accessToken: string, refreshToken: string) => {
  refreshSubscribers.forEach((cb) => cb(accessToken));
  refreshSubscribers = [];

  localStorage.setItem('accessToken', accessToken);
  localStorage.setItem('refreshToken', refreshToken);
};

const refreshAccessToken = async (): Promise<string | null> => {
  try {
    const refreshToken = localStorage.getItem('refreshToken');
    const accessToken = localStorage.getItem('accessToken');

    if (!refreshToken) {
      return null;
    }

    isRefreshing = true;

    const response = await axios.post(`http://localhost:5164/api/token/refresh`, { accessToken , refreshToken } );
    const newAccessToken = response.data.accessToken;
    const newRefreshToken = response.data.refreshToken;

    isRefreshing = false;
    onRrefreshed(newAccessToken, newRefreshToken);

    return newAccessToken;
  } catch (error: any) {
    console.error('Error refreshing access token:', error);

    if (error.response?.status === 400 && error.response?.data === 'Invalid client request') {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      localStorage.removeItem('user');
      window.location.href = '/authorization-page/login';
      return null;
    }

    isRefreshing = false;
    return null;
  }
};

const getActualRefreshToken = async () => {
  const loggedInUserJSON = localStorage.getItem('user');
  let loggedInUserId = null;

    if (loggedInUserJSON) {
      loggedInUserId = JSON.parse(loggedInUserJSON).id;
    }

    const response = await axios.get(`http://localhost:5164/api/token/${loggedInUserId}`);

    return response.data.refreshToken;
}

instance.interceptors.request.use(
  async (config): Promise<AdaptAxiosRequestConfig> => {
    const accessToken = localStorage.getItem('accessToken');
    const loggedInUserJSON = localStorage.getItem('user');
    let refreshToken = null;

    if (loggedInUserJSON) {
      refreshToken = await getActualRefreshToken();

      if (!refreshToken) {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
  
        window.location.href = '/authorization-page/login';
        return Promise.reject('Refresh token is missing');
      }
    }

    if (accessToken) {
      const headers = {
        ...(config.headers as AxiosRequestHeaders),
        'Authorization': `Bearer ${accessToken}`,
      };
      config.headers = headers as AxiosRequestHeaders;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

instance.interceptors.response.use(
  async (response: AxiosResponse): Promise<AxiosResponse> => {
    return response;
  },
  async (error: AxiosError): Promise<any> => {
    console.log('Response Error:', error);  
    const originalRequest = error.config as AdaptAxiosRequestConfig;
    const refreshToken = localStorage.getItem('refreshToken');

    if (error.response && error.response.status === 401 && refreshToken && !originalRequest.headers['X-Retry']) {
      if (!isRefreshing) {
        console.log('Refreshing access token...');
        originalRequest.headers['X-Retry'] = 'true';
        const newAccessToken = await refreshAccessToken();
        delete originalRequest.headers['X-Retry'];

        if (newAccessToken) {
          console.log('Token refreshed successfully. Retrying the original request.');
          originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
          return instance(originalRequest);
        } else {
          console.log('Token refresh failed. Redirecting to login page.');
          window.location.href = '/authorization-page/login';
          return Promise.reject(error);
        }
      } else {
        console.log('Another request is already refreshing the token. Waiting for the new token.');
        return new Promise((resolve) => {
          subscribeTokenRefresh((accessToken) => {
            originalRequest.headers['Authorization'] = `Bearer ${accessToken}`;
            resolve(instance(originalRequest));
          });
        });
      }
    }

    return Promise.reject(error);
  }
);


export default instance;