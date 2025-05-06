/* eslint-disable @typescript-eslint/no-explicit-any */
import type React from "react";

import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Lock, Mail, User } from "lucide-react";
import InputCustom from "../../components/InputCustom";
import { register } from "../../services/authService";
import { handleAxiosError } from "../../utils/handleError";
import { toast } from "react-toastify";
export default function RegisterPage() {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  });
  const [showPassword, setShowPassword] = useState(false);
  const [agreeToTerms, setAgreeToTerms] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement
    >
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };
  const navigate = useNavigate();
  const validateForm = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.firstName.trim()) {
      newErrors.firstName = "First name is required";
    }
    if (!formData.lastName.trim()) {
      newErrors.lastName = "Last name is required";
    }
    if (!formData.username.trim()) {
      newErrors.username = "Username is required";
    }
    if (!formData.email.trim()) {
      newErrors.email = "Email is required";
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = "Email is invalid";
    }
    if (!formData.password.trim()) {
      newErrors.password = "Password is required";
    }
    if (!formData.confirmPassword.trim()) {
      newErrors.confirmPassword = "Confirm password is required";
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // Validate form
    if (!validateForm()) {
      return;
    }
    try {
      // Handle registration logic here
      await register(
        formData.firstName,
        formData.lastName,
        formData.username,
        formData.email,
        formData.password
      );
      toast.success("Registration successful! Please log in.");
      navigate("/login");
    } catch (error: any) {
      handleAxiosError(error);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-950 p-4">
      <div className="w-full max-w-lg">
        {/* Header */}
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-white">Create an Account</h1>
          <p className="text-gray-400 mt-2">
            Sign up to get started with our platform
          </p>
        </div>

        {/* Registration Form */}
        <div className="bg-gray-900 rounded-lg shadow-xl p-8">
          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Name Fields */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <InputCustom
                icon={<User className="h-5 w-5 text-gray-500" />}
                label="First Name"
                name="firstName"
                type="text"
                value={formData.firstName}
                onChange={handleChange}
                placeholder="First Name"
                errorMessage={errors.firstName}
              ></InputCustom>
              <InputCustom
                label="Last Name"
                name="lastName"
                type="text"
                value={formData.lastName}
                onChange={handleChange}
                placeholder="Last Name"
                errorMessage={errors.lastName}
              ></InputCustom>
            </div>
            <InputCustom
              icon={<User className="h-5 w-5 text-gray-500" />}
              label="Username"
              name="username"
              type="text"
              value={formData.username}
              onChange={handleChange}
              placeholder="Username"
              errorMessage={errors.username}
            ></InputCustom>
            {/* Email Field */}
            <InputCustom
              icon={<Mail className="h-5 w-5 text-gray-500" />}
              label="Email"
              name="email"
              type="text"
              value={formData.email}
              onChange={handleChange}
              placeholder="Email"
              errorMessage={errors.email}
            ></InputCustom>

            {/* Password Field */}
            <InputCustom
              icon={<Lock className="h-5 w-5 text-gray-500" />}
              label="Password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              showPassword={showPassword}
              setShowPassword={setShowPassword}
              errorMessage={errors.password}
              placeholder="••••••••"
            />

            {/* Confirm Password Field */}
            <InputCustom
              icon={<Lock className="h-5 w-5 text-gray-500" />}
              label="Confirm Password"
              name="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange}
              showPassword={showPassword}
              setShowPassword={setShowPassword}
              errorMessage={errors.confirmPassword}
              placeholder="••••••••"
            />

            {/* Terms and Conditions */}
            <div className="flex items-start">
              <div className="flex items-center h-5">
                <input
                  id="terms"
                  type="checkbox"
                  checked={agreeToTerms}
                  onChange={(e) => setAgreeToTerms(e.target.checked)}
                  className="h-4 w-4 bg-gray-800 border-gray-700 rounded focus:ring-offset-gray-900 focus:ring-gray-700"
                  required
                />
              </div>
              <div className="ml-3 text-sm">
                <label htmlFor="terms" className="text-gray-300">
                  I agree to the{" "}
                  <Link
                    to="/terms"
                    className="text-gray-400 hover:text-white underline"
                  >
                    Terms and Conditions
                  </Link>{" "}
                  and{" "}
                  <Link
                    to="/privacy"
                    className="text-gray-400 hover:text-white underline"
                  >
                    Privacy Policy
                  </Link>
                </label>
              </div>
            </div>

            {/* Submit Button */}
            <button
              type="submit"
              className="w-full py-2.5 px-5 text-sm font-medium text-white bg-gray-700 rounded-lg hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-500"
            >
              Create Account
            </button>

            {/* Sign In Link */}
            <div className="text-center text-sm text-gray-400">
              Already have an account?{" "}
              <Link
                to="/login"
                className="text-gray-300 hover:text-white font-medium"
              >
                Sign in
              </Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
