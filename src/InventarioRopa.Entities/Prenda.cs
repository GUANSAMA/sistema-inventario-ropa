namespace InventarioRopa.Entities;

public sealed class Prenda
{
    public int IdPrenda { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public int TallaId { get; set; }
    public string TallaNombre { get; set; } = string.Empty;
    public int ColorId { get; set; }
    public string ColorNombre { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public bool Activo { get; set; } = true;
}
