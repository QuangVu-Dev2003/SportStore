import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginRequest } from '../models/login-request.model';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  // model: LoginRequest = {
  //   email: '',
  //   password: '',
  // };
  model = { email: '', password: '' };
  rememberMe: boolean = false;
  errorMessage: string | null = null;
  forgotPasswordEmail: string = '';
  forgotPasswordError: string | null = null;

  constructor(
    private authService: AuthService,
    private cookieService: CookieService,
    private router: Router
  ) { }

  ngOnInit(): void {
    const savedEmail = localStorage.getItem('rememberedEmail') || sessionStorage.getItem('tempEmail');
    const savedPassword = localStorage.getItem('rememberedPassword') || sessionStorage.getItem('tempPassword');
    if (savedEmail) this.model.email = savedEmail;
    if (savedPassword) this.model.password = savedPassword;
  }

  onFormSubmit(): void {
    this.authService.login(this.model).subscribe({
      next: (response) => {
        console.log('Login response:', response);

        // Lưu token vào CookieService
        this.cookieService.set('Authentication', response.token, undefined, '/');
        console.log('Token saved to cookies.');

        // Điều hướng về trang chính
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        console.error('Đăng nhập thất bại:', err);
        this.errorMessage = 'Email hoặc mật khẩu không đúng. Vui lòng thử lại.';
      },
    });
  }

  onForgotPasswordSubmit(): void {
    this.authService
      .forgotPassword(this.forgotPasswordEmail, { responseType: 'text' })
      .subscribe({
        next: (response: any) => {
          alert(response); // Hiển thị thông báo từ API
          this.forgotPasswordError = null;
        },
        error: (err) => {
          console.error('Yêu cầu quên mật khẩu thất bại:', err);
          this.forgotPasswordError = 'Không thể gửi yêu cầu, vui lòng thử lại.';
        },
      });
  }
}
