import { Component, HostListener, OnInit } from '@angular/core';
import { CanComponentDeactivate } from '../../guards/can-deactivate.guard';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Brand } from '../../models/brand.model';
import { Category } from '../../models/category.model';
import { ProductService } from '../../services/product.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Product } from '../../models/product.model';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-edit-product',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './edit-product.component.html',
  styleUrl: './edit-product.component.css'
})
export class EditProductComponent implements OnInit, CanComponentDeactivate {
  productForm!: FormGroup;
  brands: Brand[] = [];
  categories: Category[] = [];
  selectedFiles: File[] = [];
  isFormDirty = false;
  productId!: string;


  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) { }


  ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.initForm();
    this.loadProductDetails();
    this.loadBrands();
    this.loadCategories();


    this.productForm.valueChanges.subscribe(() => {
      this.isFormDirty = true;
    });
  }


  private initForm(): void {
    this.productForm = this.fb.group({
      productName: ['', Validators.required],
      description: [''],
      detail: ['', Validators.required],
      price: [0, [Validators.required, Validators.min(0)]],
      instock: [0, [Validators.required, Validators.min(0)]],
      status: [0, Validators.required],
      addDate: ['', Validators.required],
      brandId: ['', Validators.required],
      categoryIds: [[], Validators.required]
    });
  }


  private loadProductDetails(): void {
    this.productService.getProductById(this.productId).subscribe((product: Product) => {
      this.productForm.patchValue({
        productName: product.productName,
        description: product.description,
        detail: product.detail,
        price: product.price,
        instock: product.instock,
        status: product.status,
        addDate: product.addDate,
        brandId: product.brandId,
        categoryIds: product.categoryIds
      });
    });
  }


  loadBrands(): void {
    this.productService.getAllBrands().subscribe((data) => {
      this.brands = data;
    });
  }


  loadCategories(): void {
    this.productService.getAllCategories().subscribe((data) => {
      this.categories = data;
    });
  }


  onFileSelected(event: any): void {
    this.selectedFiles = Array.from(event.target.files);
    this.isFormDirty = true;
  }


  onCategoryChange(event: Event): void {
    const selectedOptions = (event.target as HTMLSelectElement).selectedOptions;
    const selectedIds = Array.from(selectedOptions).map(option => option.value);


    this.productForm.controls['categoryIds'].setValue(selectedIds);
    this.isFormDirty = true;
  }


  submitForm(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }


    console.log('Dữ liệu gửi đi:', this.productForm.value);


    const formData = new FormData();
    const values = this.productForm.value;


    const selectedBrand = this.brands.find(brand => brand.brandId === values.brandId);
    const brandName = selectedBrand ? selectedBrand.brandName : '';


    const selectedCategoryNames = this.categories
      .filter(category => values.categoryIds.includes(category.categoryId))
      .map(category => category.categoryName);


    formData.append('productName', values.productName);
    formData.append('description', values.description);
    formData.append('detail', values.detail);
    formData.append('price', values.price.toString());
    formData.append('instock', values.instock.toString());
    formData.append('status', values.status.toString());
    formData.append('addDate', values.addDate);


    formData.append('brandName', brandName);


    selectedCategoryNames.forEach((name: string) => {
      formData.append('categoryNames', name);
    });


    this.selectedFiles.forEach((file) => {
      formData.append('imageFiles', file, file.name);
    });


    this.productService.updateProduct(this.productId, formData).subscribe(
      () => {
        alert('Cập nhật sản phẩm thành công!');
        this.router.navigate(['/admin/management-product']);
      },
      (error) => {
        console.error('Lỗi khi cập nhật sản phẩm:', error);
        alert('Cập nhật sản phẩm thất bại. Vui lòng kiểm tra lại thông tin!');
      }
    );


    this.isFormDirty = false;
  }


  canDeactivate(): boolean {
    if (this.isFormDirty) {
      return confirm('Bạn có muốn hủy chỉnh sửa sản phẩm không?');
    }
    return true;
  }


  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: BeforeUnloadEvent): void {
    if (this.isFormDirty) {
      $event.returnValue = 'Bạn có muốn hủy chỉnh sửa sản phẩm không?';
    }
  }
}





