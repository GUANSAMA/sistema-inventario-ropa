using System.Security.Cryptography;
using InventarioRopa.DAL;
using InventarioRopa.Entities;

namespace InventarioRopa.BLL;

public sealed class AutenticacionServicio
{
    private const int IteracionesPbkdf2 = 210_000;
    private const int BytesHash = 32;
    private const int BytesSalt = 16;
    private readonly UsuarioRepositorio _usuarios;

    public AutenticacionServicio() : this(new UsuarioRepositorio()) { }

    public AutenticacionServicio(UsuarioRepositorio usuarios) => _usuarios = usuarios;

    public bool RequiereAdministradorInicial() => _usuarios.RequiereAdministradorInicial();

    public List<UsuarioSistema> ListarUsuarios() => _usuarios.Listar();

    public void RegistrarUsuario(string nombreUsuario, string nombreCompleto, string clave, string rol, UsuarioSistema solicitante)
    {
        ExigirAdministrador(solicitante);
        ValidarUsuario(nombreUsuario, nombreCompleto, clave);
        if (rol is not ("Administrador" or "Operador de Bodega"))
            throw new ArgumentException("Seleccione un rol válido.");
        byte[] salt = RandomNumberGenerator.GetBytes(BytesSalt);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(clave, salt, IteracionesPbkdf2, HashAlgorithmName.SHA256, BytesHash);
        _usuarios.Registrar(new UsuarioSistema
        {
            NombreUsuario = nombreUsuario.Trim(), NombreCompleto = nombreCompleto.Trim(),
            PasswordHash = hash, PasswordSalt = salt, Rol = rol, Activo = true
        });
    }

    public void DesactivarUsuario(int idUsuario, UsuarioSistema solicitante)
    {
        ExigirAdministrador(solicitante);
        if (idUsuario <= 0) throw new ArgumentException("Seleccione un usuario válido.");
        if (idUsuario == solicitante.IdUsuario) throw new ArgumentException("No puede desactivar su propia cuenta mientras está conectado.");
        var usuarios = _usuarios.Listar();
        var objetivo = usuarios.Find(u => u.IdUsuario == idUsuario && u.Activo);
        if (objetivo is null) throw new ArgumentException("El usuario ya está inactivo o no existe.");
        if (objetivo.Rol == "Administrador" && usuarios.Count(u => u.Activo && u.Rol == "Administrador") <= 1)
            throw new ArgumentException("Debe quedar al menos un Administrador activo.");
        _usuarios.Desactivar(idUsuario);
    }

    private static void ExigirAdministrador(UsuarioSistema usuario)
    {
        if (usuario.Rol != "Administrador") throw new UnauthorizedAccessException("Esta acción requiere permisos de Administrador.");
    }

    public UsuarioSistema IniciarSesion(string nombreUsuario, string clave)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrEmpty(clave))
            throw new ArgumentException("Escriba su usuario y contraseña.");

        UsuarioSistema? usuario = _usuarios.ObtenerPorNombre(nombreUsuario.Trim());
        if (usuario is null || !usuario.Activo || usuario.PasswordSalt.Length != BytesSalt)
            throw new ArgumentException("Usuario o contraseña incorrectos.");

        byte[] calculado = Rfc2898DeriveBytes.Pbkdf2(clave, usuario.PasswordSalt, IteracionesPbkdf2, HashAlgorithmName.SHA256, BytesHash);
        if (usuario.PasswordHash.Length != calculado.Length || !CryptographicOperations.FixedTimeEquals(calculado, usuario.PasswordHash))
            throw new ArgumentException("Usuario o contraseña incorrectos.");

        usuario.PasswordHash = [];
        usuario.PasswordSalt = [];
        return usuario;
    }

    public void CrearAdministradorInicial(string nombreUsuario, string nombreCompleto, string clave)
    {
        ValidarUsuario(nombreUsuario, nombreCompleto, clave);
        byte[] salt = RandomNumberGenerator.GetBytes(BytesSalt);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(clave, salt, IteracionesPbkdf2, HashAlgorithmName.SHA256, BytesHash);
        _usuarios.CrearAdministradorInicial(new UsuarioSistema
        {
            NombreUsuario = nombreUsuario.Trim(),
            NombreCompleto = nombreCompleto.Trim(),
            PasswordHash = hash,
            PasswordSalt = salt,
            Rol = "Administrador",
            Activo = true
        });
    }

    private static void ValidarUsuario(string nombreUsuario, string nombreCompleto, string clave)
    {
        if (string.IsNullOrWhiteSpace(nombreUsuario) || nombreUsuario.Trim().Length > 50)
            throw new ArgumentException("El nombre de usuario es obligatorio y admite hasta 50 caracteres.");
        if (string.IsNullOrWhiteSpace(nombreCompleto) || nombreCompleto.Trim().Length > 100)
            throw new ArgumentException("El nombre completo es obligatorio y admite hasta 100 caracteres.");
        if (clave.Length < 10)
            throw new ArgumentException("La contraseña debe tener al menos 10 caracteres.");
    }
}
