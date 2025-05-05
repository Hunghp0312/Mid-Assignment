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
import { BookForm } from "../../components/BookForm";
import CustomModal from "../../components/CustomModal";
import { toast } from "react-toastify";
import { DeleteConfirmationModal } from "../../components/DeleteConfimation";
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
      toast.error("Error adding book. Please try again.", error.message);
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
      toast.error("Error editing book. Please try again.", error.message);
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
      toast.error(error.response.data.error);
    }
  };
  useEffect(() => {
    console.log(selectedBooks);
  }, [selectedBooks]);
  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold  text-gray-300 ">Books Table</h1>
        <button
          className={`px-3 py-1 text-lg rounded-md flex items-center 
          bg-gray-700 text-gray-300 hover:bg-gray-600 border border-gray-600 transition-colors
        `}
          onClick={handleAddBook}
        >
          Add Book
        </button>
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
      />
    </div>
  );
};
export default AdminBookPage;
