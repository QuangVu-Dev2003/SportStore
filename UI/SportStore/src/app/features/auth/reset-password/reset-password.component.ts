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

  constructor(private route: ActivatedRoute, private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    // Lấy token và email từ URL
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'];
      this.email = params['email'];
    });
  }

  onSubmit(): void {
    if (this.newPassword !== this.confirmPassword) {
      this.errorMessage = 'Mật khẩu xác nhận không khớp.';
      return;
    }

    const payload = {
      email: this.email,
      token: this.token,
      newPassword: this.newPassword,
      confirmPassword: this.confirmPassword,
    };
    console.log('Payload:', payload);


    this.authService.resetPassword(payload).subscribe({
      next: (response) => {
        alert('Mật khẩu đã được đặt lại thành công!');
        this.router.navigate(['/login']);
      }
    });
  }
}