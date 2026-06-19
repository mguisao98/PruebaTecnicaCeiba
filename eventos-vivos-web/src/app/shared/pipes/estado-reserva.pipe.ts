import { Pipe, PipeTransform } from '@angular/core';
import { ESTADO_RESERVA_LABELS, EstadoReserva } from '../../core/models/evento.model';

@Pipe({ name: 'estadoReserva', standalone: true })
export class EstadoReservaPipe implements PipeTransform {
  transform(value: number | undefined): string {
    if (value === undefined || value === null) return '—';
    return ESTADO_RESERVA_LABELS[value as EstadoReserva] ?? '—';
  }
}
