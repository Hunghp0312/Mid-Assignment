import { useState } from "react";
import Navbar from "./Navbar";
import { Book, Tags, BookCopy, BookOpen } from "lucide-react";
import { Outlet } from "react-router-dom";
import { useAuthContext } from "../contexts/authContext";
const roleString =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
export default function Layout() {
  const [collapsed, setCollapsed] = useState(false);
  const { decodedToken } = useAuthContext() as {
    decodedToken: { [roleString]: string } | null;
  };
  const toggleSidebar = () => {
    setCollapsed(!collapsed);
  };

  if (decodedToken && decodedToken[roleString] === "SuperUser") {
    const navItems = [
      { icon: <Book size={20} />, label: "Books", href: "" },
      {
        icon: <Tags size={20} />,
        label: "Categories",
        href: "categories",
      },
      {
        icon: <BookOpen size={20} />,
        label: "Book-Borrowing",
        href: "book-borrowing",
      },
    ];
    return (
      <div className="flex min-h-screen bg-gray-950">
        <Navbar
          collapsed={collapsed}
          toggleSidebar={toggleSidebar}
          navItems={navItems}
        />
        <main
          className={`flex-1 transition-all duration-300 ${
            collapsed ? "ml-16" : "ml-64"
          }`}
        >
          <Outlet />
        </main>
      </div>
    );
  }
  const navItems = [
    { icon: <Book size={20} />, label: "Book", href: "" },
    {
      icon: <BookCopy size={20} />,
      label: "My Borrowings",
      href: "book-borrowing",
    },
  ];
  return (
    <div className="flex min-h-screen bg-gray-950">
      <Navbar
        collapsed={collapsed}
        toggleSidebar={toggleSidebar}
        navItems={navItems}
      />
      <main
        className={`flex-1 transition-all duration-300 ${
          collapsed ? "ml-16" : "ml-64"
        }`}
      >
        <Outlet />
      </main>
    </div>
  );
}
