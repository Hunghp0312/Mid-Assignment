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
export const getUserBookBorrowing = async (
  id: string,
  status: string,
  pageIndex: number,
  pageSize: number
) => {
  try {
    const response = await axiosInstance.get(
      `/users/${id}/book-borrowing-requests`,
      {
        params: {
          status,
          pageIndex,
          pageSize,
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error("Login error:", error);
    throw error;
  }
};
