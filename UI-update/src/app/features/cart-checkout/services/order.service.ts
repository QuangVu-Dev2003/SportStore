import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = 'https://localhost:7192/api/UserOrders';

  constructor(private http: HttpClient) { }

  checkout(order: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/checkout`, order);
  }

  getOrderDetails(orderId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${orderId}`);
  }

  cancelOrder(orderId: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/${orderId}/cancel`, {});
  }
}
