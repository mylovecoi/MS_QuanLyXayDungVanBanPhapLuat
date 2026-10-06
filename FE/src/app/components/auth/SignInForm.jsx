import {useState} from "react";
import {Link} from "react-router";
import {ChevronLeftIcon, EyeCloseIcon, EyeIcon} from "../../../assets/icons/index.js";
import Label from "../forms/Label";
import Input from "../forms/input/InputField";
import Button from "../ui/button/Button.jsx";

export default function SignInForm() {
    const [showPassword, setShowPassword] = useState(false);
    const [isChecked, setIsChecked] = useState(false);

    return (
        <div className="flex flex-col flex-1">
            <div className="w-full max-w-xl pt-10 mx-auto">
                <Link
                    to="/"
                    className="inline-flex items-center text-sm text-gray-500 transition-colors hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
                >
                    <ChevronLeftIcon className="size-5"/>
                    Back to dashboard
                </Link>
            </div>
            <div className="flex flex-col justify-center flex-1 w-full max-w-xl mx-auto">
                <div>
                    <div className="mb-5 sm:mb-8">
                        <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                            Đăng nhập
                        </h1>
                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            Vui lòng nhập đầy đủ thông tin đăng nhập!
                        </p>
                    </div>
                    <div>
                        <form>
                            <div className="space-y-6">
                                <div>
                                    <Label>
                                        Email <span className="text-error-500">*</span>{" "}
                                    </Label>
                                    <Input placeholder="info@gmail.com"/>
                                </div>
                                <div>
                                    <Label>
                                        Mật khẩu <span className="text-error-500">*</span>{" "}
                                    </Label>
                                    <div className="relative">
                                        <Input
                                            type={showPassword ? "text" : "password"}
                                            placeholder="Nhập mật khẩu"
                                        />
                                        <span
                                            onClick={() => setShowPassword(!showPassword)}
                                            className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                                        >
                                              {showPassword ? (
                                                  <EyeIcon className="fill-gray-500 dark:fill-gray-400 size-5"/>
                                              ) : (
                                                  <EyeCloseIcon className="fill-gray-500 dark:fill-gray-400 size-5"/>
                                              )}
                                        </span>
                                    </div>
                                </div>

                                <div>
                                    <Button className="w-full" size="sm">
                                        Đăng nhập
                                    </Button>
                                </div>
                            </div>
                        </form>

                        <div className="flex mt-5 items-center justify-between text-sm font-normal text-gray-700 dark:text-gray-400">
                            <Link
                                to="/signup"
                                className="text-brand-500 hover:text-brand-600 dark:text-brand-400"
                            >
                                Đăng ký
                            </Link>

                            <Link
                                to="/reset-password"
                                className="text-brand-500 hover:text-brand-600 dark:text-brand-400"
                            >
                                Quên mật khẩu?
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
