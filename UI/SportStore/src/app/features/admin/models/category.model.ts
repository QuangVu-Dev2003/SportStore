export enum CategoryStatus {
    Pending = "Đợi xác nhận",
    Open = "Mở",
    Closed = "Đóng"
}

export interface Category {
    categoryId: string;
    categoryName: string;
    description?: string;
    image?: string;
    status: CategoryStatus;
}
