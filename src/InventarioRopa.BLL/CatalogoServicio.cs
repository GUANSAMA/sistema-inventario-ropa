using InventarioRopa.DAL;
using InventarioRopa.Entities;
using ColorCatalogo = InventarioRopa.Entities.Color;

namespace InventarioRopa.BLL;

public sealed class CatalogoServicio
{
    private readonly CatalogoRepositorio _repositorio = new();
    public List<Categoria> ListarCategorias() => _repositorio.ListarCategorias();
    public List<Talla> ListarTallas() => _repositorio.ListarTallas();
    public List<ColorCatalogo> ListarColores() => _repositorio.ListarColores();
}
