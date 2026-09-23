using InventarioRopa.DAL;
using InventarioRopa.Entities;

namespace InventarioRopa.BLL;

public sealed class PrendaServicio
{
    private readonly PrendaRepositorio _repositorio;

    public PrendaServicio() : this(new PrendaRepositorio()) { }

    public PrendaServicio(PrendaRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public List<Prenda> Listar() => _repositorio.Listar();

    public List<Prenda> Buscar(string? texto, int? categoriaId = null, int? tallaId = null,
        int? colorId = null, bool? activo = null) =>
        _repositorio.Buscar(texto?.Trim(), categoriaId, tallaId, colorId, activo);

    public int Registrar(Prenda prenda, UsuarioSistema usuario)
    {
        Validar(prenda);
        ExigirUsuarioActivo(usuario);
        return _repositorio.Insertar(prenda, usuario.IdUsuario);
    }

    public void Actualizar(Prenda prenda, UsuarioSistema usuario)
    {
        if (prenda.IdPrenda <= 0) throw new ArgumentException("Seleccione una prenda válida.");
        Validar(prenda);
        ExigirUsuarioActivo(usuario);
        _repositorio.Actualizar(prenda, usuario.IdUsuario);
    }

    public void Desactivar(int idPrenda, UsuarioSistema usuario)
    {
        if (idPrenda <= 0) throw new ArgumentException("Seleccione una prenda válida.");
        ExigirUsuarioActivo(usuario);
        if (usuario.Rol != "Administrador") throw new UnauthorizedAccessException("Solo el Administrador puede desactivar prendas.");
        _repositorio.Desactivar(idPrenda, usuario.IdUsuario);
    }

    private static void ExigirUsuarioActivo(UsuarioSistema usuario)
    {
        if (usuario.IdUsuario <= 0 || !usuario.Activo)
            throw new UnauthorizedAccessException("Debe iniciar sesión para realizar esta operación.");
    }

    private static void Validar(Prenda prenda)
    {
        if (string.IsNullOrWhiteSpace(prenda.CodigoSKU)) throw new ArgumentException("El SKU es obligatorio.");
        if (prenda.CodigoSKU.Trim().Length > 20) throw new ArgumentException("El SKU no puede superar 20 caracteres.");
        if (string.IsNullOrWhiteSpace(prenda.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
        if (prenda.Nombre.Trim().Length > 100) throw new ArgumentException("El nombre no puede superar 100 caracteres.");
        if (prenda.CategoriaId <= 0 || prenda.TallaId <= 0 || prenda.ColorId <= 0)
            throw new ArgumentException("Seleccione una categoría, talla y color válidos.");
        if (prenda.Precio <= 0) throw new ArgumentException("El precio debe ser mayor que cero.");
        if (prenda.Stock < 0 || prenda.StockMinimo < 0)
            throw new ArgumentException("El stock y el stock mínimo no pueden ser negativos.");
    }
}
