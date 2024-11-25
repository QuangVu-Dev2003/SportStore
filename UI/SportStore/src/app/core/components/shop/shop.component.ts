import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Product } from '../../../features/admin/models/product.model';
import { ProductService } from '../../../features/admin/services/product.service';
import { CommonModule } from '@angular/common';
import { Brand } from '../../../features/admin/models/brand.model';
import { Category } from '../../../features/admin/models/category.model';
import { BrandService } from '../../../features/admin/services/brand.service';
import { CategoryService } from '../../../features/admin/services/category.service';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../features/auth/services/auth.service';
import { CartService } from '../../../features/cart-checkout/services/cart.service';
import { CartItem } from '../../../features/cart-checkout/models/cart.model';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.css']
})
export class ShopComponent implements OnInit {
  products: Product[] = [];
  displayedProducts: Product[] = [];
  brands: Brand[] = [];
  categories: Category[] = [];
  selectedBrands: string[] = [];
  selectedCategories: string[] = [];
  product: Product | null = null;
  quantity: number = 1;
  currentPage: number = 1;
  itemsPerPage: number = 1;
  totalItems: number = 0;

  constructor(
    private productService: ProductService,
    private brandService: BrandService,
    private categoryService: CategoryService,
    private authService: AuthService,
    private router: Router,
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.loadProducts();
    this.loadBrands();
    this.loadCategories();
  }

  loadProducts(): void {
    this.productService.getAllProducts().subscribe((data) => {
      this.products = data;
      this.totalItems = data.length;
      this.updateDisplayedProducts();
    });
  }

  loadBrands(): void {
    this.brandService.getAllBrands().subscribe((data) => {
      this.brands = data;
    });
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe((data) => {
      this.categories = data;
    });
  }

  searchProducts(): void {
    const categoryFilter = this.selectedCategories.length > 0 ? this.selectedCategories : undefined;
    const brandFilter = this.selectedBrands.length > 0 ? this.selectedBrands : undefined;

    this.productService.searchProducts(categoryFilter, brandFilter).subscribe((data) => {
      this.products = data;
      this.totalItems = data.length;
      this.updateDisplayedProducts();
    });
  }

  updateDisplayedProducts(): void {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    this.displayedProducts = this.products.slice(startIndex, endIndex);
  }

  changePage(page: number): void {
    this.currentPage = page;
    this.updateDisplayedProducts();
  }

  get totalPages(): number[] {
    return Array(Math.ceil(this.totalItems / this.itemsPerPage))
      .fill(0)
      .map((_, i) => i + 1);
  }

  clearFilters(): void {
    this.selectedBrands = [];
    this.selectedCategories = [];
    this.loadProducts();
  }

  toggleBrandSelection(brandName: string): void {
    const index = this.selectedBrands.indexOf(brandName);
    if (index > -1) {
      this.selectedBrands.splice(index, 1); // Bỏ chọn
    } else {
      this.selectedBrands.push(brandName); // Chọn
    }
    this.onFilterChange();
  }

  toggleCategorySelection(categoryName: string): void {
    const index = this.selectedCategories.indexOf(categoryName);
    if (index > -1) {
      this.selectedCategories.splice(index, 1);
    } else {
      this.selectedCategories.push(categoryName);
    }
    this.onFilterChange();
  }

  onFilterChange(): void {
    this.searchProducts();
  }

  onErrorImg(event: Event): void {
    (event.target as HTMLImageElement).src =
      'https://static.vecteezy.com/system/resources/previews/035/851/656/non_2x/football-player-logo-design-illustration-vector.jpg';
  }

  addToCart(product: Product): void {
    if (!this.authService.isAuthenticated()) {
      alert('Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.');
      this.router.navigate(['/login'], { queryParams: { returnUrl: this.router.url } });
      return;
    }

    this.authService.user().subscribe({
      next: (user) => {
        if (user && product) {
          if (!product.images || product.images.length === 0) {
            alert('Hình ảnh sản phẩm không hợp lệ!');
            return;
          }

          const cartItem: CartItem = {
            productId: product.productId,
            productName: product.productName,
            quantity: this.quantity,
            price: product.price,
            imageUrl: product.images[0],
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
