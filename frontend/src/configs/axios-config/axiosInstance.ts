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
  async (error) => {
    console.log(error);
    // Handle response error
    const originalRequest = error.config;
    if (error.response.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true; // Mark the request as retried to avoid infinite loops.
      try {
        const refreshToken = localStorage.getItem("refreshToken"); // Retrieve the stored refresh token.
        const accessToken = localStorage.getItem("accessToken"); // Retrieve the stored refresh token.os
        // Make a request to your auth server to refresh the token.
        const response = await axiosInstance.post("/auth/refresh-token", {
          accessToken,
          refreshToken,
        });
        const { accessToken: newAccessToken, refreshToken: newRefreshToken } =
          response.data;
        // Store the new access and refresh tokens.
        localStorage.setItem("accessToken", newAccessToken);
        localStorage.setItem("refreshToken", newRefreshToken);
        console.log("New access token:", newRefreshToken);
        // Update the authorization header with the new access token.
        axiosInstance.defaults.headers.common[
          "Authorization"
        ] = `Bearer ${newAccessToken}`;
        return axiosInstance(originalRequest); // Retry the original request with the new access token.
      } catch (refreshError) {
        // Handle refresh token errors by clearing stored tokens and redirecting to the login page.
        console.error("Token refresh failed:", refreshError);
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        navigate("/login");
        return Promise.reject(refreshError);
      }
    }
    // if (error.response.status === 401) {
    //   // Handle unauthorized access, e.g., redirect to login page
    //   console.error("Unauthorized access - redirecting to login");
    //   navigate("/login");
    // }
    else if (error.response.status === 403) {
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
