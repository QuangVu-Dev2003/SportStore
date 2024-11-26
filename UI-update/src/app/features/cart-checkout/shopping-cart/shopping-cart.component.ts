import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../auth/services/auth.service';
import { CartService } from '../services/cart.service';
import { Router } from '@angular/router';
import { CartItem } from '../models/cart.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../services/order.service';

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
  constructor(private authService: AuthService, private cartService: CartService, private router: Router, private orderService: OrderService) { }

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
    item.quantity++;
    this.updateCartItem(item);
  }

  decreaseQuantity(item: CartItem): void {
    if (item.quantity > 1) {
      item.quantity--;
      this.updateCartItem(item);
    }
  }


  updateCartItem(item: CartItem): void {
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

  checkout(): void {
    if (this.cartItems.length === 0) {
      alert('Giỏ hàng trống, vui lòng thêm sản phẩm trước khi thanh toán.');
      return;
    }

    this.authService.user().subscribe({
      next: (user) => {
        if (user) {
          const order = {
            orderId: null,
            userId: user.id,
            orderDate: new Date().toISOString(),
            shippingAddress: "Địa chỉ của người dùng",
            totalAmount: this.totalCartPrice,
            status: "Pending",
            orderDetails: this.cartItems.map(item => ({
              productId: item.productId,
              productName: item.productName,
              quantity: item.quantity,
              unitPrice: item.price
            }))
          };

          this.orderService.checkout(order).subscribe({
            next: (response) => {
              const orderId = response.orderId;
              alert('Đặt hàng thành công! Bạn sẽ nhận được email xác nhận.');
              this.router.navigate(['/orders', orderId]);
            },
            error: (err) => {
              alert('Đã xảy ra lỗi khi đặt hàng.');
            }
          });
        }
      },
      error: (err) => {
        alert(err.error?.message || 'Đã xảy ra lỗi khi đặt hàng.');
      }
    });
  }
}
