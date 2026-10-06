import PageBreadcrumb from "../../app/components/common/PageBreadCrumb";
import ResponsiveImage from "../../app/components/ui/images/ResponsiveImage";
import TwoColumnImageGrid from "../../app/components/ui/images/TwoColumnImageGrid";
import ThreeColumnImageGrid from "../../app/components/ui/images/ThreeColumnImageGrid";
import ComponentCard from "../../app/components/common/ComponentCard";
import PageMeta from "../../app/components/common/PageMeta.jsx";

export default function Images() {
  return (
    <>
      <PageMeta
        title="React.js Images Dashboard | TailAdmin - React.js Admin Dashboard Template"
        description="This is React.js Images page for TailAdmin - React.js Tailwind CSS Admin Dashboard Template"
      />
      <PageBreadcrumb pageTitle="Images" />
      <div className="space-y-5 sm:space-y-6">
        <ComponentCard title="Responsive image">
          <ResponsiveImage />
        </ComponentCard>
        <ComponentCard title="Image in 2 Grid">
          <TwoColumnImageGrid />
        </ComponentCard>
        <ComponentCard title="Image in 3 Grid">
          <ThreeColumnImageGrid />
        </ComponentCard>
      </div>
    </>
  );
}
