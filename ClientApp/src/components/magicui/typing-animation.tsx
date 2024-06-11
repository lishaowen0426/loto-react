"use client";

import { cn } from "../../lib/utils";
import { useEffect, useState, forwardRef } from "react";
import CSS from "csstype";

interface TypingAnimationProps {
    text: string;
    duration?: number;
    className?: string;
    style?: CSS.Properties;
}

const TypingAnimation = forwardRef<HTMLHeadingElement, TypingAnimationProps>(
    ({ text, duration = 200, className, style }, ref) => {
        const [displayedText, setDisplayedText] = useState<string>("");
        const [i, setI] = useState<number>(0);

        useEffect(() => {
            const typingEffect = setInterval(() => {
                if (i < text.length) {
                    setDisplayedText((prevState) => prevState + text.charAt(i));
                    setI(i + 1);
                } else {
                    clearInterval(typingEffect);
                }
            }, duration);

            return () => {
                clearInterval(typingEffect);
            };
        }, [duration, i]);

        return (
            <h1
                ref={ref}
                style={style}
                className={cn(
                    "font-display text-center text-4xl font-bold leading-[5rem] tracking-[-0.02em] drop-shadow-sm",
                    className
                )}
            >
                {displayedText ? displayedText : text}
            </h1>
        );
    }
);

export default TypingAnimation;
