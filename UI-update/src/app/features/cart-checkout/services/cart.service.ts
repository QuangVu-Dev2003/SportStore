import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { CartItem } from '../models/cart.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private apiUrl = 'https://localhost:7192/api/Carts';

  constructor(private http: HttpClient) { }

  addToCart(userId: string, cartItem: CartItem): Observable<any> {
    return this.http.post(`${this.apiUrl}/add-to-cart/${userId}`, cartItem)
      .pipe(
        catchError((error) => {
          console.error('Lỗi khi thêm sản phẩm vào giỏ hàng:', error);
          return throwError(() => new Error('Thêm sản phẩm thất bại!'));
        })
      );
  }

  getCartItems(userId: string): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.apiUrl}/get-cart/${userId}`);
  }

  removeFromCart(userId: string, productId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/remove-from-cart/${userId}/${productId}`);
  }

  updateCartItem(userId: string, updatedCartItem: CartItem): Observable<any> {
    // Gọi lại API addToCart với thông tin sản phẩm đã được cập nhật
    return this.addToCart(userId, updatedCartItem)
      .pipe(
        catchError((error) => {
          console.error('Lỗi khi cập nhật sản phẩm trong giỏ hàng:', error);
          return throwError(() => new Error('Cập nhật sản phẩm thất bại!'));
        })
      );
  }

  clearCart(userId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${userId}/clear`);
  }
}
