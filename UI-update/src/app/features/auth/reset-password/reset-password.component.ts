import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
  email: string = '';
  token: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  errorMessage: string | null = null;
  successMessage: string | null = null;
  showNewPassword: boolean = false;
  showConfirmPassword: boolean = false;

  constructor(private route: ActivatedRoute, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'];
      this.email = params['email'];
    });
  }

  toggleNewPassword(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onSubmit(): void {
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Mật khẩu xác nhận không khớp.';
      this.successMessage = null;
      return;
    }

    const payload = {
      email: this.email,
      token: this.token,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword,
    };

    this.authService.resetPassword(payload).subscribe({
      next: (response: any) => {
        this.successMessage = response.message || 'Mật khẩu đã được đặt lại thành công!';
        this.errorMessage = null;
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error.error.message || 'Đã xảy ra lỗi. Vui lòng thử lại.';
        if (error.error && error.error.errors) {
          this.errorMessage = error.error.errors.join(', ');
        } else if (error.error && error.error.message) {
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'Đã xảy ra lỗi không xác định.';
        }
        this.successMessage = null;
      },
    });
  }
}