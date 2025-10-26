import React from "react";

interface CheckboxProps {
    label: string;
    name: string;
    checked: boolean;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const Checkbox: React.FC<CheckboxProps> = ({ label, name, checked, onChange }) => (
    <label className="flex items-center text-white mb-2">
        <input
            type="checkbox"
            name={name}
            checked={checked}
            onChange={onChange}
            className="mr-2 accent-indigo-500"
        />
        {label}
    </label>
);

export default Checkbox;
