import { useState } from "react";
import Navbar from "./Navbar";
import {
  LayoutDashboard,
  Users,
  Settings,
  HelpCircle,
  BarChart2,
  FileText,
} from "lucide-react";
import { Outlet } from "react-router-dom";
export default function Layout() {
  const [collapsed, setCollapsed] = useState(false);

  const toggleSidebar = () => {
    setCollapsed(!collapsed);
  };
  const navItems = [
    { icon: <LayoutDashboard size={20} />, label: "Dashboard", href: "/" },
    { icon: <Users size={20} />, label: "Users", href: "/users" },
    { icon: <BarChart2 size={20} />, label: "Analytics", href: "/analytics" },
    { icon: <FileText size={20} />, label: "Reports", href: "/reports" },
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
