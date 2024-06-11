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
    useCallback,
    useMemo,
    useEffect,
} from "react";
import { cn } from "../lib";
import { motion, Variants } from "framer-motion";
import { ArrowLeft, ArrowRight, Delete } from "lucide-react";
import { useMeasure } from "@uidotdev/usehooks";
import { useLoaderData } from "react-router-dom";
import { Confetti } from "./magicui/confetti";
import TypingAnimation from "./magicui/typing-animation";

const LOTO = 7;
const LOTO_GROUP = 5;

const FramerTypingAnimation = motion(TypingAnimation);

const MatchedIcon = () => {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="#22c50d"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            className="lucide lucide-check"
        >
            <path d="M20 6 9 17l-5-5" />
        </svg>
    );
};

const UnmatchedIcon = () => {
    return (
        <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="#fd3908"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            className="lucide lucide-x"
        >
            <path d="M18 6 6 18" />
            <path d="m6 6 12 12" />
        </svg>
    );
};

interface LotoNumber {
    bonusNumber1: number;
    bonusNumber2: number;
    carryOver: number;
    drawDate: Date;
    fifthPrizeAmount: number;
    fifthPrizeMouths: number;
    fourthPrizeAmount: number;
    fourthPrizeMouths: number;
    id: number;
    issue: "第578回";
    mouths: number;
    number1: number;
    number2: number;
    number3: number;
    number4: number;
    number5: number;
    number6: number;
    number7: number;
    prize: number;
    secondPrizeAmount: number;
    secondPrizeMouths: number;
    sixthPrizeAmount: number;
    sixthPrizeMouths: number;
    thirdPrizeAmount: number;
    thirdPrizeMouths: number;
    totalSales: number;
}

interface NumberPadCtx {
    setCursor: Dispatch<SetStateAction<number>>;
    cursor: number;
    setUserInput: Dispatch<SetStateAction<(number | undefined)[]>>;
    userInput: (number | undefined)[];
    filled: boolean[];
    setFilled: Dispatch<SetStateAction<boolean[]>>;
    lotoNumber: LotoNumber;
    matched: [number, number /* bonus */][];
    setMatched: Dispatch<SetStateAction<[number, number][]>>;
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

const ResultOverlay = () => {
    const { matched, setCursor, setFilled } = useContext(NumberPadContext);
    const [prize, setPrize] = useState<number>(-1);
    const removeOverlay = useCallback(() => {
        setCursor(0);
        setFilled(Array(LOTO_GROUP).fill(false));
    }, []);

    const findPrize = useCallback((userMatched: [number, number][]) => {
        if (
            userMatched.some(([m, _]) => {
                return m == 7;
            })
        ) {
            return 1;
        } else if (
            userMatched.some(([m, b]) => {
                return m == 6 && b == 1;
            })
        ) {
            return 2;
        } else if (
            userMatched.some(([m, b]) => {
                return m == 6 && b != 1;
            })
        ) {
            return 3;
        } else if (
            userMatched.some(([m, _]) => {
                return m == 5;
            })
        ) {
            return 4;
        } else if (
            userMatched.some(([m, _]) => {
                return m == 4;
            })
        ) {
            return 5;
        } else if (
            userMatched.some(([m, b]) => {
                return m == 3 && b >= 1;
            })
        ) {
            return 6;
        } else {
            return 0;
        }
    }, []);
    useEffect(() => {
        document.body.addEventListener("click", removeOverlay, {
            capture: true,
            once: true /* so we don't need to remove */,
        });
        const userPrize = findPrize(matched);
        if (userPrize > 0) {
            const end = Date.now() + 3 * 1000; // 3 seconds
            const colors = ["#a786ff", "#fd8bbc", "#eca184", "#f8deb1"];

            const frame = () => {
                if (Date.now() > end) return;

                Confetti({
                    particleCount: 2,
                    angle: 60,
                    spread: 55,
                    startVelocity: 60,
                    origin: { x: 0, y: 0.5 },
                    colors: colors,
                });
                Confetti({
                    particleCount: 2,
                    angle: 120,
                    spread: 55,
                    startVelocity: 60,
                    origin: { x: 1, y: 0.5 },
                    colors: colors,
                });

                requestAnimationFrame(frame);
            };

            frame();
        }
        setPrize(userPrize);
    }, [matched]);
    return (
        <div className="number-overlay flex flex-col justify-center">
            {prize > -1 &&
                (prize > 0 ? (
                    <FramerTypingAnimation
                        text={`${prize}等が当たりました!`}
                        style={{ color: "red" }}
                    />
                ) : (
                    <FramerTypingAnimation
                        text="残念ですが...."
                        style={{ color: "green" }}
                    />
                ))}
        </div>
    );
};

const FilledInput = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div"> & {
        numbers: (number | undefined)[];
        sequence: number;
        result?: string;
    }
>(({ numbers, sequence, result }, ref) => {
    const { setFilled } = useContext(NumberPadContext);
    return (
        <div
            className="filled-input"
            onClick={() => {
                setFilled((f) =>
                    f.map((v, i) => {
                        if (i == sequence) {
                            return false;
                        } else {
                            return v;
                        }
                    })
                );
            }}
        >
            {numbers.map((i, index) => (
                <div key={`filled-input-${index}`}>{i}</div>
            ))}
        </div>
    );
});

const InputPad = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    ({ ...props }, ref) => {
        const { cursor, userInput, filled, setFilled } =
            useContext(NumberPadContext);

        return (
            <div ref={ref} id="number-input-pad">
                {Array(LOTO_GROUP)
                    .fill(1)
                    .map((_, index) => {
                        if (filled[index]) {
                            return (
                                <FilledInput
                                    key={`filledinput${index}`}
                                    numbers={userInput.slice(
                                        index * LOTO,
                                        index * LOTO + LOTO
                                    )}
                                    sequence={index}
                                />
                            );
                        } else {
                            const offset = index * LOTO;
                            return Array(LOTO)
                                .fill(1)
                                .map((_, i) => {
                                    return (
                                        <NumberInput
                                            key={`numberinput${offset + i}`}
                                            isFocused={cursor == offset + i}
                                            index={offset + i}
                                        >
                                            {userInput[offset + i]}
                                        </NumberInput>
                                    );
                                });
                        }
                    })}
            </div>
        );
    }
);

const Number = forwardRef<
    HTMLButtonElement,
    ComponentPropsWithoutRef<"button"> & { value: number }
>(({ value, className, disabled, ...props }, ref) => {
    const { setUserInput, cursor, setCursor, userInput, setFilled } =
        useContext(NumberPadContext);
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
                const group = Math.floor(cursor / LOTO);
                setFilled((f) => {
                    return f.map((b, i) => {
                        let filled = true;
                        if (i != group) {
                            return b;
                        } else {
                            userInput
                                .slice(group * LOTO, (group + 1) * LOTO)
                                .forEach((v, ii) => {
                                    if (
                                        v === undefined &&
                                        group * LOTO + ii != cursor
                                    ) {
                                        filled = false;
                                    }
                                });
                            return filled;
                        }
                    });
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
    const {
        setUserInput,
        setCursor,
        userInput,
        setFilled,
        lotoNumber,
        setMatched,
    } = useContext(NumberPadContext);
    const onUserSubmit = useCallback((input: (number | undefined)[]) => {}, []);
    const onUserCheck = useCallback(
        (result: LotoNumber, input: (number | undefined)[]) => {
            console.log(result);
            const winningNumber = [
                result.number1,
                result.number2,
                result.number3,
                result.number4,
                result.number5,
                result.number6,
                result.number7,
            ];
            let matched = Array(LOTO_GROUP)
                .fill(1)
                .map(() => {
                    return new Array(2).fill(0);
                }) as [number, number][];
            for (let i = 0; i < input.length; i += LOTO) {
                const group = input.slice(i, i + LOTO);
                if (group.some((v) => v === undefined)) {
                    continue;
                }
                matched[Math.floor(i / LOTO)][0] = group.reduce(
                    (accum, current) => {
                        return ((accum as number) +
                            (winningNumber.includes(current as number)
                                ? 1
                                : 0)) as number;
                    },
                    0
                ) as number;
                //check bonus
                matched[Math.floor(i / LOTO)][1] =
                    (group.includes(result.bonusNumber1) ? 1 : 0) +
                    (group.includes(result.bonusNumber2) ? 1 : 0);
            }
            setMatched(matched);
            setCursor(-1); //display the overlay and result
        },
        []
    );
    return (
        <div className="post-button-group">
            <button
                className="bg-green-500"
                onClick={() => {
                    onUserCheck(lotoNumber, userInput);
                }}
            >
                チェック
            </button>
            <button
                className="bg-red-600"
                onClick={() => {
                    setUserInput(Array(LOTO_GROUP * LOTO).fill(undefined));
                    setCursor(0);
                    setFilled(Array(LOTO_GROUP).fill(false));
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
    let lotoNumber: LotoNumber = useLoaderData() as LotoNumber;
    const [cursor, setCursor] = useState<number>(0);
    const [userInput, setUserInput] = useState<(number | undefined)[]>(
        Array(LOTO_GROUP * LOTO).fill(undefined)
    );
    const [filled, setFilled] = useState<boolean[]>(
        Array(LOTO_GROUP).fill(false)
    );
    const [matched, setMatched] = useState<[number, number][]>(
        Array(LOTO_GROUP).fill([0, 0])
    );

    return (
        <NumberPadContext.Provider
            value={{
                setCursor,
                cursor,
                userInput,
                setUserInput,
                filled,
                setFilled,
                lotoNumber,
                matched,
                setMatched,
            }}
        >
            <div className="check-number w-full  flex flex-col items-center  justify-between pt-[20px] desktop:pt-[80px]">
                <div className="relative w-full max-w-[400px] desktop:max-w-[880px] flex flex-col gap-[20px] items-center  desktop:flex-row desktop:justify-between">
                    <InputPad />
                    <NumberPad />
                    {cursor == -1 && <ResultOverlay />}
                </div>
                <PostButtonGroup />
            </div>
        </NumberPadContext.Provider>
    );
});

const CheckNumber = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return <CheckNumberControll />;
    }
);
export { Number, NumberPad };

export default CheckNumber;
