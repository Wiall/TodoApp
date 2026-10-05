import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

import {
  Observable,
  ReplaySubject,
  of,
  throwError
} from 'rxjs';

import {
  catchError,
  finalize,
  map,
  take,
  tap
} from 'rxjs/operators';

import { API_BASE_URL } from '../config/api.config';

import {
  AuthResponse,
  LoginRequest,
  RefreshTokenRequest,
  RegisterRequest
} from '../../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly apiUrl =
    `${API_BASE_URL}/api/auth`;

  private readonly storageKey =
    'todo_auth_session';

  private refreshInProgress = false;

  private refreshResultSubject =
    new ReplaySubject<AuthResponse>(1);

  constructor(
    private readonly http: HttpClient
  ) {}

  login(
    request: LoginRequest
  ): Observable<AuthResponse> {

    return this.http
      .post<AuthResponse>(
        `${this.apiUrl}/login`,
        request
      )
      .pipe(
        tap(response =>
          this.saveSession(response)
        )
      );
  }

  register(
    request: RegisterRequest
  ): Observable<AuthResponse> {

    return this.http
      .post<AuthResponse>(
        `${this.apiUrl}/register`,
        request
      )
      .pipe(
        tap(response =>
          this.saveSession(response)
        )
      );
  }

  refresh(): Observable<AuthResponse> {

    const refreshToken =
      this.getRefreshToken();

    if (!refreshToken) {
      return throwError(
        () => new Error(
          'Refresh token is missing.'
        )
      );
    }

    if (this.refreshInProgress) {
      return this.refreshResultSubject.pipe(
        take(1)
      );
    }

    this.refreshInProgress = true;

    this.refreshResultSubject =
      new ReplaySubject<AuthResponse>(1);

    const request: RefreshTokenRequest = {
      refreshToken
    };

    return this.http
      .post<AuthResponse>(
        `${this.apiUrl}/refresh`,
        request
      )
      .pipe(
        tap(response => {

          this.saveSession(response);

          this.refreshResultSubject.next(
            response
          );

          this.refreshResultSubject.complete();
        }),

        catchError(error => {

          this.refreshResultSubject.error(
            error
          );

          return throwError(
            () => error
          );
        }),

        finalize(() => {
          this.refreshInProgress = false;
        })
      );
  }

  logout(): Observable<void> {

    const refreshToken =
      this.getRefreshToken();

    if (!refreshToken) {
      this.clearSession();

      return of(void 0);
    }

    const request: RefreshTokenRequest = {
      refreshToken
    };

    return this.http
      .post<void>(
        `${this.apiUrl}/logout`,
        request
      )
      .pipe(
        finalize(() =>
          this.clearSession()
        )
      );
  }

  getAccessToken(): string | null {
    return this.getSession()
      ?.accessToken ?? null;
  }

  getRefreshToken(): string | null {
    return this.getSession()
      ?.refreshToken ?? null;
  }

  isAuthenticated(): boolean {
    return this.getAccessToken() !== null;
  }

  clearSession(): void {
    sessionStorage.removeItem(
      this.storageKey
    );
  }

  private saveSession(
    response: AuthResponse
  ): void {
    sessionStorage.setItem(
      this.storageKey,
      JSON.stringify(response)
    );
  }

  private getSession():
    AuthResponse | null {

    const rawSession =
      sessionStorage.getItem(
        this.storageKey
      );

    if (!rawSession) {
      return null;
    }

    try {
      return JSON.parse(
        rawSession
      ) as AuthResponse;
    } catch {
      this.clearSession();

      return null;
    }
  }

  getErrorMessages(
    error: unknown,
    fallback: string
  ): string[] {
    if (
      error instanceof HttpErrorResponse
    ) {
      const body = error.error;

      if (
        body &&
        typeof body.message === 'string'
      ) {
        return [body.message];
      }

      if (
        body &&
        typeof body.detail === 'string'
      ) {
        const messages =
          body.detail
            .split(/\r?\n/)
            .map((line : string) => line.trim())
            .filter((line : string) =>
              line.startsWith('-- ')
            )
            .map((line : string) =>
              line
                .replace(
                  /^--\s*[^:]+:\s*/,
                  ''
                )
                .replace(
                  /\s*Severity:\s*[^ ]+\s*$/,
                  ''
                )
                .trim()
            )
            .filter(Boolean);

        if (messages.length > 0) {
          return messages;
        }

        return [body.detail];
      }

      if (
        typeof body === 'string' &&
        body.trim()
      ) {
        return [body];
      }
    }

    return [fallback];
  }
}
