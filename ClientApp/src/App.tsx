import React from "react";
import {
    Route,
    createBrowserRouter,
    createRoutesFromElements,
    RouterProvider,
} from "react-router-dom";

import "./global.css";
import Home from "./components/Home";

const router = createBrowserRouter(
    createRoutesFromElements(<Route path="/" element={<Home />}></Route>)
);

const App = () => {
    return <RouterProvider router={router} />;
};
export default App;
