import React from "react";
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Lock, Mail } from "lucide-react";
import InputCustom from "../../components/InputCustom";
import InputCheckbox from "../../components/InputCheckbox";
import { login } from "../../services/authService";
import { useAuthContext } from "../../contexts/authContext";
import { jwtDecode } from "jwt-decode";
import LoadingPage from "../../components/Loading";
interface FormData {
  username: string;
  password: string;
  rememberMe: boolean;
}
export default function LoginPage() {
  const { setAuthenticated } = useAuthContext();
  const [formData, setFormData] = useState<FormData>({
    username: "",
    password: "",
    rememberMe: false,
  });
  const [isLoading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState<boolean>(false);
  const navigate = useNavigate();
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setLoading(true);
      const data = await login(formData.username, formData.password);
      localStorage.setItem("accessToken", data.accessToken);
      setAuthenticated(true);
      const decodedData = jwtDecode(data.accessToken);
      setLoading(false);
      if ((decodedData as { role: string })["role"] === "Admin") {
        navigate("/admin");
      } else {
        navigate("/user");
      }
    } catch (error) {
      console.error("Login error:", error);
    }
    // Handle login logic here
    console.log({ formData });
  };

  function handleChange(
    event: React.ChangeEvent<
      HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement
    >
  ): void {
    const { name, value, type, checked } = event.target;
    const fieldValue = type === "checkbox" ? checked : value;
    setFormData({
      ...formData,
      [name]: fieldValue,
    });
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
              icon={<Mail className="h-5 w-5 text-gray-500" />}
              label="Username"
              name="username"
              type="text"
              value={formData.username}
              onChange={handleChange}
              errorMessage="Invalid email address"
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
            />

            {/* Remember Me & Forgot Password */}
            <div className="flex items-center justify-between">
              <InputCheckbox
                label="Remember me"
                name="rememberMe"
                onChange={handleChange}
                checked={formData.rememberMe}
              ></InputCheckbox>
              <Link
                to="/forgot-password"
                className="text-sm text-gray-400 hover:text-gray-200"
              >
                Forgot password?
              </Link>
            </div>

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
          {JSON.stringify(formData)}
          {/* Divider */}
          {/* <div className="flex items-center my-6">
            <div className="flex-grow border-t border-gray-700"></div>
            <span className="px-4 text-sm text-gray-500">Or continue with</span>
            <div className="flex-grow border-t border-gray-700"></div>
          </div> */}

          {/* Social Login Buttons */}
          {/* <div className="grid grid-cols-2 gap-4">
            <button className="py-2.5 px-5 text-sm font-medium text-white bg-gray-800 rounded-lg border border-gray-700 hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500">
              Google
            </button>
            <button className="py-2.5 px-5 text-sm font-medium text-white bg-gray-800 rounded-lg border border-gray-700 hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500">
              GitHub
            </button>
          </div> */}
        </div>
      </div>
    </div>
  );
}
