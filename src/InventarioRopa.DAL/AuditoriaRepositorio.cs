using System.Data;
using InventarioRopa.Entities;
using Microsoft.Data.SqlClient;

namespace InventarioRopa.DAL;

public sealed class AuditoriaRepositorio
{
    public List<AuditoriaInventario> Listar(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Auditoria_Listar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        comando.Parameters.Add("@FechaInicio", SqlDbType.DateTime2).Value = (object?)fechaInicio ?? DBNull.Value;
        comando.Parameters.Add("@FechaFin", SqlDbType.DateTime2).Value = (object?)fechaFin ?? DBNull.Value;
        conexion.Open();
        using SqlDataReader lector = comando.ExecuteReader();
        var filas = new List<AuditoriaInventario>();
        while (lector.Read())
        {
            filas.Add(new AuditoriaInventario
            {
                IdAuditoria = lector.GetInt64(lector.GetOrdinal("IdAuditoria")),
                IdPrenda = lector.GetInt32(lector.GetOrdinal("IdPrenda")),
                CodigoSKU = lector.GetString(lector.GetOrdinal("CodigoSKU")),
                PrendaNombre = lector.GetString(lector.GetOrdinal("PrendaNombre")),
                IdUsuario = lector.GetInt32(lector.GetOrdinal("IdUsuario")),
                NombreUsuario = lector.GetString(lector.GetOrdinal("NombreUsuario")),
                Operacion = lector.GetString(lector.GetOrdinal("Operacion")),
                FechaHora = lector.GetDateTime(lector.GetOrdinal("FechaHora")),
                ValoresAnteriores = lector.IsDBNull(lector.GetOrdinal("ValoresAnteriores")) ? null : lector.GetString(lector.GetOrdinal("ValoresAnteriores")),
                ValoresNuevos = lector.IsDBNull(lector.GetOrdinal("ValoresNuevos")) ? null : lector.GetString(lector.GetOrdinal("ValoresNuevos"))
            });
        }
        return filas;
    }
}
