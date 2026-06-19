using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EventosVivos.Application.Services;

public class ReservationService
{
    private readonly AppDbContext _context;
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public ReservationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Reserva> CrearReservaAsync(Reserva reserva)
    {
        var evento = await _context.Eventos
            .Include(e => e.Reservas)
            .FirstOrDefaultAsync(e => e.Id == reserva.EventoId);

        if (evento == null)
            throw new NotFoundException("El evento no existe.");

        if (evento.Estado != EstadoEvento.Activo)
            throw new DomainException("El evento no está activo.");

        var tiempoRestante = evento.FechaInicio - DateTime.UtcNow;

        if (tiempoRestante.TotalHours < 1)
            throw new DomainException("No se permiten reservas para eventos que inician en menos de 1 hora.");

        if (reserva.Cantidad < 1)
            throw new DomainException("La cantidad mínima de entradas es 1.");

        if (string.IsNullOrWhiteSpace(reserva.EmailComprador) || !EmailRegex.IsMatch(reserva.EmailComprador))
            throw new DomainException("El email del comprador no es válido.");

        if (tiempoRestante.TotalHours < 24 && reserva.Cantidad > 5)
            throw new DomainException("Con menos de 24 horas para el evento, solo se permiten máximo 5 entradas por transacción.");

        if (evento.PrecioEntrada > 100 && reserva.Cantidad > 10)
            throw new DomainException("Este evento tiene precio mayor a $100, solo se permiten máximo 10 entradas por transacción.");

        int entradasOcupadas = (evento.Reservas ?? [])
            .Where(r => r.Estado != EstadoReserva.Cancelada)
            .Sum(r => r.Cantidad);

        if (entradasOcupadas + reserva.Cantidad > evento.CapacidadMaxima)
            throw new DomainException("No hay cupos suficientes para esta reserva.");

        reserva.Estado = EstadoReserva.PendientePago;

        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync();

        return reserva;
    }

    public async Task<Reserva> ConfirmarPagoAsync(int reservaId)
    {
        var reserva = await _context.Reservas
            .Include(r => r.Evento)
            .FirstOrDefaultAsync(r => r.Id == reservaId);

        if (reserva == null)
            throw new NotFoundException("La reserva no existe.");

        if (reserva.Estado == EstadoReserva.Cancelada || reserva.Estado == EstadoReserva.Perdida)
            throw new DomainException("No se puede confirmar una reserva cancelada.");

        if (reserva.Estado == EstadoReserva.Confirmada)
            throw new DomainException("La reserva ya está confirmada.");

        reserva.Estado = EstadoReserva.Confirmada;
        reserva.CodigoReserva = $"EV-{Random.Shared.Next(0, 999999):D6}";
        reserva.FechaPago = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return reserva;
    }

    public async Task<Reserva> CancelarReservaAsync(int reservaId)
    {
        var reserva = await _context.Reservas
            .Include(r => r.Evento)
            .FirstOrDefaultAsync(r => r.Id == reservaId);

        if (reserva == null)
            throw new NotFoundException("La reserva no existe.");

        if (reserva.Estado == EstadoReserva.Cancelada)
            throw new DomainException("La reserva ya está cancelada.");

        if (reserva.Estado == EstadoReserva.Perdida)
            throw new DomainException("La reserva en estado perdida no puede cancelarse.");

        if (reserva.Estado == EstadoReserva.Confirmada)
        {
            var horasRestantes = (reserva.Evento!.FechaInicio - DateTime.UtcNow).TotalHours;

            if (horasRestantes < 48)
            {
                reserva.Estado = EstadoReserva.Perdida;
            }
            else
            {
                reserva.Estado = EstadoReserva.Cancelada;
                reserva.FechaCancelacion = DateTime.UtcNow;
            }
        }
        else
        {
            reserva.Estado = EstadoReserva.Cancelada;
            reserva.FechaCancelacion = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return reserva;
    }

    public async Task<ReporteOcupacionDto> ObtenerReporteOcupacionAsync(int eventoId)
    {
        var evento = await _context.Eventos
            .Include(e => e.Reservas)
            .Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
            throw new NotFoundException("El evento no existe.");

        var reservas = evento.Reservas ?? [];

        var entradasVendidas = reservas
            .Where(r => r.Estado == EstadoReserva.Confirmada || r.Estado == EstadoReserva.Pagada)
            .Sum(r => r.Cantidad);

        var entradasBloqueadas = reservas
            .Where(r => r.Estado != EstadoReserva.Cancelada)
            .Sum(r => r.Cantidad);

        var entradasDisponibles = evento.CapacidadMaxima - entradasBloqueadas;
        var ingresos = entradasVendidas * evento.PrecioEntrada;
        var ocupacion = evento.CapacidadMaxima > 0
            ? Math.Round((double)entradasVendidas / evento.CapacidadMaxima * 100, 2)
            : 0;

        return new ReporteOcupacionDto
        {
            EventoId = evento.Id,
            Titulo = evento.Titulo,
            CapacidadMaxima = evento.CapacidadMaxima,
            EntradasVendidas = entradasVendidas,
            EntradasDisponibles = entradasDisponibles,
            Ingresos = ingresos,
            Ocupacion = ocupacion,
            Estado = evento.Estado.ToString(),
            Venue = evento.Venue?.Nombre ?? "Sin venue"
        };
    }

    public async Task<List<Reserva>> ObtenerReservasPorEventoAsync(int eventoId)
    {
        var evento = await _context.Eventos
            .Include(e => e.Reservas)
            .FirstOrDefaultAsync(e => e.Id == eventoId);

        if (evento == null)
            throw new NotFoundException("El evento no existe.");

        return evento.Reservas?.ToList() ?? [];
    }
}
