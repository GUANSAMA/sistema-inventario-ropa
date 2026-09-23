using System.Data;
using InventarioRopa.Entities;
using Microsoft.Data.SqlClient;

namespace InventarioRopa.DAL;

public sealed class PrendaRepositorio
{
    public List<Prenda> Listar()
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Prenda_Listar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        conexion.Open();
        using SqlDataReader lector = comando.ExecuteReader();
        var prendas = new List<Prenda>();
        while (lector.Read()) prendas.Add(Mapear(lector));
        return prendas;
    }

    public List<Prenda> Buscar(string? texto, int? categoriaId, int? tallaId, int? colorId, bool? activo)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Prenda_Buscar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        comando.Parameters.Add("@Texto", SqlDbType.VarChar, 100).Value = (object?)texto ?? DBNull.Value;
        comando.Parameters.Add("@CategoriaId", SqlDbType.Int).Value = (object?)categoriaId ?? DBNull.Value;
        comando.Parameters.Add("@TallaId", SqlDbType.Int).Value = (object?)tallaId ?? DBNull.Value;
        comando.Parameters.Add("@ColorId", SqlDbType.Int).Value = (object?)colorId ?? DBNull.Value;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = (object?)activo ?? DBNull.Value;
        conexion.Open();
        using SqlDataReader lector = comando.ExecuteReader();
        var prendas = new List<Prenda>();
        while (lector.Read()) prendas.Add(Mapear(lector));
        return prendas;
    }

    public int Insertar(Prenda prenda, int idUsuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = CrearComandoEscritura("sp_Prenda_Insertar", conexion, prenda, idUsuario);
        var salida = comando.Parameters.Add("@IdPrendaNueva", SqlDbType.Int);
        salida.Direction = ParameterDirection.Output;
        conexion.Open();
        comando.ExecuteNonQuery();
        return Convert.ToInt32(salida.Value);
    }

    public void Actualizar(Prenda prenda, int idUsuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = CrearComandoEscritura("sp_Prenda_Actualizar", conexion, prenda, idUsuario);
        comando.Parameters.Add("@IdPrenda", SqlDbType.Int).Value = prenda.IdPrenda;
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    public void Desactivar(int idPrenda, int idUsuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Prenda_Desactivar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        comando.Parameters.Add("@IdPrenda", SqlDbType.Int).Value = idPrenda;
        comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    private static SqlCommand CrearComandoEscritura(string nombre, SqlConnection conexion, Prenda prenda, int idUsuario)
    {
        var comando = new SqlCommand(nombre, conexion) { CommandType = CommandType.StoredProcedure };
        comando.Parameters.Add("@CodigoSKU", SqlDbType.VarChar, 20).Value = prenda.CodigoSKU;
        comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = prenda.Nombre;
        comando.Parameters.Add("@CategoriaId", SqlDbType.Int).Value = prenda.CategoriaId;
        comando.Parameters.Add("@TallaId", SqlDbType.Int).Value = prenda.TallaId;
        comando.Parameters.Add("@ColorId", SqlDbType.Int).Value = prenda.ColorId;
        comando.Parameters.Add("@Marca", SqlDbType.VarChar, 50).Value = (object?)prenda.Marca ?? DBNull.Value;
        comando.Parameters.Add("@Precio", SqlDbType.Decimal).Value = prenda.Precio;
        comando.Parameters["@Precio"].Precision = 10;
        comando.Parameters["@Precio"].Scale = 0;
        comando.Parameters.Add("@Stock", SqlDbType.Int).Value = prenda.Stock;
        comando.Parameters.Add("@StockMinimo", SqlDbType.Int).Value = prenda.StockMinimo;
        comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
        return comando;
    }

    private static Prenda Mapear(SqlDataReader lector) => new()
    {
        IdPrenda = lector.GetInt32(lector.GetOrdinal("IdPrenda")),
        CodigoSKU = lector.GetString(lector.GetOrdinal("CodigoSKU")),
        Nombre = lector.GetString(lector.GetOrdinal("Nombre")),
        CategoriaId = lector.GetInt32(lector.GetOrdinal("CategoriaId")),
        CategoriaNombre = lector.GetString(lector.GetOrdinal("CategoriaNombre")),
        TallaId = lector.GetInt32(lector.GetOrdinal("TallaId")),
        TallaNombre = lector.GetString(lector.GetOrdinal("TallaNombre")),
        ColorId = lector.GetInt32(lector.GetOrdinal("ColorId")),
        ColorNombre = lector.GetString(lector.GetOrdinal("ColorNombre")),
        Marca = lector.IsDBNull(lector.GetOrdinal("Marca")) ? null : lector.GetString(lector.GetOrdinal("Marca")),
        Precio = lector.GetDecimal(lector.GetOrdinal("Precio")),
        Stock = lector.GetInt32(lector.GetOrdinal("Stock")),
        StockMinimo = lector.GetInt32(lector.GetOrdinal("StockMinimo")),
        Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
    };
}
