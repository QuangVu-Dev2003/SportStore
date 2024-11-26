import { Component, OnInit } from '@angular/core';
import { User } from '../models/user.model';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-account-update',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './account-update.component.html',
  styleUrl: './account-update.component.css'
})
export class AccountUpdateComponent implements OnInit {
  user: User = new User();
  message: string = '';

  constructor(
    private accountService: AccountService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.accountService.getProfile().subscribe({
      next: (userData) => {
        this.user = userData;
        if (this.user.firstName === 'string') this.user.firstName = '';
        if (this.user.lastName === 'string') this.user.lastName = '';
      },
      error: (error) => {
        this.message = 'Lỗi khi tải dữ liệu người dùng';
      }
    });
  }

  onSubmit(): void {
    if (this.user.firstName?.trim().length < 2 ||
      this.user.lastName?.trim().length < 2 ||
      this.user.address?.trim().length < 2 ||
      !/^\d{10}$/.test(this.user.phoneNumber || '')) {
      this.message = 'Vui lòng sửa lỗi trong biểu mẫu.';
      return;
    }

    const updatedUser: any = {
      id: this.user.id,
      firstName: this.user.firstName?.trim(),
      lastName: this.user.lastName?.trim(),
      address: this.user.address?.trim(),
      phoneNumber: this.user.phoneNumber?.trim()
    };

    this.accountService.updateProfile(updatedUser).subscribe({
      next: (response) => {
        this.message = response.message;
        this.router.navigate(['/account']);
      },
      error: (error) => {
        this.message = 'Lỗi khi cập nhật hồ sơ.';
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/account']);
  }
}