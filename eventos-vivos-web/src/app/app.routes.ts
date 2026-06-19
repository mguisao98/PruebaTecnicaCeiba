import { Routes } from '@angular/router';
import { EventosList } from './pages/eventos-list/eventos-list';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'eventos',
    pathMatch: 'full',
  },
  {
    path: 'eventos',
    component: EventosList,
  },
  {
    path: 'eventos/nuevo',
    loadComponent: () =>
      import('./pages/evento-nuevo/evento-nuevo').then((m) => m.EventoNuevo),
  },
  {
    path: 'eventos/:id',
    loadComponent: () =>
      import('./pages/evento-detalle/evento-detalle').then((m) => m.EventoDetalle),
  },
  {
    path: 'reservar/:id',
    loadComponent: () =>
      import('./pages/reservar/reservar').then((m) => m.ReservarComponent),
  },
  {
    path: 'admin/evento/:id',
    loadComponent: () =>
      import('./pages/admin-evento/admin-evento').then((m) => m.AdminEventoComponent),
  },
];
