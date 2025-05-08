/* eslint-disable @typescript-eslint/no-explicit-any */
import React, { useEffect } from "react";
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Lock, User } from "lucide-react";
import InputCustom from "../../components/InputCustom";
import { login } from "../../services/authService";
import { useAuthContext } from "../../contexts/authContext";
import { jwtDecode } from "jwt-decode";
import LoadingPage from "../../components/loading/Loading";
import { toast } from "react-toastify";
interface LoginRequest {
  username: string;
  password: string;
}
export default function LoginPage() {
  const { setAuthenticated, isAuthenticated, decodedToken } = useAuthContext();
  const [formData, setFormData] = useState<LoginRequest>({
    username: "",
    password: "",
  });
  const [isLoading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState<boolean>(false);
  const [errors, setErrors] = useState<Record<string, string>>({
    username: "",
    password: "",
  });
  const navigate = useNavigate();
  const validateForm = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.username.trim()) {
      newErrors.username = "Username is required";
    }
    if (!formData.password.trim()) {
      newErrors.password = "Password is required";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };
  const roleString =
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setLoading(true);
      if (!validateForm()) {
        return;
      }
      const data = await login(formData.username, formData.password);
      localStorage.setItem("accessToken", data.accessToken);
      localStorage.setItem("refreshToken", data.refreshToken);

      setAuthenticated(true);
      const decodedData = jwtDecode(data.accessToken);
      if (
        decodedData &&
        (
          decodedData as {
            [roleString]: string;
          }
        )[roleString] === "SuperUser"
      ) {
        console.log("gethere");
        navigate("/admin");
      } else {
        navigate("/user");
      }
    } catch (error: any) {
      toast.error(error.response.data.error);
    } finally {
      setLoading(false);
    }
    // Handle login logic here
    console.log({ formData });
  };
  useEffect(() => {
    if (isAuthenticated) {
      if (
        (decodedToken as { [roleString]: string })[roleString] === "SuperUser"
      ) {
        navigate("/admin");
      } else {
        navigate("/user");
      }
    }
  }, [isAuthenticated, decodedToken, navigate]);
  function handleChange(
    event: React.ChangeEvent<
      HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement
    >
  ): void {
    const { name, value } = event.target;
    setFormData({
      ...formData,
      [name]: value,
    });
    // Clear error when field is edited
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  }
  if (isLoading) {
    return <LoadingPage></LoadingPage>;
  }
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-950 p-4">
      <div className="w-full max-w-md">
        {/* Logo/Header */}
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-white">Welcome Back</h1>
          <p className="text-gray-400 mt-2">
            Sign in to your account to continue
          </p>
        </div>

        {/* Login Form */}
        <div className="bg-gray-900 rounded-lg shadow-xl p-8">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Email Field */}
            <InputCustom
              icon={<User className="h-5 w-5 text-gray-500" />}
              label="Username"
              name="username"
              type="text"
              value={formData.username}
              onChange={handleChange}
              errorMessage={errors.username}
            ></InputCustom>

            {/* Password Field */}
            <InputCustom
              icon={<Lock className="h-5 w-5 text-gray-500" />}
              label="Password"
              name="password"
              type={showPassword ? "text" : "password"}
              value={formData.password}
              onChange={handleChange}
              showPassword={showPassword}
              setShowPassword={setShowPassword}
              errorMessage={errors.password}
            />

            {/* Submit Button */}
            <button
              type="submit"
              className="w-full py-2.5 px-5 text-sm font-medium text-white bg-gray-700 rounded-lg hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500"
            >
              Sign in
            </button>

            {/* Sign Up Link */}
            <div className="text-center text-sm text-gray-400">
              Don&apos;t have an account?{" "}
              <Link
                to="/register"
                className="text-gray-300 hover:text-white font-medium"
              >
                Sign up
              </Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
