export enum ProductStatus {
    InStock = 0, // "Còn Hàng"
    Restocking = 1, // "Đang Nhập Hàng"
    OutStock = 2, // "Hết Hàng"
    Discontinued = 3, // "Ngừng Bán"
}

export interface Product {
    productId: string;
    productName: string;
    description: string;
    detail: string;
    images: string[];
    price: number;
    instock: number;
    status: ProductStatus;
    addDate: Date;
    brandId: string;
    categoryIds: string[];
}
