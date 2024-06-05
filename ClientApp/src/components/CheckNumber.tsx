import { forwardRef, ComponentPropsWithoutRef, useState } from "react";
import { cn } from "../lib";
import { Keyboard } from "lucide-react";
import { motion, Variants } from "framer-motion";

const NumberInput = forwardRef<
    HTMLInputElement,
    ComponentPropsWithoutRef<"input">
>((props, ref) => {
    return <input ref={ref} className="number-input"></input>;
});

const InputPad = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div ref={ref} className="number-input-pad">
                {Array(5 * 6)
                    .fill(1)
                    .map(() => (
                        <NumberInput />
                    ))}
            </div>
        );
    }
);

const Number = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button"> & { value: number }
>(({ value, className, disabled, ...props }, ref) => {
    return (
        <button
            ref={ref}
            className={cn("number-button", className)}
            disabled={disabled ? true : false}
        >
            {value}
        </button>
    );
});

const MotionNumber = motion(Number);
const NumberPad = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div className="number-pad" ref={ref}>
                {Array(37)
                    .fill(1)
                    .map((_, index) => (
                        <MotionNumber value={index + 1} />
                    ))}
            </div>
        );
    }
);

const MotionNumberPad = motion(NumberPad);

const NumberPadButton = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button">
>((props, ref) => {
    const [expand, setExpand] = useState<boolean>(false);
    return (
        <>
            <button
                className="number-pad-button  rounded-full border-[2px] border-solid border-black/30 flex justify-center items-center"
                onClick={() => {
                    setExpand((v) => !v);
                }}
            >
                <Keyboard />
            </button>
        </>
    );
});
const CheckNumber = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <>
                <div className="w-full  flex flex-col items-center pt-[100px] gap-[50px]">
                    <InputPad />
                    <NumberPad />
                </div>
            </>
        );
    }
);
export { Number, NumberPad };

export default CheckNumber;
