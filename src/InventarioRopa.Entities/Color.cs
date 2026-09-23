namespace InventarioRopa.Entities;

public sealed class Color
{
    public int IdColor { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
