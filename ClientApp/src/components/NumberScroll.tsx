import {
    forwardRef,
    ComponentPropsWithoutRef,
    useEffect,
    useImperativeHandle,
    useRef,
    RefObject,
    useState,
} from "react";

import { cn } from "../lib";
import { motion, useAnimate } from "framer-motion";

interface DigitRef {
    top: RefObject<HTMLSpanElement>;
    topSlide: RefObject<HTMLSpanElement>;
    bottom: RefObject<HTMLDivElement>;
    bottomSlide: RefObject<HTMLDivElement>;
}

const Digit = forwardRef<
    DigitRef,
    ComponentPropsWithoutRef<"div"> & { from: number; to: number }
>(({ from, to, ...props }, ref) => {
    const [scope, animate] = useAnimate();
    const topRef = useRef<HTMLSpanElement>(null);
    const topSlideRef = useRef<HTMLSpanElement>(null);
    const bottomRef = useRef<HTMLDivElement>(null);
    const bottomSlideRef = useRef<HTMLDivElement>(null);
    useImperativeHandle(
        ref,
        () => {
            return {
                top: topRef,
                topSlide: topSlideRef,
                bottom: bottomRef,
                bottomSlide: bottomSlideRef,
            };
        },
        []
    );
    return (
        <div ref={scope}>
            <span ref={topRef} className={cn("top hidden")}>
                {from}
            </span>
            <span ref={topSlideRef} className={cn("top-slide hidden")}>
                {from}
            </span>
            <div ref={bottomRef} className={cn("bottom-slide hidden")}>
                <span>{to}</span>
            </div>
            <div ref={bottomSlideRef} className={cn("bottom")}>
                <span>{from}</span>
            </div>
        </div>
    );
});

const Figure = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        const digitRef = useRef<DigitRef>(null);
        const containerRef = useRef<HTMLDivElement>(null);
        const [value, setValue] = useState<number>(9);
        useEffect(() => {
            const interval = setInterval(() => {
                setValue((v) => {
                    if (v == 0) {
                        return 9;
                    } else {
                        return v - 1;
                    }
                });
            }, 1000);

            return () => {
                clearInterval(interval);
            };
        }, []);

        return (
            <div className={cn("figure")} ref={containerRef}>
                <Digit ref={digitRef} from={5} to={8} />
            </div>
        );
    }
);

export { Figure };
