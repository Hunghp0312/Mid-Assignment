import axiosInstance from "../configs/axios-config/axiosInstance";

export interface BookFilterParam {
  query: string;
  categoryId: string;
  available: string;
}
export const getBookPagination = async (
  filterParam: BookFilterParam,
  pageIndex: number,
  pageSize: number
) => {
  try {
    const response = await axiosInstance.get(
      `/books?pageIndex=${pageIndex}&pageSize=${pageSize}&query=${filterParam.query}&categoryId=${filterParam.categoryId}&available=${filterParam.available}`
    );
    return response.data;
  } catch (error) {
    console.error("Error fetching book pagination:", error);
    throw error;
  }
};
export const addBook = async (bookData: FormData) => {
  try {
    const response = await axiosInstance.post("/books", bookData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error adding book:", error);
    throw error;
  }
};
export const editBook = async (bookId: string, bookData: FormData) => {
  try {
    const response = await axiosInstance.put(`/books/${bookId}`, bookData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error editing book:", error);
    throw error;
  }
};
export const deleteBook = async (bookId: string) => {
  try {
    const response = await axiosInstance.delete(`/books/${bookId}`);
    return response.data;
  } catch (error) {
    console.error("Error deleting book:", error);
    throw error;
  }
};
