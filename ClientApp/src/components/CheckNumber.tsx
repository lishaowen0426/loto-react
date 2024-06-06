import {
    forwardRef,
    ComponentPropsWithoutRef,
    useState,
    Dispatch,
    SetStateAction,
    createContext,
    useContext,
    useLayoutEffect,
    useRef,
} from "react";
import { cn } from "../lib";
import { motion, Variants } from "framer-motion";
import { ArrowLeft, ArrowRight, Delete } from "lucide-react";
import { useMeasure } from "@uidotdev/usehooks";

const LOTO = 7;
const LOTO_GROUP = 5;

interface NumberPadCtx {
    setCursor: Dispatch<SetStateAction<number>>;
    cursor: number;
    setUserInput: Dispatch<SetStateAction<(number | undefined)[]>>;
    userInput: (number | undefined)[];
}

const NumberPadContext = createContext<NumberPadCtx>({} as NumberPadCtx);

const NumberInput = forwardRef<
    HTMLInputElement,
    ComponentPropsWithoutRef<"input"> & { isFocused?: boolean; index: number }
>(({ children, isFocused, index, ...props }, ref) => {
    const { setCursor } = useContext(NumberPadContext);
    return (
        <div
            ref={ref}
            className="number-input"
            data-status={isFocused && "focused"}
            onClick={() => {
                setCursor(index);
            }}
        >
            {children}
        </div>
    );
});

const InputPad = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    ({ ...props }, ref) => {
        const { cursor, userInput } = useContext(NumberPadContext);
        return (
            <div ref={ref} className="number-input-pad">
                {Array(LOTO_GROUP * LOTO)
                    .fill(1)
                    .map((_, index) => {
                        return (
                            <NumberInput
                                key={`numberinput${index}`}
                                isFocused={cursor == index}
                                index={index}
                            >
                                {userInput[index]}
                            </NumberInput>
                        );
                    })}
            </div>
        );
    }
);

const Number = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button"> & { value: number }
>(({ value, className, disabled, ...props }, ref) => {
    const { setUserInput, cursor, setCursor, userInput } =
        useContext(NumberPadContext);
    console.log(cursor);
    return (
        <button
            ref={ref}
            className={cn("number-button", className)}
            disabled={disabled ? true : false}
            onClick={() => {
                setUserInput((i) =>
                    i.map((v, index) => {
                        if (index == cursor) {
                            return value;
                        } else {
                            return v;
                        }
                    })
                );
                setCursor(() => {
                    for (let c = cursor + 1; c < LOTO_GROUP * LOTO; c++) {
                        if (userInput[c] == undefined) {
                            return c;
                        }
                    }
                    for (let c = 0; c < cursor; c++) {
                        if (userInput[c] == undefined) {
                            return c;
                        }
                    }
                    return cursor;
                });
            }}
        >
            {value}
        </button>
    );
});

const DeleteButton = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button">
>((props, ref) => {
    const { cursor, setUserInput } = useContext(NumberPadContext);
    return (
        <button
            ref={ref}
            className={cn("delete-button")}
            onClick={() => {
                setUserInput((i) =>
                    i.map((v, index) => {
                        if (index === cursor) {
                            return undefined;
                        } else {
                            return v;
                        }
                    })
                );
            }}
        >
            <Delete size="36" />
        </button>
    );
});

const NumberPad = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        const lastRowRef = useRef<HTMLDivElement>(null);
        const [measureRef, { height }] = useMeasure();
        const { setCursor, cursor, userInput } = useContext(NumberPadContext);
        const disabledNumber = userInput
            .slice(
                Math.floor(cursor / LOTO) * LOTO,
                Math.floor(cursor / LOTO) * LOTO + LOTO
            )
            .filter((v) => {
                return v != undefined;
            });

        useLayoutEffect(() => {
            if (height === null) {
                return;
            }
            console.log(height);
            lastRowRef.current!.style.height = `${height}px`;
        }, [height]);
        return (
            <div className="number-pad" ref={ref}>
                {Array(35)
                    .fill(1)
                    .map((_, index) => (
                        <Number
                            key={`number${index}`}
                            value={index + 1}
                            disabled={disabledNumber.indexOf(index + 1) != -1}
                        />
                    ))}
                <div className="last-row" ref={lastRowRef}>
                    <Number value={36} ref={measureRef} />
                    <Number value={37} />
                    <button
                        className="arrow-button"
                        onClick={() => {
                            setCursor(
                                (c) =>
                                    (c + LOTO_GROUP * LOTO - 1) %
                                    (LOTO_GROUP * LOTO)
                            );
                        }}
                    >
                        <ArrowLeft />
                    </button>
                    <button
                        className="arrow-button"
                        onClick={() => {
                            setCursor(
                                (c) =>
                                    (c + LOTO_GROUP * LOTO + 1) %
                                    (LOTO_GROUP * LOTO)
                            );
                        }}
                    >
                        <ArrowRight />
                    </button>
                    <DeleteButton />
                </div>
            </div>
        );
    }
);
const PostButtonGroup = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div">
>((props, ref) => {
    const { setUserInput, setCursor } = useContext(NumberPadContext);
    return (
        <div className="post-button-group">
            <button className="bg-green-500" onClick={() => {}}>
                チェック
            </button>
            <button
                className="bg-red-600"
                onClick={() => {
                    setUserInput(Array(LOTO_GROUP * LOTO).fill(undefined));
                    setCursor(0);
                }}
            >
                クリア
            </button>
        </div>
    );
});

const CheckNumberControll = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div">
>((props, ref) => {
    const [cursor, setCursor] = useState<number>(0);
    const [userInput, setUserInput] = useState<(number | undefined)[]>(
        Array(LOTO_GROUP * LOTO).fill(undefined)
    );
    return (
        <NumberPadContext.Provider
            value={{ setCursor, cursor, userInput, setUserInput }}
        >
            <div className="w-full  flex flex-col items-center pt-[100px] gap-[30px]">
                <InputPad />
                <NumberPad />
                <PostButtonGroup />
            </div>
        </NumberPadContext.Provider>
    );
});

const CheckNumber = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <>
                <CheckNumberControll />
            </>
        );
    }
);
export { Number, NumberPad };

export default CheckNumber;
