import PageBreadcrumb from "../components/common/PageBreadCrumb";
import UserMetaCard from "../components/UserProfile/UserMetaCard.jsx";
import UserInfoCard from "../components/UserProfile/UserInfoCard.jsx";
import UserAddressCard from "../components/UserProfile/UserAddressCard.jsx";
import PageMeta from "../components/common/PageMeta.jsx";

export default function UserProfiles() {
  return (
    <>
      <PageMeta
        title="Thông tin tài khoản"
        description="Trang thông tin tài khoản"
      />
      <PageBreadcrumb pageTitle="THÔNG TIN TÀI KHOẢN" />
      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] lg:p-6">
        <h3 className="mb-5 text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-7">
          THÔNG TIN TÀI KHOẢN
        </h3>
        <div className="space-y-6">
          <UserMetaCard />
          <UserInfoCard />
          <UserAddressCard />
        </div>
      </div>
    </>
  );
}
