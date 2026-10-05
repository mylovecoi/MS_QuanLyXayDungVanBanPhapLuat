const SearchBox = ({
                       value,
                       onChange,
                       placeholder = "Tìm kiếm...",
                   }) => {
    return (
        <div className="w-full sm:w-80">
            <input
                type="text"
                value={value}
                onChange={(e) => onChange(e.target.value)}
                placeholder={placeholder}
                className="h-10 w-full rounded-lg border border-gray-300 bg-white px-4 text-sm text-gray-700 outline-none transition placeholder:text-gray-400 focus:border-brand-500"
            />
        </div>
    );
};

export default SearchBox;