import React from "react";

import { useState } from "react";
import InputCustom from "../../components/InputCustom";
interface CategoryFormProps {
  onSubmit: (formData: FormData) => void;
  isSubmitting: boolean;
  actionButtonText?: string;
  onClose?: () => void;
  initialData?: {
    name?: string;
    description?: string;
  };
}

export function CategoryForm({
  onSubmit,
  isSubmitting,
  initialData,
  actionButtonText = "Save",
  onClose,
}: CategoryFormProps) {
  const [formState, setFormState] = useState({
    name: initialData?.name || "",
    description: initialData?.description || "",
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};
    if (!formState.name.trim()) {
      newErrors.name = "Name is required";
    }
    if (!formState.description.trim()) {
      newErrors.description = "Description is required";
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
    formData.append("name", formState.name);
    formData.append("description", formState.description);

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

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      {/* <div className="text-white">{JSON.stringify(formState)}</div> */}

      {/* Title Field */}
      <InputCustom
        label="Name"
        name="name"
        type="text"
        value={formState.name}
        onChange={handleChange}
        isRequired
        placeholder="Enter name of category"
        errorMessage={errors.name}
      ></InputCustom>

      {/* Description Field */}

      <InputCustom
        name="description"
        type="textarea"
        value={formState.description}
        onChange={handleChange}
        label="Description"
        isRequired
        placeholder="Enter category description"
        errorMessage={errors.description}
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
    </form>
  );
}
