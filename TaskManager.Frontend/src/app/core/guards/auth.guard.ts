import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  let isAuthenticated = false;
  // Subscribe briefly to check if a user is logged in
  authService.currentUser$.subscribe(user => isAuthenticated = !!user).unsubscribe();

  if (isAuthenticated) {
    return true;
  }

  // Not authenticated, redirect to login page
  router.navigate(['/login']);
  return false;
};