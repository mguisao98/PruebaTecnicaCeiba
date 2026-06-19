using Domain.Entities;
using EventosVivos.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly ReservationService _reservationService;

    public ReservasController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearReserva([FromBody] Reserva reserva)
    {
        var nuevaReserva = await _reservationService.CrearReservaAsync(reserva);
        return CreatedAtAction(nameof(ListarPorEvento), new { eventoId = nuevaReserva.EventoId }, nuevaReserva);
    }

    [HttpPut("confirmar/{id}")]
    public async Task<IActionResult> ConfirmarPago(int id)
    {
        var reserva = await _reservationService.ConfirmarPagoAsync(id);
        return Ok(reserva);
    }

    [HttpPut("cancelar/{id}")]
    public async Task<IActionResult> CancelarReserva(int id)
    {
        var reserva = await _reservationService.CancelarReservaAsync(id);
        return Ok(reserva);
    }

    [HttpGet("evento/{eventoId}")]
    public async Task<IActionResult> ListarPorEvento(int eventoId)
    {
        var reservas = await _reservationService.ObtenerReservasPorEventoAsync(eventoId);
        return Ok(reservas);
    }
}
