import { useState } from "react";
import CustomModal from "./CustomModal";
import { AlertTriangle } from "lucide-react";

interface DeleteConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: () => Promise<void>;
  title?: string;
  description?: string;
  itemName?: string;
  deleteButtonText?: string;
  cancelButtonText?: string;
}

export function DeleteConfirmationModal({
  isOpen,
  onClose,
  onConfirm,
  title = "Confirm Deletion",
  description = "Are you sure you want to delete this item? This action cannot be undone.",
  itemName,
  deleteButtonText = "Delete",
  cancelButtonText = "Cancel",
}: DeleteConfirmationModalProps) {
  const [isDeleting, setIsDeleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleConfirm = async () => {
    setIsDeleting(true);
    setError(null);

    try {
      await onConfirm();
      onClose();
    } catch (err) {
      setError(
        err instanceof Error ? err.message : "An error occurred during deletion"
      );
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <CustomModal isOpen={isOpen} onClose={onClose} title={title} size="sm">
      <div className="p-6">
        <div className="flex items-center gap-4 mb-4">
          <div className="flex-shrink-0 bg-red-900/30 p-2 rounded-full">
            <AlertTriangle className="h-6 w-6 text-red-500" />
          </div>
          <div>
            <p className="text-gray-300">{description}</p>
            {itemName && (
              <p className="mt-2 font-medium text-white">
                &quot;{itemName}&quot;
              </p>
            )}
          </div>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-900/30 border border-red-900 rounded-md text-red-400">
            {error}
          </div>
        )}

        <div className="flex justify-end gap-3 mt-6">
          <button
            type="button"
            onClick={onClose}
            className="px-4 py-2 bg-gray-800 text-gray-300 rounded-md hover:bg-gray-700 focus:outline-none focus:ring-1 focus:ring-gray-600"
            disabled={isDeleting}
          >
            {cancelButtonText}
          </button>
          <button
            type="button"
            onClick={handleConfirm}
            className={`px-4 py-2 bg-red-700 text-white rounded-md hover:bg-red-600 focus:outline-none focus:ring-1 focus:ring-red-500 flex items-center ${
              isDeleting ? "opacity-70 cursor-not-allowed" : ""
            }`}
            disabled={isDeleting}
          >
            {isDeleting ? (
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
                Deleting...
              </>
            ) : (
              deleteButtonText
            )}
          </button>
        </div>
      </div>
    </CustomModal>
  );
}
