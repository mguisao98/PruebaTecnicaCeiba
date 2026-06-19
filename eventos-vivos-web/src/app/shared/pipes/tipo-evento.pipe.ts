import { Pipe, PipeTransform } from '@angular/core';
import { TIPO_EVENTO_LABELS, TipoEvento } from '../../core/models/evento.model';

@Pipe({ name: 'tipoEvento', standalone: true })
export class TipoEventoPipe implements PipeTransform {
  transform(value: number | undefined): string {
    if (value === undefined || value === null) return '—';
    return TIPO_EVENTO_LABELS[value as TipoEvento] ?? '—';
  }
}
