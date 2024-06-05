import {
    forwardRef,
    ComponentPropsWithoutRef,
    useEffect,
    useRef,
    useState,
} from "react";

import { cn } from "../lib";

const Digit = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div"> & { from: number; to: number }
>(({ from, to, ...props }, ref) => {
    useEffect(() => {}, []);
    return (
        <div ref={ref} className="flip-card flip">
            <div className="before-top">{from}</div>
            <div className="top">{to}</div>
            <div className="bottom">{from}</div>
            <div className="after-bottom">{to}</div>
        </div>
    );
});

const Number = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div"> & {
        from: number;
        to: number;
    }
>(({ from, to, className, ...props }, ref) => {
    const from1 = from % 10;
    const from2 = Math.floor(from / 10);
    const to1 = to % 10;
    const to2 = Math.floor(to / 10);
    return (
        <div className={cn("flex flex-row gap-[3px]", className)}>
            <Digit from={from2} to={to2} />
            <Digit from={from1} to={to1} />
        </div>
    );
});

const NumberScroll = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div">
>((props, ref) => {
    const [numbers, setNumbers] = useState<number[][]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        setTimeout(() => {
            setNumbers([
                [21, 12, 34, 13, 23, 21, 23, 11, 37],
                [32, 12, 32, 36, 37, 23, 28, 29, 20],
                [21, 23, 25, 26, 35, 34, 11, 23, 25],
                [18, 19, 25, 24, 13, 11, 26, 23, 19],
                [27, 28, 31, 23, 26, 18, 34, 11, 31],
            ]);
            setIsLoading(false);
        }, 1000);
    }, []);

    if (isLoading) {
        return <div></div>;
    }

    return <div className="grid "></div>;
});

export { Number, Digit };
