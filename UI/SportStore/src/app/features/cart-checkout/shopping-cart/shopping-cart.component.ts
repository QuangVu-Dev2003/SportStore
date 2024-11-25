import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../auth/services/auth.service';
import { CartService } from '../services/cart.service';
import { Router } from '@angular/router';
import { CartItem } from '../models/cart.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit {
  cartItems: CartItem[] = [];
  totalPrice: number = 0;
  constructor(private authService: AuthService, private cartService: CartService, private router: Router) { }

  ngOnInit(): void {
    this.loadCart();
  }
  loadCart(): void {
    if (!this.authService.isAuthenticated()) {
      alert('Vui lòng đăng nhập để xem giỏ hàng.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.authService.user().subscribe({
      next: (user) => {
        if (user) {
          this.cartService.getCartItems(user.id).subscribe({
            next: (items) => this.cartItems = items,
            error: (err) => console.error('Lỗi khi tải giỏ hàng:', err)
          });
        }
      },
      error: (err) => console.error('Lỗi khi lấy thông tin người dùng:', err)
    });
  }

  get totalCartPrice(): number {
    return this.cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  }

  removeItem(item: CartItem): void {
    this.authService.user().subscribe({
      next: (user) => {
        if (user) {
          this.cartService.removeFromCart(user.id, item.productId).subscribe({
            next: () => {
              this.cartItems = this.cartItems.filter(ci => ci.productId !== item.productId);
              console.log('Xoá sản phẩm thành công:', item);
            },
            error: (err) => console.error('Lỗi khi xóa sản phẩm:', err)
          });
        }
      },
      error: (err) => console.error('Lỗi khi lấy thông tin người dùng:', err)
    });
  }

  increaseQuantity(item: CartItem): void {
    // Tăng số lượng sản phẩm
    item.quantity++;
    this.updateCartItem(item);  // Gọi lại API cập nhật giỏ hàng với số lượng mới
  }

  decreaseQuantity(item: CartItem): void {
    // Giảm số lượng sản phẩm nhưng không để số lượng bé hơn 1
    if (item.quantity > 1) {
      item.quantity--;
      this.updateCartItem(item);  // Gọi lại API cập nhật giỏ hàng với số lượng mới
    }
  }


  updateCartItem(item: CartItem): void {
    console.log('Updating cart item', item); // Kiểm tra giá trị trước khi gửi lên backend
    this.authService.user().subscribe({
      next: (user) => {
        if (user) {
          this.cartService.addToCart(user.id, item).subscribe({
            next: () => console.log('Cập nhật giỏ hàng thành công:', item),
            error: (err) => console.error('Lỗi khi cập nhật giỏ hàng:', err)
          });
        }
      },
      error: (err) => console.error('Lỗi khi lấy thông tin người dùng:', err)
    });
  }

  confirmClearCart(): void {
    if (this.cartItems.length === 0) {
      alert('Giỏ hàng không có sản phẩm nào.');
      this.router.navigate(['/shop']);
      return;
    }
    const confirmDelete = window.confirm('Bạn có chắc chắn muốn xóa tất cả sản phẩm trong giỏ hàng không?');
    if (confirmDelete) {
      this.clearAllCartItems();
    }
  }

  clearAllCartItems(): void {
    this.authService.user().subscribe({
      next: (user) => {
        if (user) {
          this.cartService.clearCart(user.id).subscribe({
            next: () => {
              this.cartItems = [];
              this.loadCart();
              console.log('Giỏ hàng đã được xóa thành công!');
            },
            error: (err) => console.error('Lỗi khi xóa giỏ hàng:', err)
          });
        }
      },
      error: (err) => console.error('Lỗi khi lấy thông tin người dùng:', err)
    });
  }
}
