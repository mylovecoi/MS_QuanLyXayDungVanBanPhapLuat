const Input = ({
                   type = "text",
                   id,
                   name,
                   placeholder,
                   value,
                   onChange,
                   className = "",
                   min,
                   max,
                   step,
                   disabled = false,
                   success = false,
                   error = false,
                   hint,
                   prefix,
                   suffix,
               }) => {
    let inputClasses = `modern-form-control h-11 w-full appearance-none rounded-2xl border px-4 py-2.5 text-sm shadow-none transition placeholder:text-gray-400 focus:outline-hidden focus:ring-3 dark:bg-gray-900 dark:text-white/90 dark:placeholder:text-white/30 ${className}`;

    if (disabled) {
        inputClasses += ` cursor-not-allowed border-gray-200 bg-gray-100 text-gray-500 opacity-60 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400`;
    } else if (error) {
        inputClasses += ` border-error-500 focus:border-error-300 focus:ring-error-500/20 dark:text-error-400 dark:border-error-500 dark:focus:border-error-800`;
    } else if (success) {
        inputClasses += ` border-success-500 focus:border-success-300 focus:ring-success-500/20 dark:text-success-400 dark:border-success-500 dark:focus:border-success-800`;
    } else {
        inputClasses += ` border-[var(--admin-border)] bg-white text-gray-800 focus:border-brand-300 focus:ring-brand-500/15 dark:border-gray-700 dark:bg-white/[0.04] dark:text-white/90 dark:focus:border-brand-800`;
    }

    if (prefix) {
        inputClasses += " pl-10";
    }

    if (suffix) {
        inputClasses += " pr-10";
    }

    return (
        <div className="relative">
            {prefix && (
                <div className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">
                    {prefix}
                </div>
            )}

            <input
                type={type}
                id={id}
                name={name}
                placeholder={placeholder}
                value={value}
                onChange={onChange}
                min={min}
                max={max}
                step={step}
                disabled={disabled}
                className={inputClasses}
            />

            {suffix && (
                <div className="absolute right-3 top-0 flex h-11 items-center justify-center">
                    {suffix}
                </div>
            )}
            {hint && (
                <p
                    className={`mt-1.5 text-xs ${
                        error
                            ? "text-error-500"
                            : success
                                ? "text-success-500"
                                : "text-gray-500"
                    }`}
                >
                    {hint}
                </p>
            )}
        </div>
    );
};

export default Input;
