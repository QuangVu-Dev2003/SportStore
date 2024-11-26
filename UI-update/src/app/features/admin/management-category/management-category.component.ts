import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Category, CategoryStatus } from '../models/category.model';
import { debounceTime, Subject } from 'rxjs';
import { CategoryService } from '../services/category.service';
import { NgxPaginationModule } from 'ngx-pagination';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-management-category',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NgxPaginationModule],
  templateUrl: './management-category.component.html',
  styleUrl: './management-category.component.css'
})
export class ManagementCategoryComponent implements AfterViewInit, OnInit {
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  @ViewChild('customerTable') customerTable!: ElementRef<HTMLTableElement>;
  categories: Category[] = [];
  searchTerm: string = '';
  isLoading: boolean = true;
  searchTerm$ = new Subject<string>();
  selectedCategory: Category | null = null;
  isModalOpen: boolean = false;
  isEditModalOpen: boolean = false;
  editCategoryData: Category = {
    categoryId: '',
    categoryName: '',
    description: '',
    image: '',
    status: CategoryStatus.Pending
  };
  selectedFile: File | null = null;
  CategoryStatus = CategoryStatus;
  constructor(private renderer: Renderer2, private categoryService: CategoryService) { }
  currentPage: number = 1;

  ngOnInit(): void {
    this.loadCategories();

    this.searchTerm$.pipe(debounceTime(300)).subscribe((term) => {
      this.searchTerm = term;
    });
  }
  onSearchInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm$.next(target.value.trim());
  }

  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getAllCategories().subscribe(
      (data) => {
        this.categories = data;
        this.isLoading = false;
      },
      (error) => {
        console.error('Error loading brands:', error);
        this.isLoading = false;
      }
    );
  }

  filterCategories(): Category[] {
    if (!this.searchTerm.trim()) {
      return this.categories;
    }
    return this.categories.filter((category) =>
      category.categoryName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }
  viewCategory(category: Category): void {
    this.selectedCategory = category;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedCategory = null;
  }
  isAddModalOpen: boolean = false;
  newCategoryData: Category = {
    categoryId: '',
    categoryName: '',
    description: '',
    image: '',
    status: CategoryStatus.Pending,
  };

  openAddModal(): void {
    this.isAddModalOpen = true;
    this.newCategoryData = {
      categoryId: '',
      categoryName: '',
      description: '',
      image: '',
      status: CategoryStatus.Pending,
    };
  }

  closeAddModal(): void {
    this.isAddModalOpen = false;
  }

  // addCategory(event: Event): void {
  //   event.preventDefault();
  //   const formData = new FormData();
  //   formData.append('categoryName', this.newCategoryData.categoryName);
  //   formData.append('description', this.newCategoryData.description || '');
  //   formData.append('status', this.newCategoryData.status);

  //   if (this.selectedFile) {
  //     formData.append('image', this.selectedFile, this.selectedFile.name);
  //   }

  //   this.categoryService.createCategory(formData).subscribe(
  //     () => {
  //       this.loadCategories();
  //       this.closeAddModal();
  //     },
  //     (error) => {
  //       console.error('Error adding brand:', error);
  //     }
  //   );
  // }
  addCategory(event: Event): void {
    event.preventDefault();
    if (!this.newCategoryData.categoryName.trim()) {
      alert('Tên danh mục không được để trống. Vui lòng nhập tên hợp lệ.');
      return;
    }

    const isDuplicateName = this.categories.some(
      (category) =>
        category.categoryName.toLowerCase() ===
        this.newCategoryData.categoryName.toLowerCase()
    );

    if (isDuplicateName) {
      alert('Tên danh mục đã tồn tại. Vui lòng chọn tên khác.');
      return;
    }

    const formData = new FormData();
    formData.append('categoryName', this.newCategoryData.categoryName);
    formData.append('description', this.newCategoryData.description || '');
    formData.append('status', this.newCategoryData.status);

    // Thêm URL của ảnh vào formData
    if (this.newCategoryData.image) {
      formData.append('imageUrl', this.newCategoryData.image);
    }

    this.categoryService.createCategory(formData).subscribe(
      () => {
        this.loadCategories();
        this.closeAddModal();
      },
      (error) => {
        console.error('Error adding category:', error);
      }
    );
  }


  closeEditModal(): void {
    this.isEditModalOpen = false;
  }

  editCategory(category: Category): void {
    this.isEditModalOpen = true;
    this.editCategoryData = {
      ...category,
      status: category.status || CategoryStatus.Pending
    };
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  updateBrand(event: Event): void {
    console.log(this.categories)
    event.preventDefault();
    // if (!this.newCategoryData.categoryName) {
    //   alert('Tên danh mục không được để trống. Vui lòng nhập tên hợp lệ.');
    //   return;
    // }

    const isDuplicateName = this.categories.some(
      (category) =>
        category.categoryName.toLowerCase() ===
        this.newCategoryData.categoryName.toLowerCase()
    );

    if (isDuplicateName) {
      alert('Tên danh mục đã tồn tại. Vui lòng chọn tên khác.');
      return;
    }
    const formData = new FormData();
    formData.append('categoryName', this.editCategoryData.categoryName);
    formData.append('description', this.editCategoryData.description || '');
    formData.append('status', this.editCategoryData.status);

    if (this.selectedFile) {
      formData.append('image', this.selectedFile, this.selectedFile.name);
    }

    this.categoryService.updateCategory(this.editCategoryData.categoryId, formData).subscribe(
      () => {
        alert('Category updated successfully!');
        window.location.reload(); // Reload lại trang
      },
      (error) => {
        console.error('Error updating category:', error);
        alert('Failed to update category. Please try again.');
      }
    );
  }


  isDeleteModalOpen: boolean = false;
  isFinalConfirmation: boolean = false;
  categoryToDelete: Category | null = null;
  openDeleteModal(category: Category): void {
    this.isDeleteModalOpen = true;
    this.isFinalConfirmation = false;
    this.categoryToDelete = category;
  }
  closeDeleteModal(): void {
    this.isDeleteModalOpen = false;
    this.isFinalConfirmation = false;
    this.categoryToDelete = null;
  }
  proceedToFinalConfirmation(): void {
    this.isFinalConfirmation = true;
  }
  confirmDelete(): void {
    if (this.categoryToDelete) {
      this.categoryService.deleteCategory(this.categoryToDelete.categoryId).subscribe(
        () => {
          alert('Brand deleted successfully');
          location.reload(); // Reload lại trang
        },
        (error) => {
          console.error('Error deleting brand:', error);
          alert('An error occurred while deleting the brand. Please try again.');
        }
      );
    }
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

  exportToExcel(): void {
    const usersForExport = this.categories.map((category) => ({
      Category_ID: category.categoryId,
      Category_Name: category.categoryName,
      Description: category.description,
      IMG: category.image,
      status: category.status
    }));

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(usersForExport);

    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Users');

    XLSX.writeFile(wb, 'users.xlsx');
  }
}