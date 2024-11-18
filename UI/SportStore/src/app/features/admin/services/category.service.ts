import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Category } from '../models/category.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = 'https://localhost:7192/api/Categories';

  constructor(private http: HttpClient) { }

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/get-all-categories`);
  }

  getCategoryById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/get-category-by-id/${id}`);
  }

  createCategory(category: FormData): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/create-category`, category);
  }

  updateCategory(id: string, category: FormData): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/update-category/${id}`, category);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete-category/${id}`);
  }
}
