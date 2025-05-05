import { Navigate } from "react-router-dom";
import { useAuthContext } from "../contexts/authContext";

const ProtectedRoute = ({
  role,
  children,
}: {
  role: string;
  children: React.ReactNode;
}) => {
  const { isAuthenticated, decodedToken } = useAuthContext() as {
    isAuthenticated: boolean;
    decodedToken: { role: string } | null;
  };
  console.log(decodedToken);
  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }
  if (decodedToken && decodedToken["role"] !== role) {
    return <Navigate to="/forbidden" />;
  }
  return children;
};
export default ProtectedRoute;
