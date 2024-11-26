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
import { authGuard } from './features/auth/guards/auth.guard';
import { ForbiddenComponent } from './core/components/forbidden/forbidden.component';
import { ResetPasswordComponent } from './features/auth/reset-password/reset-password.component';
import { ConfirmEmailComponent } from './features/auth/confirm-email/confirm-email.component';
import { AccountUpdateComponent } from './features/user/account-update/account-update.component';
import { UserListComponent } from './features/admin/management-user/user-list/user-list/user-list.component';
import { ShoppingCartComponent } from './features/cart-checkout/shopping-cart/shopping-cart.component';
import { OrderDetailsComponent } from './features/cart-checkout/order-details/order-details.component';

export const routes: Routes = [
    { path: '', component: ShopComponent },
    { path: 'shop', component: ShopComponent },
    { path: 'cart', component: ShoppingCartComponent, canActivate: [authGuard] },
    { path: 'shop/detail-product/:id', component: DetailProductComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'confirm-email', component: ConfirmEmailComponent },
    { path: 'login', component: LoginComponent },
    {
        path: 'admin/management-brand', component: BrandListComponent, canActivate: [authGuard],
        data: { roles: ['Admin'] }
    },
    {
        path: 'admin/management-category', component: ManagementCategoryComponent, canActivate: [authGuard],
        data: { roles: ['Admin'] }
    },
    {
        path: 'admin/management-product', component: ManagementProductComponent, canActivate: [authGuard],
        data: { roles: ['Admin'] }
    },
    {
        path: 'admin/add-product', component: AddProductComponent, canDeactivate: [CanDeactivateGuard], canActivate: [authGuard],
        data: { roles: ['Admin'] }
    },
    {
        path: 'admin/edit-product/:id', component: EditProductComponent, canDeactivate: [CanDeactivateGuard], canActivate: [authGuard],
        data: { roles: ['Admin'] }
    },
    {
        path: 'admin/management-user', component: UserListComponent, canActivate: [authGuard],
        data: { roles: ['Admin'] }
    },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: 'account', component: AccountComponent, canActivate: [authGuard] },
    { path: 'update-account', component: AccountUpdateComponent, canActivate: [authGuard] },
    { path: 'forbidden', component: ForbiddenComponent },
    { path: 'orders/:orderId', component: OrderDetailsComponent },
    { path: '**', component: NotFoundComponent }
];
