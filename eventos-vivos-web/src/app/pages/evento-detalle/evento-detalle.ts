import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EventoService } from '../../core/services/evento.service';
import { Evento, ReporteOcupacion, EstadoEvento } from '../../core/models/evento.model';
import { TipoEventoPipe } from '../../shared/pipes/tipo-evento.pipe';
import { EstadoEventoPipe } from '../../shared/pipes/estado-evento.pipe';

@Component({
  selector: 'app-evento-detalle',
  standalone: true,
  imports: [CommonModule, RouterLink, TipoEventoPipe, EstadoEventoPipe],
  templateUrl: './evento-detalle.html',
  styleUrl: './evento-detalle.css',
})
export class EventoDetalle implements OnInit {
  evento: Evento | null = null;
  reporte: ReporteOcupacion | null = null;
  cargando = true;
  cargandoReporte = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private eventoService: EventoService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    const id = Number(this.route.snapshot.params['id']);
    if (!id || isNaN(id)) {
      this.error = 'ID de evento inválido.';
      this.cargando = false;
      return;
    }

    this.eventoService.obtenerPorId(id).subscribe({
      next: (data) => {
        this.evento = data;
        this.cargando = false;
        this.cdr.markForCheck();
        this.cargarReporte(id);
      },
      error: () => {
        this.error = 'No se pudo cargar el evento.';
        this.cargando = false;
        this.cdr.markForCheck();
      },
    });
  }

  cargarReporte(id: number) {
    this.cargandoReporte = true;
    this.eventoService.obtenerReporte(id).subscribe({
      next: (data) => {
        this.reporte = data;
        this.cargandoReporte = false;
        this.cdr.markForCheck();
      },
      error: () => { this.cargandoReporte = false; this.cdr.markForCheck(); },
    });
  }

  get esActivo(): boolean {
    return this.evento?.estado === EstadoEvento.Activo;
  }

  formatDate(fecha: string): string {
    if (!fecha) return '—';
    return new Date(fecha).toLocaleString('es-CO', {
      weekday: 'long',
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }
}
