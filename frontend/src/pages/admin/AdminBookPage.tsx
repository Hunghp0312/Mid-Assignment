/* eslint-disable @typescript-eslint/no-explicit-any */
import DataTable from "../../components/CustomTable";
import { Archive, Pen, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import {
  addBook,
  BookFilterParam,
  deleteBook,
  editBook,
  getBookPagination,
} from "../../services/bookService";
import { BookForm } from "./BookForm";
import CustomModal from "../../components/CustomModal";
import { toast } from "react-toastify";
import { DeleteConfirmationModal } from "../../components/DeleteConfimation";
import InputCustom from "../../components/InputCustom";
import { getCategories } from "../../services/categoryService";
import { handleAxiosError } from "../../utils/handleError";
import { Category } from "../../interface/categoryInterface";
// import {BookData} from "../../interface/bookInterface";
interface BookData {
  id: string;
  title: string;
  author: string;
  categoryName: string;
  available: number;
  quantity: number;
}
const AdminBookPage = () => {
  const [isLoading, setLoading] = useState(false);
  const [categories, setCategories] = useState<Category[]>([]);
  // State for managing data (for demonstration of actions)
  const [bookData, setBookData] = useState<BookData[]>([]);
  const [filterParameters, setFilterParameters] = useState<BookFilterParam>({
    query: "",
    categoryId: "",
    available: "",
  });
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
  const [bookToDelete, setBookToDelete] = useState({
    id: "",
    title: "",
  });
  // Pagination Data
  const [pageSize, setPageSize] = useState(10);
  const [pageIndex, setPageIndex] = useState(1);
  const [totalItems, setTotalItems] = useState(0);
  // State for selected rows
  const [selectedBooks, setSelectedBooks] = useState<BookData[]>([]);
  const [formInitialData, setFormInitialData] = useState({});
  // Status component
  const StatusIndicator = ({ available }: { available: number }) => {
    const getStatusColor = () => {
      switch (available) {
        case 0:
          return "bg-red-500";
        default:
          return "bg-green-500";
      }
    };

    return (
      <div className="flex items-center justify-end">
        <div className={`h-2 w-2 rounded-full ${getStatusColor()} mr-2`}></div>
        <span>{available}</span>
      </div>
    );
  };
  // Add to list with limit
  const addWithLimit = (newElement: any) => {
    setBookData((prevList) => {
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
      const data = await getBookPagination(
        filterParameters,
        pageIndex,
        pageSize
      );
      setBookData(data.items);
      setTotalItems(data.totalCount);
      setLoading(false);
    };
    fetchData();
  }, [filterParameters, pageSize, pageIndex]);
  useEffect(() => {
    const fetchCategories = async () => {
      // Fetch categories from your API or service
      const categoryData = await getCategories(); // Replace with your API call
      setCategories([{ id: "", name: "All" }, ...categoryData]);
    };
    fetchCategories();
  }, []);

  // Bulk action handlers
  const handleBorrowBook = (books: BookData[]) => {
    console.log("Borrowing books:", books);
    // Implement your borrowing logic here
  };
  const handleAddBook = () => {
    setEditId("");
    setFormInitialData({});
    setIsFormOpen(true);
  };
  const handleAddSubmitBook = async (book: FormData) => {
    try {
      setIsSubmitting(true);
      const bookResponse = await addBook(book);
      toast.success(`Book ${bookResponse.title} added successfully.`);
      addWithLimit(bookResponse);
      setIsSubmitting(false);
    } catch (error: any) {
      setIsSubmitting(false);
      handleAxiosError(error);
    }
  };
  const handleSubmitAddEditBook = async (book: FormData) => {
    if (editId) {
      handleEditSubmitBook(book);
    } else {
      handleAddSubmitBook(book);
    }
    setIsFormOpen(false);
  };
  const handleEditBook = (book: any) => {
    setEditId(book.id);
    setFormInitialData({
      ...book,
      publishedDate: new Date(book.publishedDate),
    });
    setIsFormOpen(true);
  };
  const handleEditSubmitBook = async (book: FormData) => {
    try {
      setIsSubmitting(true);
      console.log("Editing book:", book);
      const bookResponse = await editBook(editId, book);
      toast.success(`Book ${bookResponse.title} edited successfully.`);
      setBookData((prevList) => {
        const updatedList = [...prevList];
        const index = updatedList.findIndex((item) => item.id === editId);
        if (index !== -1) {
          updatedList[index] = bookResponse;
        }
        return updatedList;
      });
      setFormInitialData({});
      setEditId("");
      setIsSubmitting(false);
    } catch (error: any) {
      setIsSubmitting(false);
      handleAxiosError(error);
    }
  };
  const handleDeleteBook = async (book: BookData) => {
    setIsConfirmationOpen(true);
    setBookToDelete(book);
  };
  const getData = async () => {
    setLoading(true);
    const data = await getBookPagination(filterParameters, pageIndex, pageSize);
    setBookData(data.items);
    setTotalItems(data.totalCount);
    setLoading(false);
  };
  const handleDeleteConfirmation = async () => {
    try {
      setIsConfirmationOpen(false);
      await deleteBook(bookToDelete.id);
      toast.success(`Book ${bookToDelete.title} deleted successfully.`);
      getData();
    } catch (error: any) {
      setIsConfirmationOpen(false);
      console.log(error);
      handleAxiosError(error);
    }
  };

  const handleFilterChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value } = e.target;
    setFilterParameters((prev) => ({ ...prev, [name]: value }));
  };
  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold  text-gray-300 ">Books Table</h1>
        <div className="flex space-x-4">
          <InputCustom
            name="query"
            type="text"
            value={filterParameters.query}
            onChange={handleFilterChange}
            placeholder="Enter title of the book"
          ></InputCustom>
          <InputCustom
            type="select"
            value={filterParameters.categoryId}
            onChange={handleFilterChange}
            name="categoryId"
            optionList={categories}
          ></InputCustom>
          <InputCustom
            type="select"
            value={filterParameters.available}
            onChange={handleFilterChange}
            name="available"
            optionList={[
              { id: "", name: "All" },
              { id: "true", name: "Available" },
              { id: " false", name: "Not Available" },
            ]}
          ></InputCustom>
          <button
            className={`px-3 py-1 text-lg rounded-md flex items-center 
          bg-gray-700 text-gray-300 hover:bg-gray-600 border border-gray-600 transition-colors
        `}
            onClick={handleAddBook}
          >
            Add Book
          </button>
        </div>
      </div>

      <div className="space-y-10">
        {/* Inventory Data Table with Checkboxes and Bulk Actions */}
        <div className="bg-gray-900 rounded-lg shadow-lg p-6">
          <DataTable
            data={bookData}
            keyField="id"
            pagination={true}
            pageIndex={pageIndex}
            pageSize={pageSize}
            setPageIndex={setPageIndex}
            setPageSize={setPageSize}
            totalItems={totalItems}
            maxSelectable={5}
            selectedRows={selectedBooks}
            setSelectedRows={setSelectedBooks}
            isLoading={isLoading}
            columns={[
              {
                header: "Title",
                accessor: "title",
                align: "left",
                width: "25%",
              },
              {
                header: "Author",
                accessor: "author",
                align: "left",
                width: "25%",
              },
              {
                header: "Category",
                accessor: "categoryName",
                align: "left",
                width: "25%",
              },
              {
                header: "Quanttiy",
                accessor: "quantity",
                align: "right",
                width: "25%",
              },
              {
                header: "Available",
                accessor: (row) => (
                  <StatusIndicator available={row.available} />
                ),
                align: "right",
                width: "25%",
              },
            ]}
            actions={[
              {
                label: "Edit",
                icon: <Pen className="h-4 w-4" />,
                onClick: (row) => handleEditBook(row),
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
              {
                label: "Delete",
                icon: <Trash2 className="h-4 w-4" />,
                onClick: (row) => handleDeleteBook(row),
                className: "bg-red-700 text-red-300 hover:bg-red-600",
              },
            ]}
            bulkActions={[
              {
                label: `Borrow Books ${selectedBooks.length}/5`,
                icon: <Archive className="h-4 w-4" />,
                onClick: handleBorrowBook,
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
            ]}
          />
        </div>
      </div>
      <CustomModal
        isOpen={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        title={editId ? "Edit Book" : "Add Book"}
      >
        <BookForm
          initialData={formInitialData}
          onSubmit={handleSubmitAddEditBook}
          onClose={() => setIsFormOpen(false)}
          isSubmitting={isSubmitting}
        ></BookForm>
      </CustomModal>
      <DeleteConfirmationModal
        isOpen={isConfirmationOpen}
        onClose={() => setIsConfirmationOpen(false)}
        onConfirm={async () => handleDeleteConfirmation()}
        description={`Are you sure you want to delete the category "${bookToDelete.title}"?`}
      />
    </div>
  );
};
export default AdminBookPage;
