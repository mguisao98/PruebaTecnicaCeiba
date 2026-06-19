import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CrearReservaDto, Reserva } from '../models/evento.model';

@Injectable({ providedIn: 'root' })
export class ReservaService {
  // private readonly baseUrl = '/api/Reservas';
  private readonly baseUrl = 'https://localhost:7258/api/Reservas';

  private http = inject(HttpClient);

  crear(reserva: CrearReservaDto): Observable<Reserva> {
    return this.http.post<Reserva>(this.baseUrl, reserva);
  }

  confirmarPago(id: number): Observable<Reserva> {
    return this.http.put<Reserva>(`${this.baseUrl}/confirmar/${id}`, {});
  }

  cancelar(id: number): Observable<Reserva> {
    return this.http.put<Reserva>(`${this.baseUrl}/cancelar/${id}`, {});
  }

  listarPorEvento(eventoId: number): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(`${this.baseUrl}/evento/${eventoId}`);
  }
}
