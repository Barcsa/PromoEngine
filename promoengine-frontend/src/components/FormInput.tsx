import React from "react";

interface FormInputProps {
  label: string;
  name: string;
  type?: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  error?: string;
  autoComplete?: string;
}

const FormInput: React.FC<FormInputProps> = ({
  label,
  name,
  type = "text",
  value,
  onChange,
  error,
  autoComplete
}) => (
  <div className="flex flex-col mb-4">
    <label htmlFor={name} className="text-sm mb-1">
      {label}
    </label>
    <input
      id={name}
      name={name}
      type={type}
      value={value}
      onChange={onChange}
      autoComplete={autoComplete}
      className={`w-full px-3 py-2 rounded-md bg-gray-800 text-white focus:outline-none focus:ring-2 ${
        error
          ? "border border-red-500 focus:ring-red-500"
          : "border border-transparent focus:ring-indigo-500"
      }`}
    />
    {error && <span className="text-red-400 text-sm mt-1">{error}</span>}
  </div>
);

export default FormInput;
