import React, {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { jwtDecode } from "jwt-decode";
import { getUserById } from "../services/userService";
import LoadingPage from "../components/loading/Loading";

interface AuthContextType {
  isAuthenticated: boolean;
  setAuthenticated: React.Dispatch<React.SetStateAction<boolean>>;
  decodedToken: object;
  user: object;
}
interface DecodedTokenType {
  id: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  exp: number;
  iat: number;
}
const authContext = createContext<AuthContextType>({
  isAuthenticated: false,
  setAuthenticated: () => {},
  decodedToken: {},
  user: {},
});

// eslint-disable-next-line react-refresh/only-export-components
export const useAuthContext = () => useContext(authContext);

const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const token = localStorage.getItem("accessToken");
  console.log(token);
  const [isAuthenticated, setAuthenticated] = useState(!!token);
  const [isLoading, setLoading] = useState(false);
  const [user, setUser] = useState({});
  const decodedToken = useMemo(() => {
    if (token) {
      return jwtDecode(token);
    }
    return {};
  }, [token]);

  const contextValue = {
    isAuthenticated,
    setAuthenticated,
    decodedToken,
    user,
  };
  useEffect(() => {
    const fetchUser = async () => {
      if (isAuthenticated && decodedToken) {
        setLoading(true);
        const data = await getUserById(
          (decodedToken as DecodedTokenType)["id"]
        );
        setUser(data);
        setLoading(false);
      }
    };
    fetchUser();
  }, [decodedToken, isAuthenticated]);
  if (isLoading) {
    return <LoadingPage></LoadingPage>;
  }
  return (
    <authContext.Provider value={contextValue}>{children}</authContext.Provider>
  );
};
export default AuthProvider;
