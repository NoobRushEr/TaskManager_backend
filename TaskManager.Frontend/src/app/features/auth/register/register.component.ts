import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../core/models/auth.model';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  passwordMatchValidator: Validators = (formGroup: FormGroup) => {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      formGroup.get('confirmPassword')?.setErrors({ mismatch: true });
    } else {
      formGroup.get('confirmPassword')?.setErrors(null);
    }
  };

  registerForm: FormGroup = this.fb.group(
    {
      firstname: ['', [Validators.required, Validators.minLength(3)]],
      lastname: ['', [Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(6)]],
    },
    {
      validators: this.passwordMatchValidator,
    },
  );

  onSubmit(): void {
    if (this.registerForm.valid) {
      const requestdata: RegisterRequest = {
        firstName: this.registerForm.get('firstname')?.value,
        lastName: this.registerForm.get('lastname')?.value,
        email: this.registerForm.get('email')?.value,
        password: this.registerForm.get('password')?.value,
      };

      this.authService.register(requestdata).subscribe({
        next: (response) => {
          console.log('Registration successful:', response);
          alert('Registration successful! Redirecting to login page...');
          this.router.navigate(['/login']);
        },
        error: (error) => {
          console.error('Registration error:', error);
          alert('Registration failed. Please try again.');
        },
      });
    } else {
      this.registerForm.markAllAsTouched();
    }
  }
}
