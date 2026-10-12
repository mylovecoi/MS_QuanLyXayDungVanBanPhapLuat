const FALLBACK_COLOR = "#667085";

const NAMED_COLORS = {
  blue: "#2563EB",
  green: "#16A34A",
  red: "#DC2626",
  orange: "#EA580C",
  cyan: "#0891B2",
  purple: "#7C3AED",
  gray: FALLBACK_COLOR,
};

function normalizeColor(value) {
  const color = String(value || "").trim();
  if (/^#[0-9A-F]{6}$/i.test(color)) return color;
  if (/^#[0-9A-F]{3}$/i.test(color)) {
    return `#${color.slice(1).split("").map((part) => part + part).join("")}`;
  }
  return NAMED_COLORS[color.toLowerCase()] || FALLBACK_COLOR;
}

/** Hiển thị tên trạng thái với chính mã màu đã cấu hình trong Danh mục trạng thái. */
export default function TrangThaiBadge({ tenTrangThai, maMauHex, fallback = "Chưa thiết lập trạng thái" }) {
  const color = normalizeColor(maMauHex);

  return (
    <span
      className="inline-flex max-w-full items-center rounded-full border px-2.5 py-1 text-xs font-medium"
      style={{ backgroundColor: `${color}18`, borderColor: `${color}40`, color }}
      title={tenTrangThai || fallback}
    >
      <span className="truncate">{tenTrangThai || fallback}</span>
    </span>
  );
}
