import axios from "axios";
import { navigate } from "../../utils/navigateHelper";
const axiosInstance = axios.create({
  baseURL: "https://localhost:7051/api", // Replace with your API base URL
  timeout: 10000, // Set a timeout for requests (in milliseconds)
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});
axiosInstance.interceptors.request.use(
  (config) => {
    // Add any custom logic before sending the request
    // For example, you can add authentication tokens here
    const tokens = localStorage.getItem("accessToken");
    if (tokens) {
      config.headers["Authorization"] = `Bearer ${tokens}`;
    }
    return config;
  },
  (error) => {
    // Handle request error
    return Promise.reject(error);
  }
);
axiosInstance.interceptors.response.use(
  (response) => {
    // Handle successful response
    return response;
  },
  (error) => {
    console.log(error);
    // Handle response error
    if (error.response.status === 401) {
      // Handle unauthorized access, e.g., redirect to login page
      console.error("Unauthorized access - redirecting to login");
      navigate("/login");
    } else if (error.response.status === 403) {
      // Handle unauthorized access, e.g., redirect to login page
      console.error("Permission Denied - redirecting to login");
    } else if (error.response.status === 404) {
      // Handle unauthorized access, e.g., redirect to login page
      console.error("Permission Denied - redirecting to login");
    } else if (error.response.status === 500) {
      // Handle unauthorized access, e.g., redirect to login page
      console.error("Permission Denied - redirecting to login");
    }
    return Promise.reject(error);
  }
);
export default axiosInstance;
