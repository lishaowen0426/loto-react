import React from "react";
import {
    Route,
    createBrowserRouter,
    createRoutesFromElements,
    RouterProvider,
} from "react-router-dom";
import { gsap } from "gsap";
import { useGSAP } from "@gsap/react";

import "./global.css";
import Home from "./components/Home";
import RootLayout from "./components/RootLayout";
import { Figure } from "./components/NumberScroll";

gsap.registerPlugin(useGSAP);

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route path="/" element={<RootLayout />}>
            <Route path="/home" element={<Home />} />
            <Route path="/figure" element={<Figure />} />
        </Route>
    )
);

const App = () => {
    return <RouterProvider router={router} />;
};
export default App;
