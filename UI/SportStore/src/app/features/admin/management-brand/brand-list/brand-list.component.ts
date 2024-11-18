import { AfterViewInit, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { BrandService } from '../../services/brand.service';
import { Brand, BrandStatus } from '../../models/brand.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { debounceTime, Subject } from 'rxjs';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-brand-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './brand-list.component.html',
  styleUrl: './brand-list.component.css',
})

export class BrandListComponent implements AfterViewInit, OnInit {
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  @ViewChild('customerTable') customerTable!: ElementRef<HTMLTableElement>;
  brands: Brand[] = [];
  searchTerm: string = '';
  isLoading: boolean = true;
  searchTerm$ = new Subject<string>();
  selectedBrand: Brand | null = null;
  isModalOpen: boolean = false;
  isEditModalOpen: boolean = false;
  editBrandData: Brand = {
    brandId: '',
    brandName: '',
    description: '',
    image: '',
    status: BrandStatus.Pending
  };
  selectedFile: File | null = null;

  BrandStatus = BrandStatus;

  constructor(private renderer: Renderer2, private brandService: BrandService) { }

  ngOnInit(): void {
    this.loadBrands();

    this.searchTerm$.pipe(debounceTime(300)).subscribe((term) => {
      this.searchTerm = term;
    });
  }

  onSearchInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm$.next(target.value);
  }

  loadBrands(): void {
    this.isLoading = true;
    this.brandService.getAllBrands().subscribe(
      (data) => {
        this.brands = data;
        this.isLoading = false;
      },
      (error) => {
        console.error('Error loading brands:', error);
        this.isLoading = false;
      }
    );
  }

  filterBrands(): Brand[] {
    if (!this.searchTerm.trim()) {
      return this.brands;
    }
    return this.brands.filter((brand) =>
      brand.brandName.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  viewBrand(brand: Brand): void {
    this.selectedBrand = brand;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedBrand = null;
  }

  isAddModalOpen: boolean = false;
  newBrandData: Brand = {
    brandId: '',
    brandName: '',
    description: '',
    image: '',
    status: BrandStatus.Pending,
  };

  openAddModal(): void {
    this.isAddModalOpen = true;
    this.newBrandData = {
      brandId: '',
      brandName: '',
      description: '',
      image: '',
      status: BrandStatus.Pending,
    };
  }

  closeAddModal(): void {
    this.isAddModalOpen = false;
  }

  addBrand(event: Event): void {
    event.preventDefault();
    const formData = new FormData();
    formData.append('brandName', this.newBrandData.brandName);
    formData.append('description', this.newBrandData.description || '');
    formData.append('status', this.newBrandData.status);

    if (this.selectedFile) {
      formData.append('image', this.selectedFile, this.selectedFile.name);
    }

    this.brandService.createBrand(formData).subscribe(
      () => {
        this.loadBrands();
        this.closeAddModal();
      },
      (error) => {
        console.error('Error adding brand:', error);
      }
    );
  }

  closeEditModal(): void {
    this.isEditModalOpen = false;
  }

  editBrand(brand: Brand): void {
    this.isEditModalOpen = true;
    this.editBrandData = {
      ...brand,
      status: brand.status || BrandStatus.Pending
    };
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.selectedFile = input.files[0];
    }
  }

  updateBrand(event: Event): void {
    event.preventDefault();
    const formData = new FormData();
    formData.append('brandName', this.editBrandData.brandName);
    formData.append('description', this.editBrandData.description || '');
    formData.append('status', this.editBrandData.status);

    if (this.selectedFile) {
      formData.append('image', this.selectedFile, this.selectedFile.name);
    }

    this.brandService.updateBrand(this.editBrandData.brandId, formData).subscribe(
      () => {
        this.loadBrands();
        this.closeEditModal();
      },
      (error) => {
        console.error('Error updating brand:', error);
      }
    );
  }

  isDeleteModalOpen: boolean = false;
  isFinalConfirmation: boolean = false;
  brandToDelete: Brand | null = null;
  openDeleteModal(brand: Brand): void {
    this.isDeleteModalOpen = true;
    this.isFinalConfirmation = false;
    this.brandToDelete = brand;
  }
  closeDeleteModal(): void {
    this.isDeleteModalOpen = false;
    this.isFinalConfirmation = false;
    this.brandToDelete = null;
  }
  proceedToFinalConfirmation(): void {
    this.isFinalConfirmation = true;
  }
  confirmDelete(): void {
    if (this.brandToDelete) {
      this.brandService.deleteBrand(this.brandToDelete.brandId).subscribe(
        () => {
          this.loadBrands();
          this.closeDeleteModal();
          console.log('Brand deleted successfully');
        },
        (error) => {
          console.error('Error deleting brand:', error);
          this.closeDeleteModal();
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
}
