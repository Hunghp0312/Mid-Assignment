/* eslint-disable @typescript-eslint/no-explicit-any */
import DataTable from "../../components/CustomTable";
import { Archive } from "lucide-react";
import { useEffect, useState } from "react";
import { BookFilterParam, getBookPagination } from "../../services/bookService";
import { addBorrowing } from "../../services/bookBorrowingService";
import InputCustom from "../../components/InputCustom";
import { getCategories } from "../../services/categoryService";
import { handleAxiosError } from "../../utils/handleError";
import { toast } from "react-toastify";
import { ConfirmationModal } from "../../components/Confirmation";
interface BookData {
  id: string;
  title: string;
  author: string;
  categoryName: string;
  available: number;
}
interface Category {
  id: string;
  name: string;
  title?: string;
}
const UserBookPage = () => {
  const [isLoading, setLoading] = useState(false);
  const [filterParameters, setFilterParameters] = useState<BookFilterParam>({
    query: "",
    categoryId: "",
    available: "",
  });
  // State for managing data (for demonstration of actions)
  const [bookData, setBookData] = useState<BookData[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  // const [paginationData, setPaginationData] = useState({
  //   totalItems: 0,
  //   pageSize: 10,
  //   pageIndex: 1,
  // });
  // Pagination Data
  const [pageSize, setPageSize] = useState(10);
  const [pageIndex, setPageIndex] = useState(1);
  const [totalItems, setTotalItems] = useState(0);
  // State for selected rows
  const [selectedBooks, setSelectedBooks] = useState<BookData[]>([]);
  const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
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
    const fetchCategory = async () => {
      const data = await getCategories();
      setCategories([{ id: "", name: "All" }, ...data]);
    };
    fetchCategory();
  }, []);

  // Bulk action handlers
  const handleBorrowBook = async () => {
    try {
      const bookIds = selectedBooks.map((book) => book.id);
      await addBorrowing(bookIds);
      toast.success("Books borrowed successfully!");
    } catch (error) {
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
      <h1 className="text-2xl font-bold mb-6 text-gray-300">Books Table</h1>
      {/* Search and filter controls */}
      <div className="flex space-x-4 mb-6">
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
      </div>
      <div className="space-y-10">
        {/* Inventory Data Table with Checkboxes and Bulk Actions */}
        <div className="bg-gray-900 rounded-lg shadow-lg p-6">
          <DataTable
            data={bookData}
            keyField="id"
            pagination={true}
            selectable={true}
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
                header: "Available",
                accessor: (row) => (
                  <StatusIndicator available={row.available} />
                ),
                align: "right",
                width: "25%",
              },
            ]}
            bulkActions={[
              {
                label: `Borrow Books ${selectedBooks.length}/5`,
                icon: <Archive className="h-4 w-4" />,
                onClick: () => setIsConfirmationOpen(true),
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
            ]}
          />
        </div>
      </div>
      <ConfirmationModal
        isOpen={isConfirmationOpen}
        onClose={() => setIsConfirmationOpen(false)}
        onConfirm={async () => handleBorrowBook()}
        description={`Are you sure you want to delete the category \n"${selectedBooks
          .map((book) => book.title)
          .join("\n")}"?`}
      />
    </div>
  );
};
export default UserBookPage;
