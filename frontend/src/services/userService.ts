import axiosInstance from "../configs/axios-config/axiosInstance";

export const getUserById = async (id: string) => {
  try {
    const response = await axiosInstance.get(`/users/${id}`);
    return response.data;
  } catch (error) {
    console.error("Login error:", error);
    throw error;
  }
};
