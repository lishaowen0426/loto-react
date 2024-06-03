import { FunctionComponent, ComponentPropsWithoutRef, forwardRef } from "react";
import Nav from "./Nav";
import { NavButton } from "./Button";
import { ChevronRight } from "lucide-react";
import { FadeText } from "./magicui/fade-text";
import { BentoCard, BentoGrid } from "./magicui/bento-grid";

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

const Tools = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return <BentoGrid ref={ref}></BentoGrid>;
    }
);

const Home: FunctionComponent<ComponentPropsWithoutRef<"div">> = (props) => {
    return (
        <div className="w-full px-[1rem]  flex flex-col items-center">
            <div className="w-full h-[1rem]"></div>
            <Nav />
            <div className="mt-[10rem] w-full flex flex-col items-center">
                <FadePrimaryTitle />
                <div className="w-full h-[2rem]"></div>
                <SecondaryTitle />
                <div className="w-full h-[5rem]"></div>
                <NavButton className="text-white text-xl font-semibold h-10">
                    浏览工具
                    <ChevronRight size="16" />
                </NavButton>
            </div>
            <Tools />
        </div>
    );
};

export default Home;
