import { createContext, useContext, useEffect, useState } from "react";

const LayoutContext = createContext(null);

export const LayoutProvider = ({ children }) => {
    const [layoutMode, setLayoutMode] = useState(() => {
        return localStorage.getItem("layoutMode") || "sidebar";
    });

    useEffect(() => {
        localStorage.setItem("layoutMode", layoutMode);
    }, [layoutMode]);

    return (
        <LayoutContext.Provider
            value={{
                layoutMode,
                setLayoutMode,
            }}
        >
            {children}
        </LayoutContext.Provider>
    );
};

export const useLayout = () => {
    const context = useContext(LayoutContext);

    if (!context) {
        throw new Error("useLayout must be used inside LayoutProvider");
    }

    return context;
};