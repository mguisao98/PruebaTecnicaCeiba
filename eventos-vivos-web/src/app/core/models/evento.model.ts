export enum TipoEvento {
  Conferencia = 0,
  Taller = 1,
  Concierto = 2,
}

export enum EstadoEvento {
  Activo = 0,
  Cancelado = 1,
  Completado = 2,
}

export enum EstadoReserva {
  PendientePago = 0,
  Confirmada = 1,
  Cancelada = 2,
  Perdida = 3,
  Pagada = 4,
}

export const TIPO_EVENTO_LABELS: Record<TipoEvento, string> = {
  [TipoEvento.Conferencia]: 'Conferencia',
  [TipoEvento.Taller]: 'Taller',
  [TipoEvento.Concierto]: 'Concierto',
};

export const ESTADO_EVENTO_LABELS: Record<EstadoEvento, string> = {
  [EstadoEvento.Activo]: 'Activo',
  [EstadoEvento.Cancelado]: 'Cancelado',
  [EstadoEvento.Completado]: 'Completado',
};

export const ESTADO_RESERVA_LABELS: Record<EstadoReserva, string> = {
  [EstadoReserva.PendientePago]: 'Pendiente de pago',
  [EstadoReserva.Confirmada]: 'Confirmada',
  [EstadoReserva.Cancelada]: 'Cancelada',
  [EstadoReserva.Perdida]: 'Perdida',
  [EstadoReserva.Pagada]: 'Pagada',
};

export interface Venue {
  id: number;
  nombre: string;
  capacidad: number;
  ciudad: string;
}

export interface Reserva {
  id: number;
  eventoId: number;
  usuarioId: number;
  cantidad: number;
  nombreComprador: string;
  emailComprador: string;
  estado: EstadoReserva;
  codigoReserva?: string;
  fechaCancelacion?: string;
  fechaPago?: string;
}

export interface Evento {
  id: number;
  titulo: string;
  descripcion: string;
  venueId: number;
  venue?: Venue;
  capacidadMaxima: number;
  fechaInicio: string;
  fechaFin: string;
  precioEntrada: number;
  tipo: TipoEvento;
  estado: EstadoEvento;
  reservas?: Reserva[];
}

export interface EventoFiltro {
  tipo?: number;
  venueId?: number;
  estado?: number;
  fechaInicio?: string;
  fechaFin?: string;
  titulo?: string;
}

export interface ReporteOcupacion {
  eventoId: number;
  titulo: string;
  capacidadMaxima: number;
  entradasVendidas: number;
  ocupacion: number;
  estado: string;
  venue: string;
  entradasDisponibles: number;
  ingresos: number;
}

export interface CrearEventoDto {
  titulo: string;
  descripcion: string;
  venueId: number;
  capacidadMaxima: number;
  fechaInicio: string;
  fechaFin: string;
  precioEntrada: number;
  tipo: TipoEvento;
}

export interface CrearReservaDto {
  eventoId: number;
  usuarioId: number;
  cantidad: number;
  nombreComprador: string;
  emailComprador: string;
}
