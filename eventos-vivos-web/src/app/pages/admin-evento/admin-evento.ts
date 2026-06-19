import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { EventoService } from '../../core/services/evento.service';
import { ReservaService } from '../../core/services/reserva.service';
import { Evento, Reserva, EstadoReserva } from '../../core/models/evento.model';
import { EstadoEventoPipe } from '../../shared/pipes/estado-evento.pipe';
import { EstadoReservaPipe } from '../../shared/pipes/estado-reserva.pipe';
import { TipoEventoPipe } from '../../shared/pipes/tipo-evento.pipe';

interface ReservaConAccion extends Reserva {
  procesando?: boolean;
  mensajeError?: string;
  mensajeExito?: string;
}

@Component({
  selector: 'app-admin-evento',
  standalone: true,
  imports: [CommonModule, RouterLink, EstadoEventoPipe, EstadoReservaPipe, TipoEventoPipe],
  templateUrl: './admin-evento.html',
  styleUrl: './admin-evento.css',
})
export class AdminEventoComponent implements OnInit {
  evento: Evento | null = null;
  reservas: ReservaConAccion[] = [];
  cargando = true;
  error = '';

  readonly EstadoReserva = EstadoReserva;

  constructor(
    private route: ActivatedRoute,
    private eventoService: EventoService,
    private reservaService: ReservaService,
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
      next: (evento) => {
        this.evento = evento;
        this.cdr.markForCheck();
        this.cargarReservas(id);
      },
      error: () => {
        this.error = 'No se pudo cargar el evento.';
        this.cargando = false;
        this.cdr.markForCheck();
      },
    });
  }

  cargarReservas(eventoId: number) {
    this.reservaService.listarPorEvento(eventoId).subscribe({
      next: (data) => {
        this.reservas = data.map((r) => ({ ...r }));
        this.cargando = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'No se pudieron cargar las reservas.';
        this.cargando = false;
        this.cdr.markForCheck();
      },
    });
  }

  confirmarPago(reserva: ReservaConAccion) {
    reserva.procesando = true;
    reserva.mensajeError = '';
    reserva.mensajeExito = '';

    this.reservaService.confirmarPago(reserva.id).subscribe({
      next: (updated) => {
        reserva.estado = updated.estado;
        reserva.codigoReserva = updated.codigoReserva;
        reserva.fechaPago = updated.fechaPago;
        reserva.procesando = false;
        reserva.mensajeExito = `Pago confirmado. Código: ${updated.codigoReserva}`;
        this.cdr.markForCheck();
      },
      error: (err: HttpErrorResponse) => {
        reserva.procesando = false;
        reserva.mensajeError = err.error?.error ?? 'No se pudo confirmar el pago.';
        this.cdr.markForCheck();
      },
    });
  }

  cancelarReserva(reserva: ReservaConAccion) {
    if (!confirm(`¿Confirmar la cancelación de la reserva #${reserva.id}?`)) return;

    reserva.procesando = true;
    reserva.mensajeError = '';
    reserva.mensajeExito = '';

    this.reservaService.cancelar(reserva.id).subscribe({
      next: (updated) => {
        reserva.estado = updated.estado;
        reserva.fechaCancelacion = updated.fechaCancelacion;
        reserva.procesando = false;
        reserva.mensajeExito = 'Reserva cancelada correctamente.';
        this.cdr.markForCheck();
      },
      error: (err: HttpErrorResponse) => {
        reserva.procesando = false;
        reserva.mensajeError = err.error?.error ?? 'No se pudo cancelar la reserva.';
        this.cdr.markForCheck();
      },
    });
  }

  puedeConfirmar(r: Reserva): boolean {
    return r.estado === EstadoReserva.PendientePago;
  }

  puedeCancelar(r: Reserva): boolean {
    return r.estado === EstadoReserva.Confirmada;
  }

  getEstadoClase(estado: EstadoReserva): string {
    switch (estado) {
      case EstadoReserva.PendientePago: return 'badge-pendiente';
      case EstadoReserva.Confirmada:    return 'badge-confirmada';
      case EstadoReserva.Cancelada:     return 'badge-cancelada';
      case EstadoReserva.Perdida:       return 'badge-perdida';
      case EstadoReserva.Pagada:        return 'badge-pagada';
      default: return '';
    }
  }

  formatDate(fecha: string | undefined): string {
    if (!fecha) return '—';
    return new Date(fecha).toLocaleString('es-CO', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }

  get totalConfirmadas(): number {
    return this.reservas.filter(r => r.estado === EstadoReserva.Confirmada || r.estado === EstadoReserva.Pagada).reduce((s, r) => s + r.cantidad, 0);
  }

  get totalPendientes(): number {
    return this.reservas.filter(r => r.estado === EstadoReserva.PendientePago).reduce((s, r) => s + r.cantidad, 0);
  }
}
