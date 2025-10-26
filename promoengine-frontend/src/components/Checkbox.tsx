import React from "react";

interface CheckboxProps {
    label: React.ReactNode;
    name: string;
    checked: boolean;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const Checkbox: React.FC<CheckboxProps> = ({
    label,
    name,
    checked,
    onChange,
}) => (
    <label className="flex items-center text-white mb-2 cursor-pointer select-none">
        <input
            type="checkbox"
            name={name}
            checked={checked}
            onChange={onChange}
            className="mr-2 accent-indigo-500"
        />
        <span>{label}</span>
    </label>
);

export default Checkbox;
