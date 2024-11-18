import { Routes } from '@angular/router';
import { HomeComponent } from './core/components/home/home.component';
import { ShopComponent } from './core/components/shop/shop.component';
import { DetailProductComponent } from './core/components/detail-product/detail-product.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { NotFoundComponent } from './core/components/not-found/not-found.component';
import { BrandListComponent } from './features/admin/management-brand/brand-list/brand-list.component';
import { AccountComponent } from './features/user/account/account.component';
import { ManagementCategoryComponent } from './features/admin/management-category/management-category.component';
import { ManagementProductComponent } from './features/admin/management-product/management-product.component';
import { AddProductComponent } from './features/admin/management-product/add-product/add-product.component';
import { CanDeactivateGuard } from './features/admin/guards/can-deactivate.guard';
import { EditProductComponent } from './features/admin/management-product/edit-product/edit-product.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'shop', component: ShopComponent },
    { path: 'shop/detail-product', component: DetailProductComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'admin/management-brand', component: BrandListComponent },
    { path: 'admin/management-category', component: ManagementCategoryComponent },
    { path: 'admin/management-product', component: ManagementProductComponent },
    { path: 'admin/add-product', component: AddProductComponent, canDeactivate: [CanDeactivateGuard] },
    { path: 'admin/edit-product/:id', component: EditProductComponent, canDeactivate: [CanDeactivateGuard] },
    { path: 'account', component: AccountComponent },
    { path: '**', component: NotFoundComponent }
];
