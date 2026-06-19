import { Routes } from '@angular/router';
import { EventosList } from '../eventos-list/eventos-list';
import { EventoDetalle } from './evento-detalle';

export const routes: Routes = [
  {
    path: '',
    component: EventosList
  },
  {
    path: 'eventos/:id',
    component: EventoDetalle
  }
];
