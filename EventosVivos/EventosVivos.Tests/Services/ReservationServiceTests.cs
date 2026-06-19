using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using EventosVivos.Application.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Tests.Services;

public class ReservationServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private async Task<Evento> SeedEventoActivo(AppDbContext context, DateTime? fechaInicio = null, decimal precio = 50m, int capacidad = 50)
    {
        var inicio = fechaInicio ?? DateTime.UtcNow.AddDays(5);
        context.Venues.Add(new Venue { Id = 1, Nombre = "Venue Test", Capacidad = 200, Ciudad = "Bogotá" });
        var evento = new Evento
        {
            Titulo = "Evento de prueba activo",
            Descripcion = "Descripción del evento de prueba activo en el sistema",
            VenueId = 1,
            CapacidadMaxima = capacidad,
            FechaInicio = inicio,
            FechaFin = inicio.AddHours(3),
            PrecioEntrada = precio,
            Tipo = TipoEvento.Conferencia,
            Estado = EstadoEvento.Activo
        };
        context.Eventos.Add(evento);
        await context.SaveChangesAsync();
        return evento;
    }

    [Fact]
    public async Task CrearReserva_Valida_RetornaEstadoPendientePago()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context);
        var service = new ReservationService(context);

        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 2,
            NombreComprador = "Juan Pérez",
            EmailComprador = "juan@ejemplo.com"
        };

        var resultado = await service.CrearReservaAsync(reserva);

        Assert.NotNull(resultado);
        Assert.Equal(EstadoReserva.PendientePago, resultado.Estado);
    }

    [Fact]
    public async Task CrearReserva_EmailInvalido_ArrojaDomainException()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context);
        var service = new ReservationService(context);

        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 1,
            NombreComprador = "Test Usuario",
            EmailComprador = "correo-invalido"
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearReservaAsync(reserva));
    }

    [Fact]
    public async Task CrearReserva_SinCapacidad_ArrojaDomainException()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, capacidad: 10);

        context.Reservas.Add(new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 10,
            NombreComprador = "Ocupante Total",
            EmailComprador = "ocupante@test.com",
            Estado = EstadoReserva.Confirmada
        });
        await context.SaveChangesAsync();

        var service = new ReservationService(context);
        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 1,
            NombreComprador = "Sin Cupo",
            EmailComprador = "sincupo@test.com"
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearReservaAsync(reserva));
    }

    [Fact]
    public async Task CrearReserva_MenosDe1Hora_ArrojaDomainException()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, fechaInicio: DateTime.UtcNow.AddMinutes(30));
        var service = new ReservationService(context);

        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 1,
            NombreComprador = "Tardío",
            EmailComprador = "tardio@test.com"
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearReservaAsync(reserva));
    }

    [Fact]
    public async Task CrearReserva_MenosDe24HorasMasDe5Entradas_ArrojaDomainException()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, fechaInicio: DateTime.UtcNow.AddHours(5));
        var service = new ReservationService(context);

        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 6,
            NombreComprador = "Tardío Bulk",
            EmailComprador = "tardiobulk@test.com"
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearReservaAsync(reserva));
    }

    [Fact]
    public async Task CrearReserva_PrecioMayor100MasDe10Entradas_ArrojaDomainException()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, precio: 150m, capacidad: 200);
        var service = new ReservationService(context);

        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 11,
            NombreComprador = "VIP Comprador",
            EmailComprador = "vip@test.com"
        };

        await Assert.ThrowsAsync<DomainException>(() => service.CrearReservaAsync(reserva));
    }

    [Fact]
    public async Task ConfirmarPago_Valido_GeneraCodigoEV()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context);
        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 2,
            NombreComprador = "Ana García",
            EmailComprador = "ana@test.com",
            Estado = EstadoReserva.PendientePago
        };
        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        var service = new ReservationService(context);
        var resultado = await service.ConfirmarPagoAsync(reserva.Id);

        Assert.Equal(EstadoReserva.Confirmada, resultado.Estado);
        Assert.NotNull(resultado.CodigoReserva);
        Assert.StartsWith("EV-", resultado.CodigoReserva);
        Assert.NotNull(resultado.FechaPago);
    }

    [Fact]
    public async Task ConfirmarPago_YaConfirmada_ArrojaDomainException()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context);
        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 1,
            NombreComprador = "Carlos",
            EmailComprador = "carlos@test.com",
            Estado = EstadoReserva.Confirmada,
            CodigoReserva = "EV-000001"
        };
        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        var service = new ReservationService(context);

        await Assert.ThrowsAsync<DomainException>(() => service.ConfirmarPagoAsync(reserva.Id));
    }

    [Fact]
    public async Task CancelarReserva_ConfirmadaConMasDe48Horas_CambiaEstadoCancelada()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, fechaInicio: DateTime.UtcNow.AddDays(10));
        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 2,
            NombreComprador = "María López",
            EmailComprador = "maria@test.com",
            Estado = EstadoReserva.Confirmada
        };
        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        var service = new ReservationService(context);
        var resultado = await service.CancelarReservaAsync(reserva.Id);

        Assert.Equal(EstadoReserva.Cancelada, resultado.Estado);
        Assert.NotNull(resultado.FechaCancelacion);
    }

    [Fact]
    public async Task CancelarReserva_ConfirmadaMenosDe48Horas_MarcaPerdida()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, fechaInicio: DateTime.UtcNow.AddHours(10));
        var reserva = new Reserva
        {
            EventoId = evento.Id,
            Cantidad = 1,
            NombreComprador = "Pedro Rojas",
            EmailComprador = "pedro@test.com",
            Estado = EstadoReserva.Confirmada
        };
        context.Reservas.Add(reserva);
        await context.SaveChangesAsync();

        var service = new ReservationService(context);
        var resultado = await service.CancelarReservaAsync(reserva.Id);

        Assert.Equal(EstadoReserva.Perdida, resultado.Estado);
    }

    [Fact]
    public async Task ObtenerReporte_EventoConReservas_RetornaDatosCorrectos()
    {
        using var context = CreateContext();
        var evento = await SeedEventoActivo(context, capacidad: 50);
        context.Reservas.AddRange(
            new Reserva { EventoId = evento.Id, Cantidad = 10, NombreComprador = "Grupo A", EmailComprador = "a@a.com", Estado = EstadoReserva.Confirmada },
            new Reserva { EventoId = evento.Id, Cantidad = 5, NombreComprador = "Grupo B", EmailComprador = "b@b.com", Estado = EstadoReserva.PendientePago }
        );
        await context.SaveChangesAsync();

        var service = new ReservationService(context);
        var reporte = await service.ObtenerReporteOcupacionAsync(evento.Id);

        Assert.Equal(10, reporte.EntradasVendidas);
        Assert.Equal(35, reporte.EntradasDisponibles);
        Assert.Equal(500m, reporte.Ingresos);
        Assert.Equal(20.0, reporte.Ocupacion);
    }
}
