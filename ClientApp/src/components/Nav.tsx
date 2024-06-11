import { AlignStartVertical, CircleUser } from "lucide-react";
import { forwardRef, ComponentPropsWithoutRef } from "react";
import { IconButton } from "./Button";
import Login from "./Login";
const Nav = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div className="w-full flex flex-row justify-between">
                <IconButton>
                    <AlignStartVertical />
                </IconButton>
                <Login />
            </div>
        );
    }
);

export default Nav;
