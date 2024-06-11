import { forwardRef, ComponentPropsWithoutRef, useState } from "react";
import { IconButton } from "./Button";
import { CircleUser } from "lucide-react";
import { useForm, SubmitHandler } from "react-hook-form";
import validator from "validator";

import { Dialog, DialogContent, DialogTrigger } from "./dialog";
interface FormInput {
    email: string;
    password: string;
}

const Form = () => {
    const [isLogin, setLogin] = useState<boolean>(true);
    const { register, handleSubmit, formState } = useForm<FormInput>({
        mode: "onSubmit",
    });

    const onSubmit: SubmitHandler<FormInput> = (data) => {
        console.log(data);
        fetch("api/checknumbers", { method: "POST" });
    };

    return (
        <div className="flex flex-col min-w-[300px]">
            <div className="login-form-header flex flex-row w-full">
                <span
                    data-status={isLogin && "selected"}
                    onClick={(ev) => {
                        ev.preventDefault();
                        setLogin(true);
                    }}
                >
                    ログイン
                </span>
                <span
                    data-status={!isLogin && "selected"}
                    onClick={(ev) => {
                        ev.preventDefault();
                        setLogin(false);
                    }}
                >
                    登録
                </span>
            </div>
            <form className="login-form" onSubmit={handleSubmit(onSubmit)}>
                <input
                    type="text"
                    data-status={
                        !formState.isValid && formState.errors.email && "error"
                    }
                    placeholder="メールアドレス"
                    {...register("email", {
                        required: true,
                        validate: (v) => {
                            return validator.isEmail(v);
                        },
                    })}
                />
                <input
                    type="password"
                    data-status={
                        !formState.isValid &&
                        formState.errors.password &&
                        "error"
                    }
                    placeholder="パスワード"
                    {...register("password", {
                        required: true,
                        validate: (v) => {
                            if (
                                validator.isAlpha(v) ||
                                !validator.isAlphanumeric(v)
                            ) {
                                return false;
                            }

                            if (!validator.isHalfWidth(v)) {
                                return false;
                            }
                            if (!validator.isLength(v, { min: 8, max: 16 })) {
                                return false;
                            }
                            return true;
                        },
                    })}
                />
                <ul
                    className="list-disc list-inside"
                    data-status={
                        !formState.isValid &&
                        formState.errors.password &&
                        "error"
                    }
                >
                    密码需符合
                    <li>长度8到18位</li>
                    <li>包含数字和字母</li>
                </ul>
                <div className="button-group">
                    <button
                        onClick={(ev) => {
                            ev.preventDefault();
                        }}
                    >
                        お忘れの場合
                    </button>
                </div>
                <button type="submit">{isLogin ? "ログイン" : "登録"}</button>
            </form>
        </div>
    );
};

const Login = () => {
    return (
        <Dialog>
            <DialogTrigger asChild>
                <button>
                    <CircleUser />
                </button>
            </DialogTrigger>
            <DialogContent className="w-fit max-w-[90%] rounded-md">
                <Form />
            </DialogContent>
        </Dialog>
    );
};

export default Login;
