import axiosInstance from "../configs/axios-config/axiosInstance";

export const addBorrowing = async (bookIds: string[]) => {
  try {
    const response = await axiosInstance.post("/book-borrowing-requests", {
      bookIds,
    });
    return response.data;
  } catch (error) {
    console.error("Error adding borrowing:", error);
    throw error;
  }
};
export const getBorrowing = async (
  status: string,
  pageIndex: number,
  pageSize: number
) => {
  try {
    const response = await axiosInstance.get("/book-borrowing-requests", {
      params: {
        status,
        pageIndex,
        pageSize,
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error fetching borrowing data:", error);
    throw error;
  }
};
export const approveBorrowing = async (id: string) => {
  try {
    const response = await axiosInstance.post(
      `/book-borrowing-requests/${id}/approve`
    );
    return response.data;
  } catch (error) {
    console.error("Error approving borrowing:", error);
    throw error;
  }
};
export const rejectBorrowing = async (id: string) => {
  try {
    const response = await axiosInstance.post(
      `/book-borrowing-requests/${id}/reject`
    );
    return response.data;
  } catch (error) {
    console.error("Error approving borrowing:", error);
    throw error;
  }
};
