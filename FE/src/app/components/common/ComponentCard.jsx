const ComponentCard = ({
                         title,
                         children,
                         className = "",
                         desc = "",
                       }) => {
  return (
      <div
          className={`modern-card rounded-[var(--admin-radius-lg)] border border-[var(--admin-border)] bg-white shadow-[var(--admin-shadow-soft)] dark:border-gray-800 dark:bg-white/[0.03] ${className}`}
      >
        {/* Card Header */}
        <div className="px-6 py-5">
          <h3 className="text-base font-medium text-gray-800 dark:text-white/90">
            {title}
          </h3>

          {desc && (
              <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                {desc}
              </p>
          )}
        </div>

        {/* Card Body */}
        <div className="border-t border-[var(--admin-border)] p-4 dark:border-gray-800 sm:p-6">
          <div className="space-y-6">{children}</div>
        </div>
      </div>
  );
};

export default ComponentCard;
