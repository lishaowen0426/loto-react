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
import { Number, NumberPad } from "./components/CheckNumber";
import CheckNumber from "./components/CheckNumber";

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route path="/" element={<RootLayout />}>
            <Route path="/home" element={<Home />} />
            <Route path="/number" element={<NumberPad />} />
            <Route path="/checknumber" element={<CheckNumber />} />
        </Route>
    )
);

const App = () => {
    return <RouterProvider router={router} />;
};
export default App;
