import { forwardRef, ComponentPropsWithoutRef } from "react";
const NumberTable = forwardRef<HTMLDivElement, ComponentPropsWithoutRef<"div">>(
    (props, ref) => {
        return (
            <div id="number-table">
                <div>1</div>
                <div>2</div>
            </div>
        );
    }
);

export default NumberTable;
