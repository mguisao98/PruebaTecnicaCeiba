using Domain.Enums;

namespace Domain.Entities;

public class Reserva
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public Evento? Evento { get; set; }
    public int Cantidad { get; set; }
    public string NombreComprador { get; set; } = string.Empty;
    public string EmailComprador { get; set; } = string.Empty;
    public EstadoReserva Estado { get; set; } = EstadoReserva.PendientePago;
    public string? CodigoReserva { get; set; }
    public DateTime? FechaCancelacion { get; set; }
    public DateTime? FechaPago { get; set; }
}
