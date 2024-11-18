import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Brand } from '../models/brand.model';

@Injectable({
  providedIn: 'root'
})
export class BrandService {
  private apiUrl = 'https://localhost:7192/api/Brands';

  constructor(private http: HttpClient) { }

  getAllBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${this.apiUrl}/get-all-brands`);
  }

  getBrandById(id: string): Observable<Brand> {
    return this.http.get<Brand>(`${this.apiUrl}/get-brand-by-id/${id}`);
  }

  createBrand(brand: FormData): Observable<Brand> {
    return this.http.post<Brand>(`${this.apiUrl}/create-brand`, brand);
  }

  updateBrand(id: string, brand: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/update-brand/${id}`, brand);
  }

  deleteBrand(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete-brand/${id}`);
  }
}
