import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppUser } from '../../../models/user.model';
import { UserService } from '../../../services/user.service';
import { debounceTime, Subject } from 'rxjs';
import * as XLSX from 'xlsx';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  users: AppUser[] = [];
  searchTerm$ = new Subject<string>();
  searchTerm: string = '';

  currentPage: number = 1;
  itemsPerPage: number = 5;
  totalItems: number = 0;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.userService.getAllUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.totalItems = data.length;
      },
      error: (err) => console.error('Error fetching users:', err)
    });

    this.searchTerm$.pipe(debounceTime(300)).subscribe((term) => {
      this.searchTerm = term;
      this.currentPage = 1;
    });
  }

  onSearchInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm$.next(target.value);
  }

  filterUser(): AppUser[] {
    let filteredUsers = this.users;

    if (this.searchTerm.trim()) {
      filteredUsers = this.users.filter((user) =>
        user.id.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        user.firstName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        user.lastName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        user.email.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }

    this.totalItems = filteredUsers.length;
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    return filteredUsers.slice(startIndex, startIndex + this.itemsPerPage);
  }

  changePage(page: number): void {
    this.currentPage = page;
  }

  get totalPages(): number[] {
    return Array(Math.ceil(this.totalItems / this.itemsPerPage))
      .fill(0)
      .map((_, i) => i + 1);
  }

  exportToExcel(): void {
    const usersForExport = this.users.map((user) => ({
      ID: user.id,
      Name: `${user.firstName} ${user.lastName}`,
      Email: user.email,
      Address: user.address,
      Phone: user.phoneNumber,
    }));

    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(usersForExport);

    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Users');

    XLSX.writeFile(wb, 'users.xlsx');
  }
}
