import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrderService } from '../services/order.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css'
})
export class OrderDetailsComponent implements OnInit {
  order: any = null;

  constructor(private route: ActivatedRoute, private orderService: OrderService) { }

  ngOnInit(): void {
    const orderId = this.route.snapshot.paramMap.get('orderId');
    if (orderId) {
      this.loadOrderDetails(orderId);
    }
  }

  loadOrderDetails(orderId: string): void {
    this.orderService.getOrderDetails(orderId).subscribe({
      next: (data) => {
        this.order = data;
      },
      error: (err) => {
        console.error('Lỗi khi tải chi tiết đơn hàng:', err);
      }
    });
  }

  cancelOrder(): void {
    const confirmed = window.confirm('Bạn có chắc chắn muốn hủy toàn bộ đơn hàng này không?');
    if (confirmed) {
      this.orderService.cancelOrder(this.order.orderId).subscribe({
        next: (response) => {
          alert(response.message);
          this.order.status = response.status; // Cập nhật trạng thái của đơn hàng
        },
        error: (err) => {
          console.error('Lỗi khi hủy đơn hàng:', err);
          alert(err.error?.message || 'Không thể hủy đơn hàng. Vui lòng thử lại.');
        }
      });
    }
  }
}
