import {useState} from "react";
import {Link, useNavigate} from "react-router";
import {
    ChevronLeftIcon,
    EyeCloseIcon,
    EyeIcon
} from "../../../assets/icons/index.js";
import Label from "../forms/Label";
import Input from "../forms/input/InputField";
import Button from "../ui/button/Button.jsx";
import {loginApi} from "../../../shared/api/authApi.js";

const DEV_CREDENTIALS = import.meta.env.DEV
    ? {username: "sa", password: "Cs@2012!"}
    : {username: "", password: ""};

export default function SignInForm() {
    const navigate = useNavigate();

    const [showPassword, setShowPassword] = useState(false);
    const [username, setUsername] = useState(DEV_CREDENTIALS.username);
    const [password, setPassword] = useState(DEV_CREDENTIALS.password);
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        setError("");

        if (!username.trim() || !password) {
            setError("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.");
            return;
        }

        try {
            setLoading(true);

            const response = await loginApi(
                username.trim(),
                password
            );

            console.log("LOGIN RESPONSE:", response);

            if (!response.isSuccess || !response.data?.accessToken) {
                setError(
                    response.message || "Đăng nhập thất bại."
                );
                return;
            }

            // Lưu access token
            localStorage.setItem(
                "accessToken",
                response.data.accessToken
            );

            // Lưu thông tin user
            localStorage.setItem(
                "userInfo",
                JSON.stringify({
                    userId: response.data.userId,
                    username: response.data.username,
                    displayName: response.data.displayName,
                    donViId: response.data.donViId,
                    groupPermissionId: response.data.groupPermissionId,
                    isSSA: response.data.isSSA,
                    firstLogin: response.data.firstLogin,
                    mustChangePassword: response.data.mustChangePassword,
                })
            );

            // Chuyển vào trang chủ
            navigate("/");
        } catch (error) {
            console.error("Login error:", error);

            setError(
                error.response?.data?.message ||
                error.message ||
                "Đăng nhập thất bại."
            );
        } finally {
            setLoading(false);
        }
    };

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
                        <form onSubmit={handleSubmit}>
                            <div className="space-y-6">

                                {/* Username */}
                                <div>
                                    <Label>
                                        Tên đăng nhập{" "}
                                        <span className="text-error-500">*</span>
                                    </Label>

                                    <Input
                                        value={username}
                                        onChange={(e) => setUsername(e.target.value)}
                                        placeholder="Nhập tên đăng nhập"
                                    />
                                </div>

                                {/* Password */}
                                <div>
                                    <Label>
                                        Mật khẩu{" "}
                                        <span className="text-error-500">*</span>
                                    </Label>

                                    <div className="relative">
                                        <Input
                                            type={showPassword ? "text" : "password"}
                                            value={password}
                                            onChange={(e) => setPassword(e.target.value)}
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

                                {/* Error */}
                                {error && (
                                    <p className="text-sm text-error-500">
                                        {error}
                                    </p>
                                )}

                                {/* Submit */}
                                <div>
                                    <Button
                                        type="submit"
                                        className="w-full"
                                        size="sm"
                                        disabled={loading}
                                    >
                                        {loading ? "Đang đăng nhập..." : "Đăng nhập"}
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
