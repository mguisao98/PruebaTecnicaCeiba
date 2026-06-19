import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CrearEventoDto, Evento, EventoFiltro, ReporteOcupacion } from '../models/evento.model';

@Injectable({ providedIn: 'root' })
export class EventoService {
  // private readonly baseUrl = '/api/Eventos';
private readonly baseUrl = 'https://localhost:7258/api/Eventos';


  private http = inject(HttpClient);

  listar(): Observable<Evento[]> {
    return this.http.get<Evento[]>(this.baseUrl);
  }

  filtrar(filtro: EventoFiltro): Observable<Evento[]> {
    let params = new HttpParams();
    if (filtro.tipo !== undefined) params = params.set('tipo', filtro.tipo.toString());
    if (filtro.venueId !== undefined) params = params.set('venueId', filtro.venueId.toString());
    if (filtro.estado !== undefined) params = params.set('estado', filtro.estado.toString());
    if (filtro.fechaInicio) params = params.set('fechaInicio', filtro.fechaInicio);
    if (filtro.fechaFin) params = params.set('fechaFin', filtro.fechaFin);
    if (filtro.titulo) params = params.set('titulo', filtro.titulo);
    return this.http.get<Evento[]>(`${this.baseUrl}/filtrar`, { params });
  }

  obtenerPorId(id: number): Observable<Evento> {
    return this.http.get<Evento>(`${this.baseUrl}/${id}`);
  }

  crear(evento: CrearEventoDto): Observable<Evento> {
    return this.http.post<Evento>(this.baseUrl, evento);
  }

  obtenerReporte(eventoId: number): Observable<ReporteOcupacion> {
    return this.http.get<ReporteOcupacion>(`${this.baseUrl}/${eventoId}/reporte-ocupacion`);
  }
}
