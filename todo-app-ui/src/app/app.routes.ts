import { Routes } from '@angular/router';

import { AppShell } from './layout/app-shell/app-shell';

import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';

import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [

  {
    path: 'login',
    component: Login
  },

  {
    path: 'register',
    component: Register
  },

  {
    path: '',
    component: AppShell,
    canActivate: [authGuard]
  },

  {
    path: '**',
    redirectTo: ''
  }
];
