import { useNavigate } from "react-router-dom";
import "./App.css";
import AppRouter from "./app.router";
import { ToastContainer } from "react-toastify";
import { useEffect } from "react";
import { setNavigator } from "./utils/navigateHelper";

function App() {
  const navigate = useNavigate();

  useEffect(() => {
    // Store navigate function when App mounts
    setNavigator(navigate);
  }, [navigate]);
  return (
    <>
      <AppRouter></AppRouter>
      <ToastContainer />
    </>
  );
}

export default App;
