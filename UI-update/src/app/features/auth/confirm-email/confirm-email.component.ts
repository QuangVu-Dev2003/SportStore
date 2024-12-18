import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-confirm-email',
    template: ''
})
export class ConfirmEmailComponent implements OnInit {
    message: string = '';

    constructor(
        private route: ActivatedRoute,
        private http: HttpClient,
        private router: Router
    ) { }

    ngOnInit(): void {
        const token = this.route.snapshot.queryParamMap.get('token');
        const email = this.route.snapshot.queryParamMap.get('email');

        if (token && email) {
            this.http
                .get<{ message: string }>(`https://localhost:7192/api/authentication/confirm-email?token=${encodeURIComponent(token)}&email=${encodeURIComponent(email)}`)
                .subscribe(
                    (response) => {
                        if (response.message === 'Xác nhận email thành công!') {
                            this.message = 'Xác nhận email thành công! Bạn sẽ được chuyển hướng đến trang đăng nhập.';

                            setTimeout(() => {
                                this.router.navigate(['/login']); // Chuyển hướng đến trang login
                            }, 2000);
                        } else {
                            this.message = 'Xác nhận email thất bại. Vui lòng thử lại.';
                        }
                    },
                    (error) => {
                        console.error('Lỗi xác nhận email:', error);
                        this.message = 'Xác nhận email thất bại. Vui lòng thử lại.';
                    }
                );
        } else {
            this.message = 'Liên kết xác nhận email không hợp lệ.';
        }
    }
}
