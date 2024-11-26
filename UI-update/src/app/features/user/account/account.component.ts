import { Component } from '@angular/core';
import { AccountService } from '../services/account.service';
import { CommonModule } from '@angular/common';
import { User } from '../models/user.model';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
declare var bootstrap: any;
@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.css'
})
export class AccountComponent {
  user: User | null = null;
  message: string = '';
  changePasswordForm: FormGroup;
  isSuccess: boolean = false;

  showOldPassword: boolean = false;
  showNewPassword: boolean = false;
  showConfirmPassword: boolean = false;

  constructor(private accountService: AccountService, private router: Router, private fb: FormBuilder) {
    this.changePasswordForm = this.fb.group(
      {
        oldPassword: ['', Validators.required],
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validators: this.passwordsMatchValidator }
    );
  }

  ngOnInit(): void {
    this.getProfile();
  }

  getProfile(): void {
    this.accountService.getProfile().subscribe({
      next: (user) => {
        this.user = user;
        this.message = '';
      },
      error: (err) => {
        this.message = 'Không thể tải thông tin tài khoản.';
        console.error(err);
      }
    });
  }

  passwordsMatchValidator(form: FormGroup): { [key: string]: boolean } | null {
    const newPassword = form.get('newPassword')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return newPassword && confirmPassword && newPassword !== confirmPassword
      ? { mismatch: true }
      : null;
  }

  onChangePassword() {
    if (this.changePasswordForm.valid) {
      const payload = {
        oldPassword: this.changePasswordForm.get('oldPassword')?.value,
        newPassword: this.changePasswordForm.get('newPassword')?.value,
        confirmPassword: this.changePasswordForm.get('confirmPassword')?.value,
      };
      console.log('Payload:', payload);

      this.accountService.changePassword(payload).subscribe({
        next: (response: any) => {
          console.log('Response from API:', response);
          this.isSuccess = true;
          this.message = response.message || 'Mật khẩu đã được thay đổi thành công!';

          const modalElement = document.getElementById('changePasswordModal');
          if (modalElement) {
            modalElement.classList.remove('show');
            modalElement.setAttribute('aria-hidden', 'true');
            modalElement.style.display = 'none';

            const backdrop = document.querySelector('.modal-backdrop');
            if (backdrop) {
              backdrop.remove();
            }

            document.body.classList.remove('modal-open');
            document.body.style.overflow = '';
            document.body.style.paddingRight = '';
          }
          this.clearMessageAfterDelay();
        },
        error: (error) => {
          console.error('Error response:', error);
          this.isSuccess = false;
          this.message =
            error.error?.message || 'Không thể thay đổi mật khẩu. Vui lòng thử lại.';
          this.clearMessageAfterDelay();
        },
      });
    } else {
      console.error('Form is invalid');
    }
  }

  private clearMessageAfterDelay() {
    setTimeout(() => {
      this.message = '';
    }, 5000);
  }

  markForDeletion(): void {
    this.accountService.markForDeletion().subscribe({
      next: (response) => {
        this.message = response;
      },
      error: (err) => {
        this.message = 'Không thể đánh dấu tài khoản.';
        console.error(err);
      }
    });
  }

  restoreAccount(): void {
    this.accountService.restoreAccount().subscribe({
      next: (response) => {
        this.message = response;
      },
      error: (err) => {
        this.message = 'Không thể khôi phục tài khoản.';
        console.error(err);
      }
    });
  }
  getInitials(firstName?: string, lastName?: string): string {
    const firstInitial = firstName ? firstName.charAt(0).toUpperCase() : '';
    const lastInitial = lastName ? lastName.charAt(0).toUpperCase() : '';
    return `${firstInitial}${lastInitial}`;
  }

  UpdateProfile(): void {
    this.router.navigate(['/update-account']);
  }

  togglePasswordVisibility(field: string): void {
    if (field === 'oldPassword') {
      this.showOldPassword = !this.showOldPassword;
    } else if (field === 'newPassword') {
      this.showNewPassword = !this.showNewPassword;
    } else if (field === 'confirmPassword') {
      this.showConfirmPassword = !this.showConfirmPassword;
    }
  }
}
