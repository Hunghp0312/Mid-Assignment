import Layout from "./layout/Layout";
import Login from "./pages/auth/Login";
import { useRoutes } from "react-router-dom";
import NotFound from "./pages/NotFound";
import Register from "./pages/auth/Register";
import Forbidden from "./pages/Forbidden";
import ProtectedRoute from "./routers/ProtectedRoute";
import UserBookPage from "./pages/user/UserBookPage";
import AdminBookPage from "./pages/admin/AdminBookPage";
import BookBorrowingPage from "./pages/BookBorrowingPage";
import AdminCategoryPage from "./pages/admin/AdminCategoryPage";

const AppRouter = () => {
  const routes = useRoutes([
    { path: "*", element: <NotFound /> },
    { path: "/forbidden", element: <Forbidden /> },
    { path: "/login", element: <Login /> },
    { path: "/", element: <Login /> },
    { path: "/register", element: <Register /> },
    {
      path: "/admin",
      element: (
        <ProtectedRoute role="SuperUser">
          <Layout />
        </ProtectedRoute>
      ),
      children: [
        {
          path: "",
          element: <AdminBookPage />,
        },
        {
          path: "book-borrowing",
          element: <BookBorrowingPage />,
        },
        {
          path: "categories",
          element: <AdminCategoryPage />,
        },
      ],
    },
    {
      path: "/user",
      element: (
        <ProtectedRoute role="User">
          <Layout />
        </ProtectedRoute>
      ),
      children: [
        {
          path: "",
          element: <UserBookPage />,
        },
        {
          path: "book-borrowing",
          element: <BookBorrowingPage />,
        },
      ],
    },
  ]);
  return routes;
};
export default AppRouter;
