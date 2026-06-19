import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Venue } from '../models/evento.model';

@Injectable({ providedIn: 'root' })
export class VenueService {
  private readonly baseUrl = '/api/Venues';
  private http = inject(HttpClient);

  listar(): Observable<Venue[]> {
    return this.http.get<Venue[]>(this.baseUrl);
  }
}
