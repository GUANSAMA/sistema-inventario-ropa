namespace InventarioRopa.Entities;

public sealed class Talla
{
    public int IdTalla { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
