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
import NumberTable from "./components/NumberTable";

const router = createBrowserRouter(
    createRoutesFromElements(
        <Route element={<RootLayout />}>
            <Route path="/" element={<Home />} />
            <Route
                path="/checknumber"
                element={<CheckNumber />}
                loader={async () => {
                    return fetch("/api/loto7/latestnumber", {
                        method: "GET",
                        headers: { Accept: "application/json" },
                    });
                }}
            />
            <Route path="current" element={<NumberTable />} />
        </Route>
    )
);

const App = () => {
    return <RouterProvider router={router} />;
};
export default App;
