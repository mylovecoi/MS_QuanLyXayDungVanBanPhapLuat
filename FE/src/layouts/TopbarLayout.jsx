import { Outlet } from "react-router";
import AppHeader from "./AppHeader.jsx";
import AppTopbar from "./AppTopbar.jsx";
import Footer from "../app/components/footer/Footer.jsx";

const TopbarLayout = () => {
    return (
        <div className="min-h-screen flex flex-col">
            <AppHeader />

            <AppTopbar />

            <main className="flex-1">
                <div className="w-auto p-4 md:p-6">
                    <Outlet />
                </div>
            </main>

            <Footer />
        </div>
    );
};

export default TopbarLayout;