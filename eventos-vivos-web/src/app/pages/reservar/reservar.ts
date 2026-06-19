import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { EventoService } from '../../core/services/evento.service';
import { ReservaService } from '../../core/services/reserva.service';
import { Evento, EstadoReserva, Reserva } from '../../core/models/evento.model';
import { TipoEventoPipe } from '../../shared/pipes/tipo-evento.pipe';

@Component({
  selector: 'app-reservar',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TipoEventoPipe],
  templateUrl: './reservar.html',
  styleUrl: './reservar.css',
})
export class ReservarComponent implements OnInit {
  evento: Evento | null = null;
  reservaCreada: Reserva | null = null;
  form: FormGroup;
  cargando = true;
  enviando = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private eventoService: EventoService,
    private reservaService: ReservaService,
    private cdr: ChangeDetectorRef,
  ) {
    this.form = this.fb.group({
      cantidad: [1, [Validators.required, Validators.min(1)]],
      nombreComprador: ['', [Validators.required, Validators.minLength(2)]],
      emailComprador: ['', [Validators.required, Validators.email]],
    });
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id || isNaN(id)) {
      this.error = 'ID de evento inválido.';
      this.cargando = false;
      return;
    }

    this.eventoService.obtenerPorId(id).subscribe({
      next: (data) => {
        this.evento = data;
        this.cargando = false;
        const max = this.cuposDisponibles;
        this.form.get('cantidad')?.setValidators([Validators.required, Validators.min(1), Validators.max(max)]);
        this.form.get('cantidad')?.updateValueAndValidity();
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'No se pudo cargar la información del evento.';
        this.cargando = false;
        this.cdr.markForCheck();
      },
    });
  }

  get cuposDisponibles(): number {
    if (!this.evento) return 0;
    const reservadas = this.evento.reservas
      ?.filter((r) => r.estado !== EstadoReserva.Cancelada && r.estado !== EstadoReserva.Perdida)
      .reduce((acc, r) => acc + (r.cantidad || 0), 0) ?? 0;
    return Math.max(this.evento.capacidadMaxima - reservadas, 0);
  }

  reservar(): void {
    if (this.form.invalid || !this.evento) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando = true;
    this.error = '';
    const valores = this.form.value;

    this.reservaService
      .crear({
        eventoId: this.evento.id,
        usuarioId: 1,
        cantidad: Number(valores.cantidad),
        nombreComprador: valores.nombreComprador.trim(),
        emailComprador: valores.emailComprador.trim(),
      })
      .subscribe({
        next: (reserva) => {
          this.enviando = false;
          this.reservaCreada = reserva;
          this.cdr.markForCheck();
        },
        error: (err: HttpErrorResponse) => {
          this.enviando = false;
          this.error = err.error?.error ?? 'No se pudo completar la reserva. Intenta nuevamente.';
          this.cdr.markForCheck();
        },
      });
  }

  fieldError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl || !ctrl.touched || ctrl.valid) return '';
    if (ctrl.errors?.['required']) return 'Este campo es obligatorio.';
    if (ctrl.errors?.['min']) return `El mínimo es ${ctrl.errors['min'].min}.`;
    if (ctrl.errors?.['max']) return `Solo hay ${ctrl.errors['max'].max} cupos disponibles.`;
    if (ctrl.errors?.['minlength']) return `Mínimo ${ctrl.errors['minlength'].requiredLength} caracteres.`;
    if (ctrl.errors?.['email']) return 'Ingresa un email válido.';
    return 'Valor inválido.';
  }

  formatDate(fecha: string): string {
    if (!fecha) return '—';
    return new Date(fecha).toLocaleString('es-CO', {
      day: '2-digit',
      month: 'long',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }
}
