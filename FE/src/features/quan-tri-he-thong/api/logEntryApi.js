import {quanTriHeThongPagedRequest} from "../../../shared/api/quanTriHeThongApi";

const LOG_ENTRY_PATH = "/he-thong/nhat-ky-he-thong";

export function getLogEntries({
    search = "",
    pageSize = 10,
    pageCurrent = 1,
    fromDate = "",
    toDate = "",
} = {}) {
    const params = new URLSearchParams({
        pageSize: String(pageSize),
        pageCurrent: String(pageCurrent),
    });

    if (search.trim()) {
        params.set("search", search.trim());
    }

    if (fromDate) {
        params.set("fromDate", fromDate);
    }

    if (toDate) {
        params.set("toDate", toDate);
    }

    return quanTriHeThongPagedRequest(
        `${LOG_ENTRY_PATH}?${params.toString()}`
    );
}

