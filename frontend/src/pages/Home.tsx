import DataTable from "../components/CustomTable";
import { Eye, Edit, Trash2, Archive, Tag } from "lucide-react";
import { useState } from "react";

export default function DataDisplayPage() {
  // State for managing data (for demonstration of actions)
  const [inventoryData, setInventoryData] = useState([
    {
      id: "P001",
      name: "Wireless Headphones",
      category: "Electronics",
      stock: 145,
      price: 89.99,
      status: "In Stock",
    },
    {
      id: "P002",
      name: "Smart Watch",
      category: "Electronics",
      stock: 78,
      price: 199.99,
      status: "In Stock",
    },
    {
      id: "P003",
      name: "Bluetooth Speaker",
      category: "Electronics",
      stock: 0,
      price: 59.99,
      status: "Out of Stock",
    },
    {
      id: "P004",
      name: "Laptop Backpack",
      category: "Accessories",
      stock: 234,
      price: 45.99,
      status: "In Stock",
    },
    {
      id: "P005",
      name: "Mechanical Keyboard",
      category: "Computer Peripherals",
      stock: 12,
      price: 129.99,
      status: "Low Stock",
    },
    {
      id: "P006",
      name: "Wireless Mouse",
      category: "Computer Peripherals",
      stock: 56,
      price: 49.99,
      status: "In Stock",
    },
    {
      id: "P007",
      name: "Monitor Stand",
      category: "Accessories",
      stock: 23,
      price: 79.99,
      status: "In Stock",
    },
    {
      id: "P008",
      name: "USB-C Hub",
      category: "Electronics",
      stock: 42,
      price: 69.99,
      status: "In Stock",
    },
    {
      id: "P009",
      name: "External SSD",
      category: "Storage",
      stock: 18,
      price: 149.99,
      status: "Low Stock",
    },
    {
      id: "P010",
      name: "Webcam",
      category: "Electronics",
      stock: 0,
      price: 89.99,
      status: "Out of Stock",
    },
    {
      id: "P011",
      name: "Desk Lamp",
      category: "Accessories",
      stock: 67,
      price: 39.99,
      status: "In Stock",
    },
    {
      id: "P012",
      name: "Ergonomic Chair",
      category: "Furniture",
      stock: 5,
      price: 299.99,
      status: "Low Stock",
    },
  ]);

  // State for selected rows
  const [selectedProducts, setSelectedProducts] = useState<any[]>([]);

  // Financial data example
  const financialData = [
    {
      id: 1,
      month: "January",
      revenue: 45250.75,
      expenses: 32100.5,
      profit: 13150.25,
      growth: 0,
    },
    {
      id: 2,
      month: "February",
      revenue: 48750.25,
      expenses: 33200.75,
      profit: 15549.5,
      growth: 18.24,
    },
    {
      id: 3,
      month: "March",
      revenue: 52150.5,
      expenses: 34500.25,
      profit: 17650.25,
      growth: 13.51,
    },
    {
      id: 4,
      month: "April",
      revenue: 49800.75,
      expenses: 35100.5,
      profit: 14700.25,
      growth: -16.71,
    },
    {
      id: 5,
      month: "May",
      revenue: 53250.25,
      expenses: 34750.75,
      profit: 18499.5,
      growth: 25.85,
    },
    {
      id: 6,
      month: "June",
      revenue: 54100.5,
      expenses: 35200.25,
      profit: 18900.25,
      growth: 2.17,
    },
    {
      id: 7,
      month: "July",
      revenue: 56750.25,
      expenses: 36100.75,
      profit: 20649.5,
      growth: 9.26,
    },
    {
      id: 8,
      month: "August",
      revenue: 58200.5,
      expenses: 36500.25,
      profit: 21700.25,
      growth: 5.09,
    },
    {
      id: 9,
      month: "September",
      revenue: 55800.75,
      expenses: 37100.5,
      profit: 18700.25,
      growth: -13.82,
    },
    {
      id: 10,
      month: "October",
      revenue: 59250.25,
      expenses: 37750.75,
      profit: 21499.5,
      growth: 14.97,
    },
    {
      id: 11,
      month: "November",
      revenue: 62100.5,
      expenses: 38200.25,
      profit: 23900.25,
      growth: 11.17,
    },
    {
      id: 12,
      month: "December",
      revenue: 68750.25,
      expenses: 39100.75,
      profit: 29649.5,
      growth: 24.06,
    },
  ];

  // User activity data
  const activityData = [
    {
      id: 1,
      user: "John Doe",
      action: "Created new project",
      timestamp: "2023-04-23T10:30:15Z",
      ip: "192.168.1.1",
      status: "success",
    },
    {
      id: 2,
      user: "Jane Smith",
      action: "Updated user profile",
      timestamp: "2023-04-23T09:15:22Z",
      ip: "192.168.1.45",
      status: "success",
    },
    {
      id: 3,
      user: "Bob Johnson",
      action: "Failed login attempt",
      timestamp: "2023-04-22T22:45:10Z",
      ip: "192.168.2.30",
      status: "error",
    },
    {
      id: 4,
      user: "Alice Brown",
      action: "Deleted document",
      timestamp: "2023-04-22T16:20:05Z",
      ip: "192.168.3.12",
      status: "warning",
    },
    {
      id: 5,
      user: "Charlie Wilson",
      action: "Exported data report",
      timestamp: "2023-04-22T14:05:30Z",
      ip: "192.168.1.87",
      status: "success",
    },
    {
      id: 6,
      user: "Diana Miller",
      action: "Changed password",
      timestamp: "2023-04-21T11:30:15Z",
      ip: "192.168.4.22",
      status: "success",
    },
    {
      id: 7,
      user: "Edward Davis",
      action: "Failed login attempt",
      timestamp: "2023-04-21T09:45:10Z",
      ip: "192.168.5.18",
      status: "error",
    },
    {
      id: 8,
      user: "Fiona Clark",
      action: "Uploaded new files",
      timestamp: "2023-04-20T16:20:05Z",
      ip: "192.168.1.32",
      status: "success",
    },
    {
      id: 9,
      user: "George Rodriguez",
      action: "Created new user account",
      timestamp: "2023-04-20T14:05:30Z",
      ip: "192.168.2.87",
      status: "success",
    },
    {
      id: 10,
      user: "Hannah Lee",
      action: "Modified system settings",
      timestamp: "2023-04-19T11:30:15Z",
      ip: "192.168.3.45",
      status: "warning",
    },
  ];

  // Format currency
  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
      minimumFractionDigits: 2,
    }).format(amount);
  };

  // Format date
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat("en-US", {
      dateStyle: "medium",
      timeStyle: "short",
    }).format(date);
  };

  // Status component
  const StatusIndicator = ({ status }: { status: string }) => {
    const getStatusColor = () => {
      switch (status.toLowerCase()) {
        case "success":
          return "bg-green-500";
        case "error":
          return "bg-red-500";
        case "warning":
          return "bg-yellow-500";
        case "in stock":
          return "bg-green-500";
        case "out of stock":
          return "bg-red-500";
        case "low stock":
          return "bg-yellow-500";
        default:
          return "bg-gray-500";
      }
    };

    return (
      <div className="flex items-center">
        <div className={`h-2 w-2 rounded-full ${getStatusColor()} mr-2`}></div>
        <span>{status}</span>
      </div>
    );
  };

  // Action handlers
  const handleView = (product: any) => {
    alert(`Viewing details for ${product.name}`);
  };

  const handleEdit = (product: any) => {
    alert(`Editing ${product.name}`);
  };

  const handleDelete = (product: any) => {
    if (confirm(`Are you sure you want to delete ${product.name}?`)) {
      setInventoryData(inventoryData.filter((item) => item.id !== product.id));
    }
  };

  // Bulk action handlers
  const handleBulkDelete = (products: any[]) => {
    if (
      confirm(
        `Are you sure you want to delete ${products.length} selected products?`
      )
    ) {
      const productIds = products.map((product) => product.id);
      setInventoryData(
        inventoryData.filter((item) => !productIds.includes(item.id))
      );
      setSelectedProducts([]);
    }
  };

  const handleBulkArchive = (products: any[]) => {
    alert(`Archiving ${products.length} products`);
    // In a real application, you would update the status of these products
  };

  const handleBulkTag = (products: any[]) => {
    const tag = prompt("Enter tag name for selected products:");
    if (tag) {
      alert(`Adding tag "${tag}" to ${products.length} products`);
      // In a real application, you would update the products with the new tag
    }
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-6">Data Display Tables</h1>

      <div className="space-y-10">
        {/* Financial Data Table with Pagination */}

        {/* Inventory Data Table with Checkboxes and Bulk Actions */}
        <div className="bg-gray-900 rounded-lg shadow-lg p-6">
          <h2 className="text-xl font-semibold mb-4">Product Inventory</h2>
          <DataTable
            data={inventoryData}
            keyField="id"
            pagination={true}
            itemsPerPage={6}
            selectable={true}
            onSelectionChange={setSelectedProducts}
            columns={[
              {
                header: "Product ID",
                accessor: "id",
                align: "left",
                width: "12%",
              },
              {
                header: "Product Name",
                accessor: "name",
                align: "left",
                width: "25%",
              },
              {
                header: "Category",
                accessor: "category",
                align: "left",
                width: "18%",
              },
              {
                header: "Price",
                accessor: (row) => formatCurrency(row.price),
                align: "right",
                width: "15%",
              },
              {
                header: "Stock",
                accessor: "stock",
                align: "right",
                width: "10%",
              },
              {
                header: "Status",
                accessor: (row) => <StatusIndicator status={row.status} />,
                align: "left",
                width: "15%",
              },
            ]}
            actions={[
              {
                label: "View",
                icon: <Eye className="h-4 w-4" />,
                onClick: handleView,
                className: "bg-gray-800 text-gray-300 hover:bg-gray-700",
              },
              {
                label: "Edit",
                icon: <Edit className="h-4 w-4" />,
                onClick: handleEdit,
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
              {
                label: "Delete",
                icon: <Trash2 className="h-4 w-4" />,
                onClick: handleDelete,
                className:
                  "bg-red-900/30 text-red-400 hover:bg-red-900/50 border-red-900",
              },
            ]}
            bulkActions={[
              {
                label: "Delete",
                icon: <Trash2 className="h-4 w-4" />,
                onClick: handleBulkDelete,
                className:
                  "bg-red-900/30 text-red-400 hover:bg-red-900/50 border-red-900",
              },
              {
                label: "Archive",
                icon: <Archive className="h-4 w-4" />,
                onClick: handleBulkArchive,
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
              {
                label: "Tag",
                icon: <Tag className="h-4 w-4" />,
                onClick: handleBulkTag,
                className: "bg-gray-700 text-gray-300 hover:bg-gray-600",
              },
            ]}
          />
        </div>

        {/* Activity Data Table with Checkboxes */}
      </div>
    </div>
  );
}
