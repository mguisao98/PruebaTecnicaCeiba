import { Pipe, PipeTransform } from '@angular/core';
import { ESTADO_EVENTO_LABELS, EstadoEvento } from '../../core/models/evento.model';

@Pipe({ name: 'estadoEvento', standalone: true })
export class EstadoEventoPipe implements PipeTransform {
  transform(value: number | undefined): string {
    if (value === undefined || value === null) return '—';
    return ESTADO_EVENTO_LABELS[value as EstadoEvento] ?? '—';
  }
}
