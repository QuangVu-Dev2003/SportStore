export enum BrandStatus {
    Pending = "Đợi xác nhận",
    Open = "Mở",
    Closed = "Đóng"
}

export interface Brand {
    brandId: string;
    brandName: string;
    description?: string;
    image?: string;
    status: BrandStatus;
}
