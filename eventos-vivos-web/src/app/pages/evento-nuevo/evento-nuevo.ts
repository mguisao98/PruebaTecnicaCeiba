import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { EventoService } from '../../core/services/evento.service';
import { VenueService } from '../../core/services/venue.service';
import { Venue, TipoEvento } from '../../core/models/evento.model';

@Component({
  selector: 'app-evento-nuevo',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './evento-nuevo.html',
  styleUrl: './evento-nuevo.css',
})
export class EventoNuevo implements OnInit {
  form: FormGroup;
  venues: Venue[] = [];
  enviando = false;
  error = '';
  exito = '';

  readonly tiposEvento = [
    { value: TipoEvento.Conferencia, label: 'Conferencia' },
    { value: TipoEvento.Taller, label: 'Taller' },
    { value: TipoEvento.Concierto, label: 'Concierto' },
  ];

  constructor(
    private fb: FormBuilder,
    private eventoService: EventoService,
    private venueService: VenueService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {
    this.form = this.fb.group({
      titulo: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
      descripcion: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      venueId: ['', [Validators.required]],
      capacidadMaxima: ['', [Validators.required, Validators.min(1)]],
      fechaInicio: ['', [Validators.required]],
      fechaFin: ['', [Validators.required]],
      precioEntrada: ['', [Validators.required, Validators.min(0.01)]],
      tipo: ['', [Validators.required]],
    });
  }

  ngOnInit() {
    this.venueService.listar().subscribe({
      next: (data) => { this.venues = data; this.cdr.markForCheck(); },
      error: () => { this.error = 'No se pudieron cargar los venues.'; this.cdr.markForCheck(); },
    });
  }

  get f() {
    return this.form.controls;
  }

  crear() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando = true;
    this.error = '';
    this.exito = '';

    const valores = this.form.value;
    const dto = {
      titulo: valores.titulo,
      descripcion: valores.descripcion,
      venueId: Number(valores.venueId),
      capacidadMaxima: Number(valores.capacidadMaxima),
      fechaInicio: valores.fechaInicio,
      fechaFin: valores.fechaFin,
      precioEntrada: Number(valores.precioEntrada),
      tipo: Number(valores.tipo),
    };

    this.eventoService.crear(dto).subscribe({
      next: (evento) => {
        this.enviando = false;
        this.exito = `Evento "${evento.titulo}" creado correctamente.`;
        this.cdr.markForCheck();
        setTimeout(() => this.router.navigate(['/eventos', evento.id]), 1500);
      },
      error: (err: HttpErrorResponse) => {
        this.enviando = false;
        this.error = err.error?.error ?? 'No se pudo crear el evento. Verifica los datos.';
        this.cdr.markForCheck();
      },
    });
  }

  fieldError(field: string): string {
    const ctrl = this.form.get(field);
    if (!ctrl || !ctrl.touched || ctrl.valid) return '';
    if (ctrl.errors?.['required']) return 'Este campo es obligatorio.';
    if (ctrl.errors?.['minlength']) {
      const req = ctrl.errors['minlength'].requiredLength;
      return `Mínimo ${req} caracteres.`;
    }
    if (ctrl.errors?.['maxlength']) {
      const req = ctrl.errors['maxlength'].requiredLength;
      return `Máximo ${req} caracteres.`;
    }
    if (ctrl.errors?.['min']) return `El valor mínimo es ${ctrl.errors['min'].min}.`;
    return 'Valor inválido.';
  }
}
