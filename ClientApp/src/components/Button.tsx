import { forwardRef, ComponentPropsWithoutRef } from "react";
import { cn } from "../lib";
const IconButton = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button">
>(({ className, children, ...props }, ref) => {
    return (
        <button
            ref={ref}
            type="button"
            className={cn(
                "w-[36px] h-[36px]  rounded-full flex items-center justify-center bg-white/5",
                className
            )}
        >
            {children}
        </button>
    );
});

const ActionButton = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button">
>(({ children, className, type, form, ...props }, ref) => {
    return (
        <button
            type={type || "button"}
            form={form}
            className={cn(
                "bg-action-button rounded-3xl text-xl font-cnB py-[10px]",
                className
            )}
            {...props}
        >
            {children}
        </button>
    );
});

const NavButton = forwardRef<HTMLAnchorElement, ComponentPropsWithoutRef<"a">>(
    ({ children, className, ...props }, ref) => {
        return (
            <a
                className={cn(
                    "bg-action-button px-[2rem] inline-flex items-center justify-center whitespace-pre gap-1 rounded-full",
                    className
                )}
                {...props}
            >
                {children}
            </a>
        );
    }
);
export { IconButton, ActionButton, NavButton };
