import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, OnInit, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LoginRequest } from '../models/login-request.model';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit, OnDestroy {
  model = { email: '', password: '' };
  rememberMe: boolean = false;
  errorMessage: string | null = null;
  forgotPasswordEmail: string = '';
  forgotPasswordError: string | null = null;
  restoreAccountError: string | null = null;
  isAccountDeletable: boolean = false;
  failedAttempts: string | number | null = null;
  lockoutMessage: string | null = null;
  remainingLockoutTime: number | null = null;
  countdownInterval: any;
  timeLeft: string = '0:00';
  isEmailUnconfirmed: boolean = false;
  showPassword: boolean = false;

  constructor(
    private authService: AuthService,
    private cookieService: CookieService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) { }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const rememberedEmail = localStorage.getItem('rememberedEmail');
      const rememberedPassword = localStorage.getItem('rememberedPassword');

      if (rememberedEmail && rememberedPassword) {
        this.model.email = rememberedEmail;
        this.model.password = rememberedPassword;
        this.rememberMe = true;
      }
    }
  }

  ngOnDestroy(): void {
    if (this.countdownInterval) {
      clearInterval(this.countdownInterval);
    }
  }

  onFormSubmit(): void {
    this.authService.login(this.model).subscribe({
      next: (response) => {
        console.log('Login response:', response);

        this.cookieService.set('Authentication', response.token, undefined, '/');

        if (this.rememberMe) {
          localStorage.setItem('rememberedEmail', this.model.email);
          localStorage.setItem('rememberedPassword', this.model.password);
        } else {
          localStorage.removeItem('rememberedEmail');
          localStorage.removeItem('rememberedPassword');
        }

        this.router.navigateByUrl('/');

        this.checkAccountStatus();
      },
      error: (error) => {
        this.isEmailUnconfirmed = false;
        this.failedAttempts = null;
        if (error.error && error.error.message) {
          if (error.error.message.includes('Email not confirmed')) {
            this.errorMessage = 'Email của bạn chưa được xác nhận. Vui lòng xác nhận email trước khi đăng nhập.';
            this.isEmailUnconfirmed = true;
          }
          else if (error.error.message.includes('Bạn đã nhập sai mật khẩu')) {
            this.errorMessage = error.error.message;
            this.failedAttempts = error.error.failedAttempts?.toString() || '0';
          }
          else if (error.error.message.includes('Tài khoản của bạn đã bị khóa')) {
            this.errorMessage = error.error.message;
            this.failedAttempts = '5';

            if (typeof error.error.remainingLockoutTime === 'number') {
              this.remainingLockoutTime = error.error.remainingLockoutTime;
              if (this.remainingLockoutTime !== null) {
                this.startCountdown(this.remainingLockoutTime);
              }
            } else {
              console.warn('Thời gian khóa còn lại không hợp lệ.');
            }
          }
          else {
            this.errorMessage = error.error.message;
          }
        } else {
          this.errorMessage = 'Đăng nhập thất bại. Vui lòng thử lại.';
        }
      },
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onRememberMeChange(): void {
    if (!this.rememberMe) {
      localStorage.removeItem('rememberedEmail');
      localStorage.removeItem('rememberedPassword');
    }
  }

  startCountdown(remainingTime: number): void {
    if (remainingTime <= 0) {
      this.timeLeft = '0:00';
    }
    this.countdownInterval = setInterval(() => {
      if (remainingTime > 0) {
        remainingTime--;
        this.timeLeft = this.formatTime(remainingTime);
      } else {
        clearInterval(this.countdownInterval);
        this.timeLeft = '0:00';
      }
    }, 1000);
  }


  formatTime(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}:${remainingSeconds < 10 ? '0' + remainingSeconds : remainingSeconds}`;
  }

  checkAccountStatus(): void {
    this.authService.getAccountStatus().subscribe({
      next: (status) => {
        if (status.isDeleted) {
          const deletionDate = new Date(status.deletionDate);
          const currentDate = new Date();
          const diffTime = currentDate.getTime() - deletionDate.getTime();
          const diffDays = diffTime / (1000 * 3600 * 24);

          if (diffDays <= 30) {
            this.isAccountDeletable = true;
          } else {
            this.errorMessage = 'Tài khoản của bạn đã bị xóa hơn 30 ngày. Vui lòng đăng ký lại.';
          }
        } else {
          this.router.navigateByUrl('/');
        }
      },
      error: (err) => {
        console.error('Lỗi khi kiểm tra trạng thái tài khoản:', err);
        this.errorMessage = 'Không thể kiểm tra trạng thái tài khoản. Vui lòng thử lại.';
      }
    });
  }

  restoreAccount(): void {
    this.authService.restoreAccount(this.model.email).subscribe({
      next: (response) => {
        alert('Tài khoản của bạn đã được khôi phục.');
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        console.error('Khôi phục tài khoản thất bại:', err);
        this.restoreAccountError = 'Không thể khôi phục tài khoản. Vui lòng thử lại.';
      },
    });
  }

  onForgotPasswordSubmit(): void {
    this.authService
      .forgotPassword(this.forgotPasswordEmail, { responseType: 'text' })
      .subscribe({
        next: (response: any) => {
          alert(response);
          this.forgotPasswordError = null;
        },
        error: (err) => {
          console.error('Yêu cầu quên mật khẩu thất bại:', err);
          this.forgotPasswordError = 'Không thể gửi yêu cầu, vui lòng thử lại.';
        },
      });
  }
}