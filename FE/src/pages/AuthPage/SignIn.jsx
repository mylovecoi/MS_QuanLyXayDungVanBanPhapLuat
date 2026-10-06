import PageMeta from "../../app/components/common/PageMeta";
import AuthLayout from "./AuthPageLayout";
import SignInForm from "../../app/components/auth/SignInForm";

export default function SignIn() {
    return (
        <>
            <PageMeta
                title="Đăng nhập"
                description="Đăng nhập"
            />
            <AuthLayout>
                <SignInForm />
            </AuthLayout>
        </>
    );
}
