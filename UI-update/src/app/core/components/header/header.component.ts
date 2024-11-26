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
  user: AppUser | null = null;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
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
    localStorage.removeItem('rememberedEmail');
    localStorage.removeItem('rememberedPassword');
    sessionStorage.removeItem('tempEmail');
    sessionStorage.removeItem('tempPassword');
    this.authService.logout();
    this.router.navigate(['/']).then(() => {
      window.location.reload();
    });
  }
}