import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EventoService } from '../../core/services/evento.service';
import { VenueService } from '../../core/services/venue.service';
import { Evento, EventoFiltro, TipoEvento, EstadoEvento, Venue } from '../../core/models/evento.model';
import { TipoEventoPipe } from '../../shared/pipes/tipo-evento.pipe';
import { EstadoEventoPipe } from '../../shared/pipes/estado-evento.pipe';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-eventos-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TipoEventoPipe, EstadoEventoPipe],
  templateUrl: './eventos-list.html',
  styleUrl: './eventos-list.css',
})
export class EventosList implements OnInit {
  eventos: Evento[] = [];
  venues: Venue[] = [];
  cargando = false;
  error = '';

  filtro: EventoFiltro = {};
  tipoSeleccionado = '';
  estadoSeleccionado = '';
  venueSeleccionado = '';
  fechaInicio = '';
  fechaFin = '';
  titulo = '';

  readonly tiposEvento = [
    { value: TipoEvento.Conferencia, label: 'Conferencia' },
    { value: TipoEvento.Taller, label: 'Taller' },
    { value: TipoEvento.Concierto, label: 'Concierto' },
  ];

  readonly estadosEvento = [
    { value: EstadoEvento.Activo, label: 'Activo' },
    { value: EstadoEvento.Cancelado, label: 'Cancelado' },
    { value: EstadoEvento.Completado, label: 'Completado' },
  ];

  constructor(
    private eventoService: EventoService,
    private venueService: VenueService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit() {
    this.cargarVenues();
    this.cargarEventos();
  }

  cargarVenues() {
    this.venueService.listar().subscribe({
      next: (data) => { this.venues = data; this.cdr.markForCheck(); },
      error: () => {},
    });
  }

  cargarEventos() {
    this.cargando = true;
    this.error = '';
    this.eventoService.listar().subscribe({
      next: (data) => {
        this.eventos = data;
        this.cargando = false;
        this.cdr.markForCheck();
      },
      error: (err: HttpErrorResponse) => {
        this.error = 'No se pudieron cargar los eventos. Verifica la conexión con la API.';
        this.cargando = false;
        this.cdr.markForCheck();
      },
    });
  }

  aplicarFiltros() {
    const filtro: EventoFiltro = {};
    if (this.titulo.trim()) filtro.titulo = this.titulo.trim();
    if (this.tipoSeleccionado !== '') filtro.tipo = Number(this.tipoSeleccionado);
    if (this.estadoSeleccionado !== '') filtro.estado = Number(this.estadoSeleccionado);
    if (this.venueSeleccionado !== '') filtro.venueId = Number(this.venueSeleccionado);
    if (this.fechaInicio) filtro.fechaInicio = this.fechaInicio;
    if (this.fechaFin) filtro.fechaFin = this.fechaFin;

    const hayFiltros = Object.keys(filtro).length > 0;
    if (!hayFiltros) {
      this.cargarEventos();
      return;
    }

    this.cargando = true;
    this.error = '';
    this.eventoService.filtrar(filtro).subscribe({
      next: (data) => {
        this.eventos = data;
        this.cargando = false;
        this.cdr.markForCheck();
      },
      error: (err: HttpErrorResponse) => {
        this.error = 'Error al aplicar filtros.';
        this.cargando = false;
        this.cdr.markForCheck();
      },
    });
  }

  limpiarFiltros() {
    this.titulo = '';
    this.tipoSeleccionado = '';
    this.estadoSeleccionado = '';
    this.venueSeleccionado = '';
    this.fechaInicio = '';
    this.fechaFin = '';
    this.cargarEventos();
  }

  getEstadoClase(estado: number | undefined): string {
    switch (estado) {
      case EstadoEvento.Activo:
        return 'badge-activo';
      case EstadoEvento.Cancelado:
        return 'badge-cancelado';
      case EstadoEvento.Completado:
        return 'badge-completado';
      default:
        return '';
    }
  }

  getTipoClase(tipo: number | undefined): string {
    switch (tipo) {
      case TipoEvento.Conferencia:
        return 'badge-conferencia';
      case TipoEvento.Taller:
        return 'badge-taller';
      case TipoEvento.Concierto:
        return 'badge-concierto';
      default:
        return '';
    }
  }

  formatDate(fecha: string): string {
    if (!fecha) return '—';
    return new Date(fecha).toLocaleString('es-CO', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }
}
