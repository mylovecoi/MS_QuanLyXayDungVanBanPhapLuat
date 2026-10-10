const FileInput = ({className, onChange, id, name}) => {
  return (
      <input
          id={id}
          name={name}
          type="file"
          className={`modern-form-control h-11 w-full overflow-hidden rounded-2xl border border-[var(--admin-border)] bg-white text-sm text-gray-500 shadow-none transition-colors file:mr-5 file:h-11 file:cursor-pointer file:border-0 file:border-r file:border-solid file:border-gray-200 file:bg-gray-50 file:px-4 file:text-sm file:font-medium file:text-gray-700 placeholder:text-gray-400 hover:file:bg-brand-50 focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/10 dark:border-gray-700 dark:bg-white/[0.04] dark:text-gray-400 dark:text-white/90 dark:file:border-gray-800 dark:file:bg-white/[0.03] dark:file:text-gray-400 dark:placeholder:text-gray-400 ${className || ""}`}
          onChange={onChange}
      />
  );
};

export default FileInput;
