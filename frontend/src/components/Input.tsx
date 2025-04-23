import { Eye, EyeOff } from "lucide-react";

interface InputProps {
  icon?: React.ReactNode;
  name: string;
  type: string;
  value: string;
  placeholder: string;
  onChange: (event: React.ChangeEvent<HTMLInputElement>) => void;
  errorMessage?: string;
  showPassword?: boolean;
  setShowPassword?: React.Dispatch<React.SetStateAction<boolean>>;
}
const Input: React.FC<InputProps> = ({
  name,
  type = "text",
  value,
  placeholder = "",
  onChange,
  errorMessage = "",
  icon,
  showPassword = false,
  setShowPassword = () => {},
}) => {
  return (
    <div className="relative">
      {icon && (
        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
          {icon}
        </div>
      )}
      <input
        id={name}
        name={name}
        type={type == "password" ? (showPassword ? "text" : "password") : type}
        value={value}
        onChange={onChange}
        className="bg-gray-800 text-gray-100 sm:text-sm rounded-lg block w-full pl-10 p-2.5 border border-gray-700 focus:ring-gray-700 focus:border-gray-700"
        placeholder={placeholder}
        required
      />
      {type === "password" && (
        <div className="absolute inset-y-0 right-0 pr-3 flex items-center">
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            className="text-gray-500 hover:text-gray-300 focus:outline-none"
          >
            {showPassword ? (
              <EyeOff className="h-5 w-5" />
            ) : (
              <Eye className="h-5 w-5" />
            )}
          </button>
        </div>
      )}
      {errorMessage && (
        <p className="text-sm text-red-500 mt-1">{errorMessage}</p>
      )}
    </div>
  );
};
export default Input;
