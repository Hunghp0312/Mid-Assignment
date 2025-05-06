import axiosInstance from "../configs/axios-config/axiosInstance";

export const login = async (username: string, password: string) => {
  try {
    const response = await axiosInstance.post("/auth/login", {
      username,
      password,
    });
    return response.data;
  } catch (error) {
    console.error("Login error:", error);
    throw error;
  }
};
export const register = async (
  firstName: string,
  lastName: string,
  username: string,
  email: string,
  password: string
) => {
  try {
    const response = await axiosInstance.post("/auth/register", {
      firstName,
      lastName,
      username,
      email,
      password,
    });
    return response.data;
  } catch (error) {
    console.error("Register error:", error);
    throw error;
  }
};
