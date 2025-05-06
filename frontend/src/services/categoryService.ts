/* eslint-disable @typescript-eslint/no-explicit-any */
import axiosInstance from "../configs/axios-config/axiosInstance";

export const getCategories = async () => {
  try {
    const response = await axiosInstance.get("/categories");
    return response.data;
  } catch (error) {
    console.error("Error fetching categories:", error);
    throw error;
  }
};
export const getCategoryById = async (id: string) => {
  try {
    const response = await axiosInstance.get(`/categories/${id}`);
    return response.data;
  } catch (error) {
    console.error("Error fetching category:", error);
    throw error;
  }
};
export const addCategory = async (category: any) => {
  try {
    const response = await axiosInstance.post("/categories", category);
    return response.data;
  } catch (error) {
    console.error("Error adding category:", error);
    throw error;
  }
};
export const editCategory = async (id: string, category: any) => {
  try {
    const response = await axiosInstance.put(`/categories/${id}`, category);
    return response.data;
  } catch (error) {
    console.error("Error editing category:", error);
    throw error;
  }
};
export const deleteCategory = async (id: string) => {
  try {
    const response = await axiosInstance.delete(`/categories/${id}`);
    return response.data;
  } catch (error) {
    console.error("Error deleting category:", error);
    throw error;
  }
};
export const getCategoryPagination = async (
  pageIndex: number,
  pageSize: number
) => {
  try {
    const response = await axiosInstance.get(
      `/categories?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
    return response.data;
  } catch (error) {
    console.error("Error fetching categories:", error);
    throw error;
  }
};
