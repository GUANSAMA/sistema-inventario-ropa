namespace InventarioRopa.Entities;

public sealed class UsuarioSistema
{
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
}
