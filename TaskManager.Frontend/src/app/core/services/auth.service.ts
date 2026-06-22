import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { AuthResponse, RegisterRequest, LoginRequest } from '../models/auth.model';
import { tap } from 'rxjs/internal/operators/tap';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Auth`; // Replace with your actual API URL

  private currentUser =  new BehaviorSubject<AuthResponse | null>(
    JSON.parse(localStorage.getItem('currentUser') || 'null')
  );

  public currentUser$ = this.currentUser.asObservable();

  register(userData: RegisterRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, userData);
  }

  login(credentials: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((response: AuthResponse) => {
        if (response.isSuccess && response.data) {
          localStorage.setItem('token', response.data.token);
          localStorage.setItem('currentUser', JSON.stringify(response));
          this.currentUser.next(response);
        }
      })
    )
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this.currentUser.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
  
}
