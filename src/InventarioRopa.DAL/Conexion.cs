using Microsoft.Data.SqlClient;

namespace InventarioRopa.DAL;

public static class Conexion
{
    private const string VariableConexion = "INVENTARIO_ROPA_SQLSERVER";
    private const string ConexionDesarrollo =
        @"Server=.\SQLEXPRESS;Database=InventarioRopaDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

    public static SqlConnection Crear()
    {
        string cadena = Environment.GetEnvironmentVariable(VariableConexion) ?? ConexionDesarrollo;
        return new SqlConnection(cadena);
    }
}
