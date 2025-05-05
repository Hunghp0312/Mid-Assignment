import { useState } from "react";
import Navbar from "./Navbar";
import {
  LayoutDashboard,
  Users,
  Settings,
  HelpCircle,
  Book,
  Tags,
  BookCopy,
} from "lucide-react";
import { Outlet } from "react-router-dom";
import { useAuthContext } from "../contexts/authContext";
export default function Layout() {
  const [collapsed, setCollapsed] = useState(false);
  const { decodedToken } = useAuthContext() as {
    decodedToken: { role: string } | null;
  };
  const toggleSidebar = () => {
    setCollapsed(!collapsed);
  };
  if (decodedToken && decodedToken["role"] === "Admin") {
    const navItems = [
      { icon: <LayoutDashboard size={20} />, label: "Dashboard", href: "/" },
      { icon: <Users size={20} />, label: "Users", href: "users" },
      {
        icon: <Tags size={20} />,
        label: "Categories",
        href: "categories",
      },
      { icon: <Book size={20} />, label: "Books", href: "books" },
      { icon: <Settings size={20} />, label: "Settings", href: "/settings" },
      { icon: <HelpCircle size={20} />, label: "Help", href: "/help" },
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
    { icon: <LayoutDashboard size={20} />, label: "Dashboard", href: "" },
    { icon: <BookCopy size={20} />, label: "My Borrowings", href: "borrows" },
    { icon: <Settings size={20} />, label: "Settings", href: "/settings" },
    { icon: <HelpCircle size={20} />, label: "Help", href: "/help" },
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
