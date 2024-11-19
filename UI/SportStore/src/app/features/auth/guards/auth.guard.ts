import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { AuthService } from '../services/auth.service';
import { jwtDecode } from 'jwt-decode';

export const authGuard: CanActivateFn = (route, state) => {
  const cookieService = inject(CookieService);
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = cookieService.get('Authentication');
  if (!token) {
    console.log("No token found");
    authService.logout();
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
  }

  try {
    const decodedToken: any = jwtDecode(token.replace('Bearer ', ''));

    console.log('Token decoded:', decodedToken);

    const rolesFromToken = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    const userRoles: string[] = Array.isArray(rolesFromToken) ? rolesFromToken : [rolesFromToken];

    console.log('Roles in token:', rolesFromToken);
    console.log('User roles:', userRoles);

    const expirationDate = decodedToken.exp * 1000;
    if (expirationDate < Date.now()) {
      console.log("Token expired");
      authService.logout();
      return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
    }

    const requiredRoles: string[] = route.data?.['roles'] || [];
    console.log('Required roles:', requiredRoles);

    if (requiredRoles.length && !requiredRoles.some(role => userRoles.includes(role))) {
      console.log("Role mismatch");
      return router.createUrlTree(['/forbidden']);
    }

    return true;
  } catch (error) {
    console.error('Error decoding token:', error);
    authService.logout();
    return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
  }
};
