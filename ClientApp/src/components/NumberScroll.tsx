import {
    forwardRef,
    ComponentPropsWithoutRef,
    useEffect,
    useRef,
    useCallback,
    useState,
    useMemo,
} from "react";
import { motion, useAnimate, AnimationSequence } from "framer-motion";

import { cn } from "../lib";
import { Divide } from "lucide-react";

const LOTO_COUNT = 9;

const Digit = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div"> & { sequence: any[]; isBonus?: boolean }
>(({ sequence, isBonus, className, ...props }, ref) => {
    const [{ from, to }, setValue] = useState<{
        from: any;
        to: any;
        index: number;
    }>({
        from: sequence[0],
        to: sequence[1],
        index: 0,
    });
    const [scope, animate] = useAnimate();

    const animationSequence: AnimationSequence = [
        [
            ".before-top",
            {
                rotateX: ["0deg", "90deg"],
                transformOrigin: ["bottom", "bottom"],
            },
            {
                duration: 0.5,
                ease: "easeIn",
                delay: 1,
            },
        ],
        [
            ".after-bottom",
            {
                rotateX: ["90deg", "0deg"],
                transformOrigin: ["top", "top"],
            },
            { duration: 0.5, ease: "easeInOut" },
        ],
    ];
    useEffect(() => {
        animate(animationSequence).then(() => {
            setValue(({ from: f, to: t, index: i }) => {
                const ii = (i + 1) % sequence.length;
                return {
                    from: sequence[ii],
                    to: sequence[(ii + 1) % sequence.length],
                    index: ii,
                };
            });
        });
    });
    return (
        <div
            ref={scope}
            className={cn("flip-card flip", className)}
            data-bonus={isBonus && "true"}
        >
            <div className="before-top">{from}</div>
            <div className="top">{to}</div>
            <div className="bottom">{from}</div>
            <div className="after-bottom">{to}</div>
        </div>
    );
});

const Group = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div"> & {
        sequence: any[];
        isBonus?: boolean;
    }
>(({ sequence, className, isBonus, ...props }, ref) => {
    const number_of_digits = useMemo(() => {
        return (sequence[0] + "").length;
    }, []);

    const getDigit = useCallback((v: any, d: number) => {
        return (v + "")[d];
    }, []);

    return (
        <div className={cn("flex flex-row flip-number w-fit", className)}>
            {Array(number_of_digits)
                .fill(1)
                .map((_, index) => {
                    return (
                        <Digit
                            key={`digit${index}`}
                            isBonus={isBonus}
                            sequence={sequence.map((n) => getDigit(n, index))}
                        />
                    );
                })}
        </div>
    );
});

const NumberScroll = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div">
>(({ className, ...props }, ref) => {
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

    return (
        <div
            className={cn(
                "number-scroll grid grid-rows-2 grid-cols-10 gap-[5px] w-fit",
                className
            )}
        >
            <Group
                sequence={["今回", "前回", "5月", "4月", "3月"]}
                className="row-start-2 col-start-1 col-end-2"
            />
            <div className="title row-start-1 col-start-2 col-end-9">
                本数字
            </div>
            <div className="title row-start-1 col-start-9 col-end-11">
                ボーナス
            </div>
            {Array(LOTO_COUNT)
                .fill(1)
                .map((_, index) => {
                    return (
                        <Group
                            className="row-start-2"
                            key={`number${index}`}
                            sequence={numbers.map((n) => n[index])}
                            isBonus={index === 7 || index === 8 ? true : false}
                        />
                    );
                })}
        </div>
    );
});

export { Group, Digit, NumberScroll };
