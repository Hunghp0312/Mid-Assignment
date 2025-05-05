import Layout from "./layout/Layout";
import Login from "./pages/auth/Login";
import { useRoutes } from "react-router-dom";
import NotFound from "./pages/NotFound";
import Home from "./pages/Home";
import Register from "./pages/auth/Register";
import Forbidden from "./pages/Forbidden";
import ProtectedRoute from "./routers/ProtectedRoute";
import UserBookPage from "./pages/user/UserBookPage";
import LoadingPage from "./components/Loading";
import LoadingOverlay from "./components/LoadingOverlay";
import TestPage from "./pages/TestPage";
import AddEditBookPage from "./pages/admin/AdminBookPage";
import CustomModal from "./components/CustomModal";
import AdminBookPage from "./pages/admin/AdminBookPage";
import { BookRequestsList } from "./components/BookRequest";

const AppRouter = () => {
  const handleOnClose = () => {
    console.log("Modal closed");
  };

  const routes = useRoutes([
    { path: "*", element: <NotFound /> },
    { path: "/forbidden", element: <Forbidden /> },
    { path: "/login", element: <Login /> },
    { path: "/register", element: <Register /> },
    { path: "/loading", element: <LoadingPage /> },
    { path: "/loading-overlay", element: <LoadingOverlay /> },
    { path: "/test", element: <TestPage /> },
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
          element: <Home />,
        },
        {
          path: "test123",
          element: <BookRequestsList />,
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
          path: "test",
          element: <Home />,
        },
        {
          path: "book-form",
          element: (
            <CustomModal isOpen={true} title="123" onClose={handleOnClose}>
              <AddEditBookPage />
            </CustomModal>
          ),
        },
        {
          path: "123",
          element: <AdminBookPage />,
        },
      ],
    },
  ]);
  return routes;
};
export default AppRouter;
