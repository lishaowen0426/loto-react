import { AlignStartVertical, CircleUser } from "lucide-react";
import { forwardRef, ComponentPropsWithoutRef } from "react";
import { IconButton } from "./Button";
const Nav = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div className="w-full flex flex-row justify-between">
                <IconButton>
                    <AlignStartVertical />
                </IconButton>
                <IconButton>
                    <CircleUser />
                </IconButton>
            </div>
        );
    }
);

export default Nav;
