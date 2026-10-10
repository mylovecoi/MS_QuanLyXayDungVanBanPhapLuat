import { useState } from "react";

const Select = ({
                    options,
                    placeholder = "Select an option",
                    onChange,
                    className = "",
                    defaultValue = "",
                    value,
                    disabled = false,
                }) => {
    const [selectedValue, setSelectedValue] = useState(defaultValue);

    const currentValue =
        value !== undefined ? value : selectedValue;

    const handleChange = (e) => {
        const newValue = e.target.value;

        setSelectedValue(newValue);
        onChange?.(newValue);
    };

    return (
        <select
            className={`modern-form-control h-11 w-full appearance-none rounded-2xl border border-[var(--admin-border)] bg-white px-4 py-2.5 pr-11 text-sm shadow-none transition placeholder:text-gray-400 focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/10 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:opacity-60 dark:border-gray-700 dark:bg-white/[0.04] dark:text-white/90 dark:placeholder:text-white/30 dark:focus:border-brand-800 ${
                currentValue
                    ? "text-gray-800 dark:text-white/90"
                    : "text-gray-400 dark:text-gray-400"
            } ${className}`}
            value={currentValue}
            onChange={handleChange}
            disabled={disabled}
        >
            <option
                value=""
                disabled
                className="text-gray-700 dark:bg-gray-900 dark:text-gray-400"
            >
                {placeholder}
            </option>

            {options.map((option) => (
                <option
                    key={option.value}
                    value={option.value}
                    className="text-gray-700 dark:bg-gray-900 dark:text-gray-400"
                >
                    {option.label}
                </option>
            ))}
        </select>
    );
};

export default Select;
