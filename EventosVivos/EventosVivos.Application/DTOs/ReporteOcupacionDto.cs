namespace Application.DTOs;

public class ReporteOcupacionDto
{
    public int EventoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int CapacidadMaxima { get; set; }
    public int EntradasVendidas { get; set; }
    public double Ocupacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Venue { get; set; } = string.Empty;
    public int EntradasDisponibles { get; set; }   // ← NUEVO
    public decimal Ingresos { get; set; }          // ← NUEVO
}
