using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Application.Services;

public class EventService
{
    private readonly AppDbContext _context;

    public EventService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Evento> CrearEventoAsync(Evento evento)
    {
        if (string.IsNullOrWhiteSpace(evento.Titulo) || evento.Titulo.Length < 5 || evento.Titulo.Length > 100)
            throw new DomainException("El título debe tener entre 5 y 100 caracteres.");

        if (string.IsNullOrWhiteSpace(evento.Descripcion) || evento.Descripcion.Length < 10 || evento.Descripcion.Length > 500)
            throw new DomainException("La descripción debe tener entre 10 y 500 caracteres.");

        var venue = await _context.Venues.FindAsync(evento.VenueId);
        if (venue == null)
            throw new NotFoundException("El venue no existe.");

        if (evento.CapacidadMaxima <= 0)
            throw new DomainException("La capacidad máxima debe ser un entero positivo.");

        if (evento.CapacidadMaxima > venue.Capacidad)
            throw new DomainException("La capacidad máxima del evento no puede exceder la capacidad del venue.");

        if (evento.FechaInicio <= DateTime.UtcNow)
            throw new DomainException("La fecha de inicio debe ser futura.");

        if (evento.FechaFin <= evento.FechaInicio)
            throw new DomainException("La fecha de fin debe ser posterior al inicio.");

        if (evento.PrecioEntrada <= 0)
            throw new DomainException("El precio debe ser positivo.");

        if (!Enum.IsDefined(typeof(TipoEvento), evento.Tipo))
            throw new DomainException("Tipo de evento inválido.");

        bool superpuesto = await _context.Eventos.AnyAsync(e =>
            e.VenueId == evento.VenueId &&
            e.Estado == EstadoEvento.Activo &&
            evento.FechaInicio < e.FechaFin &&
            evento.FechaFin > e.FechaInicio);

        if (superpuesto)
            throw new DomainException("Ya existe un evento activo en este venue con horarios superpuestos.");

        if ((evento.FechaInicio.DayOfWeek == DayOfWeek.Saturday ||
             evento.FechaInicio.DayOfWeek == DayOfWeek.Sunday) &&
             evento.FechaInicio.Hour >= 22)
        {
            throw new DomainException("Los eventos en fin de semana no pueden iniciar después de las 22:00.");
        }

        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();

        return evento;
    }

    public async Task<List<Evento>> ObtenerEventosAsync()
    {
        return await _context.Eventos
            .Include(e => e.Venue)
            .ToListAsync();
    }

    public async Task<List<Evento>> FiltrarEventosAsync(
        TipoEvento? tipo,
        int? venueId,
        EstadoEvento? estado,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? titulo)
    {
        var query = _context.Eventos
            .Include(e => e.Venue)
            .AsQueryable();

        if (tipo.HasValue)
            query = query.Where(e => e.Tipo == tipo);

        if (venueId.HasValue)
            query = query.Where(e => e.VenueId == venueId);

        if (estado.HasValue)
            query = query.Where(e => e.Estado == estado);

        if (fechaInicio.HasValue)
            query = query.Where(e => e.FechaInicio >= fechaInicio);

        if (fechaFin.HasValue)
            query = query.Where(e => e.FechaFin <= fechaFin);

        if (!string.IsNullOrWhiteSpace(titulo))
            query = query.Where(e => e.Titulo.ToLower().Contains(titulo.ToLower()));

        return await query.ToListAsync();
    }

    public async Task<Evento> ObtenerEventoPorIdAsync(int id)
    {
        var evento = await _context.Eventos
            .Include(e => e.Reservas)
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento == null)
            throw new NotFoundException("El evento no existe.");

        if (DateTime.UtcNow > evento.FechaFin && evento.Estado == EstadoEvento.Activo)
        {
            evento.Estado = EstadoEvento.Completado;
            await _context.SaveChangesAsync();
        }

        return evento;
    }
}
