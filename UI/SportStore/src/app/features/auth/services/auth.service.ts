import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode'; // Import đúng từ jwt-decode
import { LoginRequest } from '../models/login-request.model';
import { LoginResponse } from '../models/login-response.model';
import { AppUser } from '../models/app-user.model';
import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';


@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private url = 'https://localhost:7192/api/Authentication';
  private $user = new BehaviorSubject<AppUser | null>(null);
  private isBrowser: boolean;

  constructor(private http: HttpClient, private cookieService: CookieService, @Inject(PLATFORM_ID) private platformId: object) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    this.loadUserFromStorage();
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.url}/login-user`, request).pipe(
      tap((response) => {
        this.cookieService.set('Authentication', `Bearer ${response.token}`, undefined, '/', undefined, true, 'Strict');
        this.setUserFromToken(response.token);
        this.saveUserToStorage();
      })
    );
  }

  private setUserFromToken(token: string): void {
    try {
      const decodedToken: any = jwtDecode(token);

      const user: AppUser = {
        id: decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: decodedToken.email,
        userName: decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
        refreshTokens: [],
        failedLoginAttempts: 0,
        lockoutEnd: undefined,
      };

      this.$user.next(user);
    } catch (error) {
      console.error('Lỗi giải mã JWT:', error);
      this.$user.next(null);
    }
  }

  user(): Observable<AppUser | null> {
    return this.$user.asObservable(); // Observable để lắng nghe user
  }

  isAuthenticated(): boolean {
    const token = this.cookieService.get('Authentication');
    if (!token) return false;

    try {
      const decodedToken: any = jwtDecode(token);
      const expirationDate = decodedToken.exp * 1000;
      return expirationDate > Date.now();
    } catch {
      return false;
    }
  }

  logout(): void {
    localStorage.removeItem('user'); // Xóa user khỏi localStorage
    this.cookieService.delete('Authentication', '/');
    this.$user.next(null);
  }

  private saveUserToStorage(): void {
    if (this.isBrowser) {
      const currentUser = this.$user.getValue();
      if (currentUser) {
        localStorage.setItem('user', JSON.stringify(currentUser));
      }
    }
  }

  private loadUserFromStorage(): void {
    if (this.isBrowser) {
      const userData = localStorage.getItem('user');
      if (userData) {
        this.$user.next(JSON.parse(userData) as AppUser);
      }
    }
  }

  forgotPassword(email: string, options?: any): Observable<any> {
    return this.http.post(`${this.url}/forgot-password`, { email }, options);
  }

  resetPassword(payload: any): Observable<any> {
    return this.http.post(`${this.url}/reset-password`, payload);
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.url}/register-user`, data).pipe(
      catchError((error) => {
        // Kiểm tra phản hồi lỗi từ backend
        const errorMessage = error.error?.message || 'Đăng ký không thành công.';
        const detailedErrors = error.error?.errors?.join(' ') || ''; // Gộp lỗi chi tiết
        return throwError(() => new Error(`${errorMessage} ${detailedErrors}`));
      })
    );
  }
  // In auth.service.ts
  getAccountStatus(): Observable<any> {
    return this.http.get<any>('https://localhost:7192/api/User/account-status').pipe(
      catchError((error) => {
        console.error('Error fetching account status:', error);
        return throwError(() => new Error('Failed to fetch account status.'));
      })
    );
  }
  // In auth.service.ts
  restoreAccount(email: string): Observable<any> {
    return this.http.post(`https://localhost:7192/api/User/restore-account`, { email }).pipe(
      catchError((error) => {
        console.error('Error restoring account:', error);
        return throwError(() => new Error('Failed to restore account.'));
      })
    );
  }

}
