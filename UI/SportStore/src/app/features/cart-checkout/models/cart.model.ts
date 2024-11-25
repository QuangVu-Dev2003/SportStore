export interface CartItem {
    productId: string;
    productName: string;
    quantity: number;
    price: number;
    imageUrl: string;
}

export interface Cart {
    id: string;
    userId: string;
    items: CartItem[];
}
