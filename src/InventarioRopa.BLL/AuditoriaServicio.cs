using InventarioRopa.DAL;
using InventarioRopa.Entities;

namespace InventarioRopa.BLL;

public sealed class AuditoriaServicio
{
    private readonly AuditoriaRepositorio _repositorio = new();
    public List<AuditoriaInventario> Listar(UsuarioSistema usuario, DateTime? desde = null, DateTime? hasta = null)
    {
        if (usuario.Rol != "Administrador" || !usuario.Activo)
            throw new UnauthorizedAccessException("Solo el Administrador puede consultar la auditoría.");
        return _repositorio.Listar(desde, hasta);
    }
}
