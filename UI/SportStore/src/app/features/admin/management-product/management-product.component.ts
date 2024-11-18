import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Product, ProductStatus } from '../models/product.model';
import { ProductService } from '../services/product.service';
import { debounceTime, Subject } from 'rxjs';

@Component({
  selector: 'app-management-product',
  standalone: true,
  imports: [RouterLink, CommonModule, FormsModule],
  templateUrl: './management-product.component.html',
  styleUrl: './management-product.component.css'
})
export class ManagementProductComponent implements OnInit, AfterViewInit {
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  @ViewChild('customerTable') customerTable!: ElementRef<HTMLTableElement>;
  products: Product[] = [];
  productStatuses = ProductStatus;
  isLoading: boolean = true;
  isModalOpen: boolean = false;
  isDeleteModalOpen = false;
  isFinalConfirmation = false;
  searchTerm: string = '';
  searchTerm$ = new Subject<string>();
  selectedProduct: Product | null = null;
  newProduct: Product = this.initializeProduct();

  constructor(private productService: ProductService, private renderer: Renderer2) { }

  ngOnInit(): void {
    this.loadProducts();
    this.searchTerm$.pipe(debounceTime(300)).subscribe((term) => {
      this.searchTerm = term;
    });
  }

  onSearchInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm$.next(target.value);
  }

  loadProducts(): void {
    this.isLoading = true;
    this.productService.getAllProducts().subscribe((data) => {
      this.products = data;
      this.isLoading = false;
    });
  }

  filterProducts(): Product[] {
    if (!this.searchTerm.trim()) {
      return this.products;
    }
    return this.products.filter((product) =>
      product.productName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  initializeProduct(): Product {
    return {
      productId: '',
      productName: '',
      description: '',
      detail: '',
      images: [],
      price: 0,
      instock: 0,
      status: ProductStatus.Restocking,
      addDate: new Date(),
      brandId: '',
      categoryIds: []
    };
  }

  viewProduct(product: Product): void {
    this.selectedProduct = product;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedProduct = null;
  }

  openDeleteModal(product: Product): void {
    this.selectedProduct = product;
    this.isDeleteModalOpen = true;
    this.isFinalConfirmation = false;
  }

  closeDeleteModal(): void {
    this.isDeleteModalOpen = false;
    this.isFinalConfirmation = false;
  }

  proceedToFinalConfirmation(): void {
    this.isFinalConfirmation = true;
  }

  confirmDelete(): void {
    if (this.selectedProduct) {
      this.productService.deleteProduct(this.selectedProduct.productId).subscribe(
        (response) => {
          console.log('Delete response:', response);
          this.loadProducts();
          this.closeDeleteModal();
        },
        (error) => {
          console.error('Error deleting product:', error);
        }
      );
    }
  }

  private convertProductToFormData(product: Product): FormData {
    const formData = new FormData();
    formData.append('productName', product.productName);
    formData.append('description', product.description);
    formData.append('detail', product.detail);
    formData.append('price', product.price.toString());
    formData.append('instock', product.instock.toString());
    formData.append('status', product.status.toString());
    formData.append('addDate', product.addDate.toISOString());
    formData.append('brandId', product.brandId);

    if (product.categoryIds) {
      product.categoryIds.forEach((categoryId, index) => {
        formData.append(`categoryIds[${index}]`, categoryId);
      });
    }

    return formData;
  }

  ngAfterViewInit(): void {
    if (this.searchInput?.nativeElement) {
      this.renderer.listen(this.searchInput.nativeElement, 'input', () => {
        const searchValue = this.searchInput.nativeElement.value.toLowerCase();
        const rows = this.customerTable?.nativeElement.querySelectorAll('tbody tr') || [];

        rows.forEach((value: Element) => {
          const row = value as HTMLElement;
          const rowText = row.textContent?.toLowerCase() || '';
          this.renderer.setStyle(row, 'display', rowText.includes(searchValue) ? '' : 'none');
        });
      });
    }
  }
}
