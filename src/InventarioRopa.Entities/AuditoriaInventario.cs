namespace InventarioRopa.Entities;

public sealed class AuditoriaInventario
{
    public long IdAuditoria { get; set; }
    public int IdPrenda { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string PrendaNombre { get; set; } = string.Empty;
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
}
