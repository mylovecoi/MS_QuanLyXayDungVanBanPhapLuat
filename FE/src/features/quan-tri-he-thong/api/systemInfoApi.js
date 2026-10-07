import {
    quanTriHeThongRequest,
} from "./../../../shared/api/quanTriHeThongApi";

const SYSTEM_INFO_PATH = "/he-thong/cau-hinh-he-thong";

export function getSystemInfo() {
    return quanTriHeThongRequest(SYSTEM_INFO_PATH);
}

export function saveSystemInfo(input) {
    return quanTriHeThongRequest(SYSTEM_INFO_PATH, {
        method: "PUT",
        body: input,
    });
}

