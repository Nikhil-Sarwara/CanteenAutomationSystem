// In: src/apiClient.js
import axios from "axios";

const apiClient = axios.create({
  baseURL: "http://localhost:5154", // Your backend URL
});

// Add a request interceptor to include the token in headers
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default apiClient;
