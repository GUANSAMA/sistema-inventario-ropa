namespace InventarioRopa.Entities;

public sealed class Categoria
{
    public int IdCategoria { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
