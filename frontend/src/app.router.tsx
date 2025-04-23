import Layout from "./components/Layout";
import Login from "./pages/auth/Login";
import { useRoutes } from "react-router-dom";
import NotFound from "./pages/NotFound";
import Home from "./pages/Home";
import Register from "./pages/auth/Register";

const AppRouter = () => {
  const routes = useRoutes([
    { path: "*", element: <NotFound /> },
    { path: "/login", element: <Login /> },
    { path: "/register", element: <Register /> },
    {
      element: <Layout />,
      children: [
        {
          path: "/",
          element: <Home />,
        },
      ],
    },
  ]);
  return routes;
};
export default AppRouter;
