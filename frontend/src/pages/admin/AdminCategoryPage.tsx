/* eslint-disable @typescript-eslint/no-explicit-any */
import DataTable from "../../components/CustomTable";
import { Pen, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import {
  addCategory,
  deleteCategory,
  editCategory,
  getCategoryPagination,
} from "../../services/categoryService";
import CustomModal from "../../components/CustomModal";
import { toast } from "react-toastify";
import { DeleteConfirmationModal } from "../../components/DeleteConfimation";
import { CategoryForm } from "./CategoryForm";
export interface CategoryData {
  id: string;
  name: string;
  description: string;
}
const AdminCategoryPage = () => {
  const [isLoading, setLoading] = useState(false);
  // State for managing data (for demonstration of actions)
  const [categoryData, setcategoryData] = useState<CategoryData[]>([]);
  // const [paginationData, setPaginationData] = useState({
  //   totalItems: 0,
  //   pageSize: 10,
  //   pageIndex: 1,
  // });
  // Form attribute
  const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [editId, setEditId] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [categoryToDelete, setCategoryToDelete] = useState({
    id: "",
    name: "",
    description: "",
  });
  // Pagination Data
  const [pageSize, setPageSize] = useState(10);
  const [pageIndex, setPageIndex] = useState(1);
  const [totalItems, setTotalItems] = useState(0);

  const [formInitialData, setFormInitialData] = useState({});
  // Add to list with limit
  const addWithLimit = (newElement: any) => {
    setcategoryData((prevList) => {
      const updatedList = [...prevList];

      if (updatedList.length >= pageSize) {
        updatedList.pop(); // remove last
      }

      updatedList.unshift(newElement); // add to front
      return updatedList;
    });
  };
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      const data = await getCategoryPagination(pageIndex, pageSize);
      setcategoryData(data.items);
      setTotalItems(data.totalCount);
      setLoading(false);
    };
    fetchData();
  }, [pageSize, pageIndex]);

  const handleAddCategory = () => {
    setEditId("");
    setFormInitialData({});
    setIsFormOpen(true);
  };
  const handleAddSubmitCategory = async (category: FormData) => {
    try {
      setIsSubmitting(true);
      const categoryResponse = await addCategory(category);
      toast.success(`cCtegory ${categoryResponse.name} added successfully.`);
      addWithLimit(categoryResponse);
      setIsSubmitting(false);
    } catch (error: any) {
      setIsSubmitting(false);
      toast.error(error.response.data.error);
    }
  };
  const handleSubmitAddEditCategory = async (category: FormData) => {
    if (editId) {
      handleEditSubmitCategory(category);
    } else {
      handleAddSubmitCategory(category);
    }
    setIsFormOpen(false);
  };
  const handleEditCategory = (category: any) => {
    setEditId(category.id);
    setFormInitialData({
      ...category,
      publishedDate: new Date(category.publishedDate),
    });
    setIsFormOpen(true);
  };
  const handleEditSubmitCategory = async (category: FormData) => {
    try {
      setIsSubmitting(true);
      console.log("Editing category:", category);
      const categoryResponse = await editCategory(editId, category);
      toast.success(`Category ${categoryResponse.name} edited successfully.`);
      setcategoryData((prevList) => {
        const updatedList = [...prevList];
        const index = updatedList.findIndex((item) => item.id === editId);
        if (index !== -1) {
          updatedList[index] = categoryResponse;
        }
        return updatedList;
      });
      setFormInitialData({});
      setEditId("");
      setIsSubmitting(false);
    } catch (error: any) {
      setIsSubmitting(false);
      toast.error(error.response.data.error);
    }
  };
  const handleDeleteCategory = async (category: CategoryData) => {
    setIsConfirmationOpen(true);
    setCategoryToDelete(category);
  };
  const getData = async () => {
    setLoading(true);
    const data = await getCategoryPagination(pageIndex, pageSize);
    setcategoryData(data.items);
    setTotalItems(data.totalCount);
    setLoading(false);
  };
  const handleDeleteConfirmation = async () => {
    try {
      setIsConfirmationOpen(false);
      await deleteCategory(categoryToDelete.id);
      toast.success(`Category ${categoryToDelete.name} deleted successfully.`);
      getData();
    } catch (error: any) {
      setIsConfirmationOpen(false);
      toast.error(error.response.data.error);
    }
  };
  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold  text-gray-300 ">Category Table</h1>
        <button
          className={`px-3 py-1 text-lg rounded-md flex items-center 
            bg-gray-700 text-gray-300 hover:bg-gray-600 border border-gray-600 transition-colors
          `}
          onClick={handleAddCategory}
        >
          Add category
        </button>
      </div>

      <div className="space-y-10">
        {/* Inventory Data Table with Checkboxes and Bulk Actions */}
        <div className="bg-gray-900 rounded-lg shadow-lg p-6">
          <DataTable
            data={categoryData}
            keyField="id"
            pagination={true}
            pageIndex={pageIndex}
            pageSize={pageSize}
            setPageIndex={setPageIndex}
            setPageSize={setPageSize}
            totalItems={totalItems}
            isLoading={isLoading}
            columns={[
              {
                header: "Name",
                accessor: "name",
                align: "left",
                width: "25%",
              },
              {
                header: "Description",
                accessor: "description",
                align: "left",
                width: "25%",
              },
            ]}
            actions={[
              {
                label: "Edit",
                icon: <Pen className="h-4 w-4" />,
                onClick: (row) => handleEditCategory(row),
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
              {
                label: "Delete",
                icon: <Trash2 className="h-4 w-4" />,
                onClick: (row) => handleDeleteCategory(row),
                className: "bg-red-700 text-red-300 hover:bg-red-600",
              },
            ]}
          />
        </div>
      </div>
      <CustomModal
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        title={editId ? "Edit Category" : "Add Category"}
      >
        <CategoryForm
          initialData={formInitialData}
          onSubmit={handleSubmitAddEditCategory}
          onClose={() => setIsFormOpen(false)}
          isSubmitting={isSubmitting}
        ></CategoryForm>
      </CustomModal>
      <DeleteConfirmationModal
        isOpen={isConfirmationOpen}
        onClose={() => setIsConfirmationOpen(false)}
        onConfirm={async () => handleDeleteConfirmation()}
        description={`Are you sure you want to delete the category "${categoryToDelete.name}"?`}
      />
    </div>
  );
};
export default AdminCategoryPage;
