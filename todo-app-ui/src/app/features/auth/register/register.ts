import { Component, ChangeDetectorRef } from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Router,
  RouterLink
} from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {

  isLoading = false;
  errorMessages: string[] = [];

  readonly registerForm;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {
    this.registerForm = this.formBuilder.nonNullable.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.pattern(/\p{Lu}/u),
          Validators.pattern(/\p{Ll}/u),
          Validators.pattern(/[^\p{L}\p{N}]/u)
        ]
      ]
    });
  }

  submit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();

      return;
    }

    this.isLoading = true;
    this.errorMessages = [];

    this.authService
      .register(
        this.registerForm.getRawValue()
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/']);
        },

        error: error => {

          this.errorMessages =
            this.authService.getErrorMessages(
              error,
              'Failed to create account.'
            );

          this.isLoading = false;

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }

  get passwordValue(): string {
    return this.registerForm.controls.password.value;
  }

  get hasMinLength(): boolean {
    return this.passwordValue.length >= 8;
  }

  get hasUppercase(): boolean {
    return /\p{Lu}/u.test(
      this.passwordValue
    );
  }

  get hasLowercase(): boolean {
    return /\p{Ll}/u.test(
      this.passwordValue
    );
  }

  get hasSpecialCharacter(): boolean {
    return /[^\p{L}\p{N}]/u.test(
      this.passwordValue
    );
  }
}
