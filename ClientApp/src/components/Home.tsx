import {
    FunctionComponent,
    ComponentPropsWithoutRef,
    forwardRef,
    useLayoutEffect,
    useRef,
    RefObject,
} from "react";
import { useMeasure } from "@uidotdev/usehooks";
import { motion } from "framer-motion";
import Nav from "./Nav";
import { NavButton } from "./Button";
import { ChevronRight } from "lucide-react";
import { FadeText } from "./magicui/fade-text";
import {
    BentoCard,
    BentoGrid,
    type BentoCardProps,
} from "./magicui/bento-grid";
import { cn } from "../lib";
import { NumberScroll } from "./NumberScroll";

const Tools: BentoCardProps[] = [
    {
        name: "当選チェッカ",
        description: "快速查看你的号码是否中奖",
        href: "/checknumber",
        cta: "使用工具",
        className: "",
    },
    {
        name: "当選番号",
        description: "查看历史中将号码",
        href: "/",
        cta: "使用工具",
        className: "",
    },
    {
        name: "公布号码",
        description: "查看他人号码",
        href: "/",
        cta: "使用工具",
        className: "",
    },
];

const ArrowIcon = forwardRef<SVGSVGElement, ComponentPropsWithoutRef<"svg">>(
    (props, ref) => {
        return (
            <svg
                className="mr-6 h-8 w-14 [transform:rotateY(180deg)rotateX(0deg)translateY(10px)translateX(-5px)]"
                width="45"
                height="25"
                viewBox="0 0 45 25"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
            >
                <path
                    d="M43.2951 3.47877C43.8357 3.59191 44.3656 3.24541 44.4788 2.70484C44.5919 2.16427 44.2454 1.63433 43.7049 1.52119L43.2951 3.47877ZM4.63031 24.4936C4.90293 24.9739 5.51329 25.1423 5.99361 24.8697L13.8208 20.4272C14.3011 20.1546 14.4695 19.5443 14.1969 19.0639C13.9242 18.5836 13.3139 18.4152 12.8336 18.6879L5.87608 22.6367L1.92723 15.6792C1.65462 15.1989 1.04426 15.0305 0.563943 15.3031C0.0836291 15.5757 -0.0847477 16.1861 0.187863 16.6664L4.63031 24.4936ZM43.7049 1.52119C32.7389 -0.77401 23.9595 0.99522 17.3905 5.28788C10.8356 9.57127 6.58742 16.2977 4.53601 23.7341L6.46399 24.2659C8.41258 17.2023 12.4144 10.9287 18.4845 6.96211C24.5405 3.00476 32.7611 1.27399 43.2951 3.47877L43.7049 1.52119Z"
                    fill="currentColor"
                    className="fill-gray-300 dark:fill-gray-700"
                ></path>
            </svg>
        );
    }
);

const ToolPointer = forwardRef<
    HTMLDivElement,
    ComponentPropsWithoutRef<"div"> & { container: RefObject<HTMLDivElement> }
>(({ className, container, ...prop }, ref) => {
    const scrollRef = useRef<HTMLDivElement>(null);
    return (
        <motion.div
            ref={scrollRef}
            animate={{ y: [null, -5, 5] }}
            transition={{
                duration: 3,
                repeat: Infinity,
                repeatType: "reverse",
            }}
            className={cn("flex flex-row", className)}
            onClick={() => {
                const { y } = scrollRef.current!.getBoundingClientRect();
                window.scrollBy(0, y - 18);
            }}
        >
            <span className="bg-action-button text-white font-semibold px-[1rem] rounded-full align-middle inline-block py-[0.5rem]">
                LOTO7ツール
            </span>
            <ArrowIcon />
        </motion.div>
    );
});

const PrimaryTitle = () => {
    return (
        <div className="text-text-primary text-5xl font-semibold  text-left  whitespace-nowrap  ">
            LOTO7 <br className="desktop:hidden" />
            ツールボックス
        </div>
    );
};
const FadePrimaryTitle = () => {
    return (
        <FadeText>
            <PrimaryTitle />
        </FadeText>
    );
};

const SecondaryTitle = () => {
    return (
        <div className="text-text-secondary text-balance text-center">
            ロト7は、1～37の数字の中から異なる7個の数字を選びます。 1等なら最高
            <div className="text-text-primary/80 font-bold inline"> 6億円 </div>
            （理論値）、キャリーオーバーがある場合、1等の当せん金は最高
            <div className="text-text-primary/80 font-bold inline">10億円</div>
            です。
            抽せんされた7個の「本数字」と2個の「ボーナス数字」が、自分が選んだ数字といくつ一致しているかで1等～6等までの当せんが決まります。
        </div>
    );
};

const ToolGrid = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <BentoGrid
                ref={ref}
                className="w-full max-w-[1500px] grid auto-rows-[12rem] grid-cols-1 desktop:grid-cols-3 min-h-[100vh] "
            >
                {Tools.map((i) => (
                    <BentoCard key={i.name} {...i} className="col-span-1" />
                ))}
                <div
                    className="col-span-1 desktop:col-span-3 flex justify-center underline text-text-primary/30 font-semibold underline-offset-[4px]"
                    onClick={() => {
                        window.scrollTo(0, 0);
                    }}
                >
                    <span>回到顶部</span>
                </div>
            </BentoGrid>
        );
    }
);

const Home: FunctionComponent<ComponentPropsWithoutRef<"div">> = (props) => {
    const [ref, { height }] = useMeasure();
    const toolRef = useRef<HTMLDivElement>(null);
    const containerRef = useRef<HTMLDivElement>(null);
    useLayoutEffect(() => {
        if (height == null || toolRef.current == null) {
            return;
        }
        toolRef.current.style.marginTop = `calc(100svh - ${height}px - 5rem)`;
    }, [height]);
    return (
        <div className="" ref={containerRef}>
            <div ref={ref} className="w-full flex flex-col items-center">
                <Nav />
                <div className="w-full min-h-[10rem]"></div>
                <FadePrimaryTitle />
                <div className="w-full min-h-[2rem]"></div>
                <SecondaryTitle />
                <div className="w-full min-h-[5rem]"></div>
                <NavButton className="hidden text-white text-xl font-semibold h-10">
                    浏览工具
                    <ChevronRight size="16" />
                </NavButton>
                <div className="w-full min-h-[2rem]"></div>
                <NumberScroll className="hidden desktop:grid" />
            </div>
            <div className="w-full flex flex-col items-center" ref={toolRef}>
                <ToolPointer container={containerRef} />
                <div className="w-full min-h-[20px]"></div>
                <ToolGrid />
            </div>
        </div>
    );
};

export default Home;
