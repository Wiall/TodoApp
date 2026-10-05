import {
  HttpErrorResponse,
  HttpInterceptorFn
} from '@angular/common/http';

import {
  inject
} from '@angular/core';

import {
  catchError,
  switchMap,
  throwError
} from 'rxjs';

import { Router } from '@angular/router';

import {
  AuthService
} from '../services/auth.service';

import {
  API_BASE_URL
} from '../config/api.config';

export const authInterceptor:
  HttpInterceptorFn = (req, next) => {

  const authService =
    inject(AuthService);

  const router =
    inject(Router);

  const accessToken =
    authService.getAccessToken();

  const isApiRequest =
    req.url.startsWith(
      API_BASE_URL
    );

  const isAuthRequest =
    req.url.includes(
      '/api/auth/'
    );

  /*
   * Login, register, refresh and logout
   * must not go through bearer/refresh logic.
   */
  if (
    !isApiRequest ||
    isAuthRequest
  ) {
    return next(req);
  }

  const authenticatedRequest =
    accessToken
      ? req.clone({
        setHeaders: {
          Authorization:
            `Bearer ${accessToken}`
        }
      })
      : req;

  return next(
    authenticatedRequest
  ).pipe(
    catchError(
      (error: unknown) => {

        /*
         * Refresh only when the API
         * explicitly says that the token
         * is no longer accepted.
         */
        if (
          !(
            error instanceof
            HttpErrorResponse
          ) ||
          error.status !== 401
        ) {
          return throwError(
            () => error
          );
        }

        return authService
          .refresh()
          .pipe(

            switchMap(
              response => {

                const retryRequest =
                  req.clone({
                    setHeaders: {
                      Authorization:
                        `Bearer ${response.accessToken}`
                    }
                  });

                return next(
                  retryRequest
                );
              }
            ),

            catchError(
              refreshError => {

                authService.clearSession();

                router.navigate(
                  ['/login']
                );

                return throwError(
                  () => refreshError
                );
              }
            )
          );
      }
    )
  );
};
