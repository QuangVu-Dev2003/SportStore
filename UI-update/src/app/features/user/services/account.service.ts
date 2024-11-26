import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private apiUrl = 'https://localhost:7192/api/User';

  constructor(private http: HttpClient) { }

  getProfile(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/profile`);
  }
  updateProfile(user: any): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/update`, user);
  }

  markForDeletion(): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/mark-for-deletion`, {});
  }

  restoreAccount(): Observable<string> {
    return this.http.post<string>(`${this.apiUrl}/restore-account`, {});
  }

  changePassword(payload: { oldPassword: string; newPassword: string; confirmPassword: string }) {
    return this.http.post<{ message: string }>(`${this.apiUrl}/change-password`, payload);
  }
}
