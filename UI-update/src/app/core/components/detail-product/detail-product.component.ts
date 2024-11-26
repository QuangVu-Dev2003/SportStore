import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../features/admin/services/product.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Product } from '../../../features/admin/models/product.model';
import { AuthService } from '../../../features/auth/services/auth.service';
import { CartService } from '../../../features/cart-checkout/services/cart.service';
import { CartItem } from '../../../features/cart-checkout/models/cart.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-detail-product',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './detail-product.component.html',
  styleUrl: './detail-product.component.css'
})
export class DetailProductComponent implements OnInit {
  product: Product | null = null;
  quantity: number = 1;
  thumbImages: string[] = [];

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private authService: AuthService,
    private cartService: CartService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    if (productId) {
      this.loadProduct(productId);
    }
  }

  loadProduct(id: string): void {
    this.productService.getProductById(id).subscribe({
      next: (data) => {
        this.product = data;
        if (this.product && this.product.images) {
          this.thumbImages = this.product.images.map((img) => `${img}`);
        }
      }
    });
  }

  onErrorImg(event: Event): void {
    (event.target as HTMLImageElement).src =
      'https://static.vecteezy.com/system/resources/previews/035/851/656/non_2x/football-player-logo-design-illustration-vector.jpg';
  }

  increaseQuantity(): void {
    this.quantity++;
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addToCart(): void {
    if (!this.authService.isAuthenticated()) {
      alert('Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.authService.user().subscribe({
      next: (user) => {
        if (user && this.product) {
          if (!this.product.images || this.product.images.length === 0) {
            alert('Hình ảnh sản phẩm không hợp lệ!');
            return;
          }

          const cartItem: CartItem = {
            productId: this.product.productId,
            productName: this.product.productName,
            quantity: this.quantity,
            price: this.product.price,
            imageUrl: this.product.images[0],
          };

          this.cartService.addToCart(user.id, cartItem).subscribe({
            next: () => {
              alert('Sản phẩm đã được thêm vào giỏ hàng!');
              console.log('Thêm sản phẩm thành công:', cartItem);
            },
            error: (err) => {
              console.error('Thêm giỏ hàng thất bại:', err);
              alert('Lỗi khi thêm sản phẩm vào giỏ hàng!');
            }
          });
        } else {
          alert('Không tìm thấy thông tin người dùng!');
        }
      },
      error: (err) => {
        console.error('Lỗi khi lấy thông tin người dùng:', err);
      }
    });
  }
}