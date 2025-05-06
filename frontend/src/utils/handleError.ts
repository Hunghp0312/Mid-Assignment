/* eslint-disable @typescript-eslint/no-explicit-any */
import { toast } from "react-toastify";

export const handleAxiosError = (error: any) => {
  if (error?.response?.data?.error) {
    toast.error(error.response.data.error);
  }
  if (error?.response?.data?.errors) {
    for (const key in error.response.data.errors) {
      if (error.response.data.errors) {
        const errorMessage = error.response.data.errors[key];
        toast.error(`${errorMessage}`);
      }
    }
  }
};
