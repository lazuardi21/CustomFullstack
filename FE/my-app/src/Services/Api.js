// services/api.js
import axios from 'axios';
import Cookies from 'js-cookie';

const API_URL = 'http://localhost:5240/api/v1/Users'; // Replace with your API base URL

const axiosInstance = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = Cookies.get('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export const login = async (email, password) => {
  try {
    const response = await axiosInstance.post('/login', { email, password });
    const { data } = response;
    if (data.token) {
      Cookies.set('token', data.token, { expires: 7 }); // Store token in cookie with expiration (7 days)
    }
    return data;
  } catch (error) {
    throw error;
  }
};

// Add more API functions as needed

export default axiosInstance;
