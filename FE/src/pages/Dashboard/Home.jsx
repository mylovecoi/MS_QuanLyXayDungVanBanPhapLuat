import EcommerceMetrics from "../../app/components/ecommerce/EcommerceMetrics.jsx";
import MonthlySalesChart from "../../app/components/ecommerce/MonthlySalesChart.jsx";
import StatisticsChart from "../../app/components/ecommerce/StatisticsChart.jsx";
import MonthlyTarget from "../../app/components/ecommerce/MonthlyTarget.jsx";
import RecentOrders from "../../app/components/ecommerce/RecentOrders.jsx";
import DemographicCard from "../../app/components/ecommerce/DemographicCard.jsx";
import PageMeta from "../../app/components/common/PageMeta.jsx";

export default function Home() {
  return (
    <>
      <PageMeta
        title="Base Template by Hoang"
        description="This is React.js Base Template by Hoang"
      />
      <div className="grid grid-cols-12 gap-4 md:gap-6">
        <div className="col-span-12 space-y-6 xl:col-span-7">
          <EcommerceMetrics />
          <MonthlySalesChart />
        </div>

        <div className="col-span-12 xl:col-span-5">
          <MonthlyTarget />
        </div>

        <div className="col-span-12">
          <StatisticsChart />
        </div>

        <div className="col-span-12 xl:col-span-5">
          <DemographicCard />
        </div>

        <div className="col-span-12 xl:col-span-7">
          <RecentOrders />
        </div>
      </div>
    </>
  );
}
