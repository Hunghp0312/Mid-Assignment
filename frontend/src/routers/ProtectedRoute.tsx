import { Navigate } from "react-router-dom";
import { useAuthContext } from "../contexts/authContext";

const ProtectedRoute = ({
  role,
  children,
}: {
  role: string;
  children: React.ReactNode;
}) => {
  const roleString =
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const { isAuthenticated, decodedToken } = useAuthContext() as {
    isAuthenticated: boolean;
    decodedToken: {
      [roleString]: string;
    };
  };
  console.log(isAuthenticated);
  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }
  console.log(decodedToken[roleString], role);
  if (decodedToken && decodedToken[roleString] !== role) {
    console.log(decodedToken[roleString]);
    return <Navigate to="/forbidden" />;
  }
  return children;
};
export default ProtectedRoute;
