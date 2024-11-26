import { Component, OnInit } from '@angular/core';
import { BrandService } from '../../../features/admin/services/brand.service';
import { CategoryService } from '../../../features/admin/services/category.service';
import { Brand } from '../../../features/admin/models/brand.model';
import { Category } from '../../../features/admin/models/category.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent implements OnInit {
  brands: Brand[] = [];
  categories: Category[] = [];

  constructor(private brandService: BrandService, private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadBrands();
    this.loadCategories();
  }

  loadBrands(): void {
    this.brandService.getAllBrands().subscribe(
      (data) => {
        this.brands = data;
      }
    );
  }

  loadCategories(): void {
    this.categoryService.getAllCategories().subscribe(
      (data) => {
        this.categories = data;
      }
    );
  }

}
