import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { tap } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const cookieService = inject(CookieService);
  const authToken = cookieService.get('Authentication');

  console.log('AuthInterceptor - Token:', authToken);
  console.log('AuthInterceptor - Request URL:', req.url);

  if (authToken) {
    const authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${authToken}`,
      },
    });

    console.log('AuthInterceptor - Modified Request:', authReq);

    return next(authReq).pipe(
      tap({
        next: (event) => {
          console.log('AuthInterceptor - Response Success:', event);
        },
        error: (err) => {
          console.error('AuthInterceptor - Response Error:', err);
        },
      })
    );
  }

  console.log('AuthInterceptor - No token found, sending original request.');
  return next(req).pipe(
    tap({
      next: (event) => {
        console.log('AuthInterceptor - Response Success (No Token):', event);
      },
      error: (err) => {
        console.error('AuthInterceptor - Response Error (No Token):', err);
      },
    })
  );
};
