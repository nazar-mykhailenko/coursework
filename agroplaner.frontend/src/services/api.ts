import axios from 'axios';

// Create axios instance with default config
const api = axios.create({
  baseURL: 'http://localhost:5105/api', // Updated to match your backend URL
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor for adding auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for handling unauthorized errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Handle unauthorized errors (status 401)
    if (error.response && error.response.status === 401) {
      // Clear token from localStorage
      localStorage.removeItem('token');
        // Dispatch logout action to update Redux state
      // We need to import store dynamically to avoid circular dependencies
      import('../store/index').then(({ store }) => {
        import('../store/slices/authSlice').then(({ logout }) => {
          store.dispatch(logout());
        });
      });
      
      // Redirect to login page
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
