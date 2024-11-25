import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product } from '../models/product.model';
import { Observable } from 'rxjs';
import { Brand } from '../models/brand.model';
import { Category } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = 'https://localhost:7192/api/products';
  private brandApiUrl = 'https://localhost:7192/api/Brands';
  private categoryApiUrl = 'https://localhost:7192/api/Categories';

  constructor(private http: HttpClient) { }

  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/get-all`);
  }

  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/get-by-id/${id}`);
  }

  createProduct(product: FormData): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/create`, product);
  }

  updateProduct(id: string, product: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/update/${id}`, product);
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete/${id}`);
  }

  getAllBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${this.brandApiUrl}/get-all-brands`);
  }

  getBrandById(brandId: string): Observable<Brand> {
    return this.http.get<Brand>(`${this.apiUrl}/get-brand-by-id/${brandId}`);
  }

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.categoryApiUrl}/get-all-categories`);
  }

  searchProducts(categoryNames?: string[], brandNames?: string[]): Observable<Product[]> {
    let params = new HttpParams();

    if (categoryNames && categoryNames.length > 0) {
      categoryNames.forEach(category => {
        params = params.append('categoryNames', category);
      });
    }

    if (brandNames && brandNames.length > 0) {
      brandNames.forEach(brand => {
        params = params.append('brandNames', brand);
      });
    }

    return this.http.get<Product[]>(`${this.apiUrl}/search`, { params });
  }
}
