using System.Data;
using InventarioRopa.Entities;
using Microsoft.Data.SqlClient;
using ColorCatalogo = InventarioRopa.Entities.Color;

namespace InventarioRopa.DAL;

public sealed class CatalogoRepositorio
{
    public List<Categoria> ListarCategorias() => Consultar<Categoria>("sp_Categoria_Listar", lector => new Categoria
    {
        IdCategoria = lector.GetInt32(lector.GetOrdinal("IdCategoria")),
        Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
        Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
    });

    public List<Talla> ListarTallas() => Consultar<Talla>("sp_Talla_Listar", lector => new Talla
    {
        IdTalla = lector.GetInt32(lector.GetOrdinal("IdTalla")),
        Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
        Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
    });

    public List<ColorCatalogo> ListarColores() => Consultar<ColorCatalogo>("sp_Color_Listar", lector => new ColorCatalogo
    {
        IdColor = lector.GetInt32(lector.GetOrdinal("IdColor")),
        Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
        Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
    });

    private static List<T> Consultar<T>(string procedimiento, Func<SqlDataReader, T> mapear)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new(procedimiento, conexion) { CommandType = CommandType.StoredProcedure };
        conexion.Open();
        using SqlDataReader lector = comando.ExecuteReader();
        var resultados = new List<T>();
        while (lector.Read()) resultados.Add(mapear(lector));
        return resultados;
    }
}
