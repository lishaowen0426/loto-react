import { forwardRef, ComponentPropsWithoutRef } from "react";
import { cn } from "../lib";
import { Keyboard } from "lucide-react";

const Number = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button"> & { value: number }
>(({ value, className, disabled, ...props }, ref) => {
    return (
        <button
            className={cn("number-button", className)}
            disabled={disabled ? true : false}
        >
            {value}
        </button>
    );
});
const NumberPad = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div className="number-pad">
                {Array(37)
                    .fill(1)
                    .map((_, index) => (
                        <Number value={index + 1} />
                    ))}
            </div>
        );
    }
);

const NumberPadButton = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button">
>((props, ref) => {
    return (
        <button className="w-[48px] h-[48px] rounded-full border-[2px] border-solid border-black/30 flex justify-center items-center sticky left-[100px] top-[500px]">
            <Keyboard />
        </button>
    );
});
const CheckNumber = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div className="w-full min-h-[100vh] overflow-auto">
                <NumberPadButton />
            </div>
        );
    }
);
export { Number, NumberPad };

export default CheckNumber;
