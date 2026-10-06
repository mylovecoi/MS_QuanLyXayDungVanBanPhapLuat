// import React from 'react';
// import { createRoot } from 'react-dom/client';
// import { App } from './app/App.jsx';
// import './styles.css';
//
// createRoot(document.getElementById('root')).render(
//   <React.StrictMode>
//     <App />
//   </React.StrictMode>
// );
// //
import "@fontsource-variable/inter";
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import "swiper/swiper-bundle.css";
import "flatpickr/dist/flatpickr.css";
import {App} from "./app/App.jsx";
import { ThemeProvider } from "./context/ThemeContext";

import { AppWrapper } from "./app/components/common/PageMeta";

createRoot(document.getElementById("root")).render(
    <StrictMode>
        <ThemeProvider>
            <AppWrapper>
                <App />
            </AppWrapper>
        </ThemeProvider>
    </StrictMode>,
);