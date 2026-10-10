const TextArea = ({
                      placeholder = "Enter your message", // Placeholder text
                      rows = 3, // Number of rows
                      value = "", // Current value
                      onChange, // Change handler
                      className = "", // Additional CSS classes
                      disabled = false, // Disabled state
                      error = false, // Error state
                      hint = "", // Hint text to display
                  }) => {
    const handleChange = (e) => {
        if (onChange) {
            onChange(e.target.value);
        }
    };

    let textareaClasses = `modern-form-control w-full rounded-2xl border px-4 py-3 text-sm shadow-none transition focus:outline-hidden ${className} `;

    if (disabled) {
        textareaClasses += ` cursor-not-allowed border-gray-200 bg-gray-100 text-gray-500 opacity-60 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400`;
    } else if (error) {
        textareaClasses += ` border-gray-300 bg-white focus:border-error-300 focus:ring-3 focus:ring-error-500/10 dark:border-gray-700 dark:bg-white/[0.04] dark:text-white/90 dark:focus:border-error-800`;
    } else {
        textareaClasses += ` border-[var(--admin-border)] bg-white text-gray-900 focus:border-brand-300 focus:ring-3 focus:ring-brand-500/10 dark:border-gray-700 dark:bg-white/[0.04] dark:text-white/90 dark:focus:border-brand-800`;
    }

    return (
        <div className="relative">
      <textarea
          placeholder={placeholder}
          rows={rows}
          value={value}
          onChange={handleChange}
          disabled={disabled}
          className={textareaClasses}
      />

            {hint && (
                <p
                    className={`mt-2 text-sm ${
                        error ? "text-error-500" : "text-gray-500 dark:text-gray-400"
                    }`}
                >
                    {hint}
                </p>
            )}
        </div>
    );
};

export default TextArea;
