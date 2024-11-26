import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppUser } from '../models/user.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:7192/api/AdminUser';
  constructor(private http: HttpClient) { }

  getAllUsers(): Observable<AppUser[]> {
    return this.http.get<AppUser[]>(`${this.apiUrl}/get-all-users`);
  }

  getUserRoles(userId: string): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/get-user-roles/${userId}`);
  }

  assignRoleToUser(userId: string, roleName: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign-role`, { userId, roleName });
  }

  reAuthenticate(password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/re-authenticate`, { password });
  }
}
