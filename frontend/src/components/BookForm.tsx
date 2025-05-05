import React, { useEffect } from "react";

import { useState } from "react";
import InputCustom from "./InputCustom";
import InputDate from "./InputDate";
import { getCategories } from "../services/categoryService";
import { set } from "date-fns";
import LoadingOverlay from "./LoadingOverlay";
interface BookFormProps {
  onSubmit: (formData: FormData) => void;
  isSubmitting: boolean;
  actionButtonText?: string;
  onClose?: () => void;
  initialData?: {
    title?: string;
    author?: string;
    description?: string;
    quantity?: number;
    isbn?: string;
    publishedDate?: Date;
    categoryId?: string;
  };
}

export function BookForm({
  onSubmit,
  isSubmitting,
  initialData,
  actionButtonText = "Save",
  onClose,
}: BookFormProps) {
  const [formState, setFormState] = useState({
    title: initialData?.title || "",
    author: initialData?.author || "",
    description: initialData?.description || "",
    quantity: initialData?.quantity || 1,
    isbn: initialData?.isbn || "",
    publishedDate: initialData?.publishedDate || new Date(),
    categoryId: initialData?.categoryId || "",
  });
  const [isLoading, setIsLoading] = useState(false);
  useEffect(() => {
    const fetchCategory = async () => {
      setIsLoading(true);
      const data = await getCategories();
      setCategories(data);
      if (!initialData?.categoryId) {
        setFormState((prev) => ({
          ...prev,
          categoryId: data[0]?.id || prev.categoryId,
        }));
      }
      setIsLoading(false);
    };
    fetchCategory();
  }, []);
  const [categories, setCategories] = useState([]);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formState.title.trim()) {
      newErrors.title = "Title is required";
    }

    if (!formState.author.trim()) {
      newErrors.author = "Author is required";
    }

    if (!formState.description.trim()) {
      newErrors.description = "Description is required";
    }

    if (formState.quantity < 0) {
      newErrors.quantity = "Quantity cannot be negative";
    }

    if (formState.isbn && !/^(?:\d{10}|\d{13})$/.test(formState.isbn)) {
      newErrors.isbn = "ISBN must be 10 or 13 digits";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const formData = new FormData();
    formData.append("title", formState.title);
    formData.append("author", formState.author);
    formData.append("description", formState.description);
    formData.append("quantity", formState.quantity.toString());
    formData.append("isbn", formState.isbn);
    formData.append("publishedDate", formState.publishedDate.toISOString());
    formData.append("categoryId", formState.categoryId);

    onSubmit(formData);
  };

  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value } = e.target;

    setFormState((prev) => ({
      ...prev,
      [name]: name === "quantity" ? Number.parseInt(value) || 0 : value,
    }));

    // Clear error when field is edited
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  const handleDateSelect = (date: Date, name: string) => {
    setFormState((prev) => ({ ...prev, [name]: date }));
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      {/* <div className="text-white">{JSON.stringify(formState)}</div> */}

      {/* Title Field */}
      <InputCustom
        label="Title"
        name="title"
        type="text"
        value={formState.title}
        onChange={handleChange}
        isRequired
        placeholder="Enter title of the book"
      ></InputCustom>

      {/* Author Field */}
      <InputCustom
        label="Author"
        name="author"
        type="text"
        value={formState.author}
        onChange={handleChange}
        isRequired
        placeholder="Enter author name"
      ></InputCustom>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Published Date Field */}
        <InputDate
          value={formState.publishedDate}
          onChange={handleDateSelect}
          name="publishedDate"
          label="Published Date"
          isRequired
        ></InputDate>
        {/* Category Field */}
        <InputCustom
          type="select"
          value={formState.categoryId}
          onChange={handleChange}
          name="categoryId"
          label="Category"
          optionList={categories}
        ></InputCustom>
        {/* Quantity Field */}
        <InputCustom
          name="quantity"
          type="number"
          value={formState.quantity}
          onChange={handleChange}
          label="Quantity"
          isRequired
        ></InputCustom>
        {/* ISBN Field */}
        <InputCustom
          name="isbn"
          type="string"
          value={formState.isbn}
          onChange={handleChange}
          label="ISBN"
          isRequired
          placeholder="Enter ISBN (10 or 13 digits)"
        ></InputCustom>
      </div>
      {/* Description Field */}

      <InputCustom
        name="description"
        type="textarea"
        value={formState.description}
        onChange={handleChange}
        label="Description"
        isRequired
        placeholder="Enter book description"
      ></InputCustom>
      {/* Form Actions */}
      <div className="flex justify-end space-x-4 pt-4">
        <button
          type="button"
          className="px-4 py-2 bg-gray-800 text-gray-300 rounded-md hover:bg-gray-700 focus:outline-none focus:ring-1 focus:ring-gray-600"
          onClick={onClose}
          disabled={isSubmitting}
        >
          Cancel
        </button>
        <button
          type="submit"
          className={`px-4 py-2 bg-gray-700 text-white rounded-md hover:bg-gray-600 focus:outline-none focus:ring-1 focus:ring-gray-500 flex items-center ${
            isSubmitting ? "opacity-70 cursor-not-allowed" : ""
          }`}
          disabled={isSubmitting}
        >
          {isSubmitting ? (
            <>
              <svg
                className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
              >
                <circle
                  className="opacity-25"
                  cx="12"
                  cy="12"
                  r="10"
                  stroke="currentColor"
                  strokeWidth="4"
                ></circle>
                <path
                  className="opacity-75"
                  fill="currentColor"
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                ></path>
              </svg>
              Saving...
            </>
          ) : (
            actionButtonText
          )}
        </button>
      </div>
      {isLoading && <LoadingOverlay />}
    </form>
  );
}
