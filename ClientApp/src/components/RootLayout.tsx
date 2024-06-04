import { ReactNode } from "react";
import { Outlet } from "react-router-dom";

const RootLayout = () => {
    return (
        <div className="w-full px-[1rem] pt-[1rem] flex flex-col items-cente scroll-smooth">
            <Outlet />
        </div>
    );
};

export default RootLayout;
