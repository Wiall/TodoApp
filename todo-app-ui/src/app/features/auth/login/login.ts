import { Component, ChangeDetectorRef } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  ActivatedRoute,
  Router,
  RouterLink
} from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {

  isLoading = false;
  errorMessages: string[] = [];

  readonly loginForm;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {
    this.loginForm = this.formBuilder.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  submit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessages = [];

    this.authService
      .login(this.loginForm.getRawValue())
      .subscribe({
        next: () => {

          const returnUrl =
            this.activatedRoute
              .snapshot
              .queryParamMap
              .get('returnUrl');

          this.router.navigateByUrl(
            returnUrl || '/'
          );
        },

        error: error => {

          this.errorMessages =
            this.authService.getErrorMessages(
              error,
              'Invalid email or password.'
            );

          this.isLoading = false;

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }
}
