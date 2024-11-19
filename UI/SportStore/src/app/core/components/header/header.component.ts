import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AppUser } from '../../../features/auth/models/app-user.model';
import { AuthService } from '../../../features/auth/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  user: AppUser | null = null; // Khởi tạo user

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    // Lắng nghe user từ AuthService
    this.authService.user().subscribe({
      next: (response) => {
        this.user = response;
      },
      error: (err) => {
        console.error('Lỗi khi lấy thông tin người dùng:', err);
      },
    });
  }

  onLogout(): void {
    this.authService.logout(); // Gọi hàm logout
    this.router.navigate(['/']); // Chuyển hướng về trang chủ
  }
}
