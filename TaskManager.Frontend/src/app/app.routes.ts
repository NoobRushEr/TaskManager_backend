import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { authGuard } from './core/guards/auth.guard';
import { TaskListComponent } from './features/tasks/task-list/task-list.component';

export const routes: Routes = [
    {path: '', redirectTo: 'login', pathMatch: 'full'},

    {path: 'login', component: LoginComponent},
    {path: 'register', component: RegisterComponent},

    //protected routes can be added here,
    {path: 'tasks', component: TaskListComponent, canActivate: [authGuard]},

    {path: '**', redirectTo: 'login'}
];
