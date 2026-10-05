import {useState} from "react";
import {Link} from "react-router";
import {ChevronLeftIcon, EyeCloseIcon, EyeIcon} from "../../assets/icons";
import Label from "../forms/Label";
import Input from "../forms/input/InputField";
import FileInput from "../forms/Input/FileInput";

export default function SignUpForm() {
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    return (
        <div className="flex flex-col flex-1 w-full overflow-y-auto lg:w-1/2 no-scrollbar">
            <div className="w-full max-w-2xl mx-auto mb-5 sm:pt-10">
                <Link
                    to="/"
                    className="inline-flex items-center text-sm text-gray-500 transition-colors hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
                >
                    <ChevronLeftIcon className="size-5"/>
                    Back to dashboard
                </Link>
            </div>

            <div className="flex flex-col justify-center flex-1 w-full max-w-2xl mx-auto">
                <div>
                    <div className="mb-5 sm:mb-8">
                        <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
                            Đăng ký
                        </h1>

                        <p className="text-sm text-gray-500 dark:text-gray-400">
                            Nhập thông tin để tạo tài khoản
                        </p>
                    </div>

                    <div>
                        <form>
                            <div className="space-y-5">

                                {/* Tên tài khoản + Tên đăng nhập */}
                                <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
                                    <div>
                                        <Label>
                                            Tên tài khoản
                                            <span className="text-error-500">*</span>
                                        </Label>
                                        <Input
                                            type="text"
                                            id="fullName"
                                            name="fullName"
                                            placeholder="Nhập tên tài khoản"
                                        />
                                    </div>

                                    <div>
                                        <Label>
                                            Tên đăng nhập
                                            <span className="text-error-500">*</span>
                                        </Label>

                                        <Input
                                            type="text"
                                            id="username"
                                            name="username"
                                            placeholder="Nhập tên đăng nhập"
                                        />
                                    </div>
                                </div>

                                {/* Căn cước công dân */}
                                <div>
                                    <Label>
                                        Căn cước công dân
                                        <span className="text-error-500">*</span>
                                    </Label>

                                    <Input
                                        type="text"
                                        id="cccd"
                                        name="cccd"
                                        placeholder="Nhập số căn cước công dân"
                                    />
                                </div>

                                {/* Email */}
                                <div>
                                    <Label>
                                        Email
                                        <span className="text-error-500">*</span>
                                    </Label>

                                    <Input
                                        type="email"
                                        id="email"
                                        name="email"
                                        placeholder="Nhập email"
                                    />
                                </div>

                                <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
                                    {/* Mật khẩu */}
                                    <div>
                                        <Label>
                                            Mật khẩu
                                            <span className="text-error-500">*</span>
                                        </Label>

                                        <div className="relative">
                                            <Input
                                                id="password"
                                                name="password"
                                                placeholder="Nhập mật khẩu"
                                                type={showPassword ? "text" : "password"}
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

                                    {/* Xác nhận mật khẩu */}
                                    <div>
                                        <Label>
                                            Xác nhận mật khẩu
                                            <span className="text-error-500">*</span>
                                        </Label>

                                        <div className="relative">
                                            <Input
                                                id="confirmPassword"
                                                name="confirmPassword"
                                                placeholder="Nhập lại mật khẩu"
                                                type={showConfirmPassword ? "text" : "password"}
                                            />

                                            <span
                                                onClick={() =>
                                                    setShowConfirmPassword(!showConfirmPassword)
                                                }
                                                className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                                            >
                                                {showConfirmPassword ? (
                                                    <EyeIcon className="fill-gray-500 dark:fill-gray-400 size-5"/>
                                                ) : (
                                                    <EyeCloseIcon className="fill-gray-500 dark:fill-gray-400 size-5"/>
                                                )}
                                            </span>
                                        </div>
                                    </div>
                                </div>
                                <div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
                                    {/* CCCD mặt trước */}
                                    <div>
                                        <Label>
                                            CCCD mặt trước
                                            <span className="text-error-500">*</span>
                                        </Label>

                                        <FileInput
                                            id="cccdFront"
                                            name="cccdFront"
                                        />
                                    </div>

                                    {/* CCCD mặt sau */}
                                    <div>
                                        <Label>
                                            CCCD mặt sau
                                            <span className="text-error-500">*</span>
                                        </Label>

                                        <FileInput
                                            id="cccdBack"
                                            name="cccdBack"
                                        />
                                    </div>
                                </div>
                                {/* Button */}
                                <div>
                                    <button
                                        type="submit"
                                        className="flex items-center justify-center w-full px-4 py-3 text-sm font-medium text-white transition rounded-lg bg-brand-500 shadow-theme-xs hover:bg-brand-600"
                                    >
                                        Đăng ký
                                    </button>
                                </div>
                            </div>
                        </form>

                        <div className="mt-5">
                            <p className="text-sm font-normal text-center text-gray-700 dark:text-gray-400 sm:text-start">
                                Đã có tài khoản đăng ký?{" "}
                                <Link
                                    to="/signin"
                                    className="text-brand-500 hover:text-brand-600 dark:text-brand-400"
                                >
                                    Đăng nhập
                                </Link>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}