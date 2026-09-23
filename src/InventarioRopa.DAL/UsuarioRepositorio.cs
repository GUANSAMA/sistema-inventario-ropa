using System.Data;
using InventarioRopa.Entities;
using Microsoft.Data.SqlClient;

namespace InventarioRopa.DAL;

public sealed class UsuarioRepositorio
{
    public UsuarioSistema? ObtenerPorNombre(string nombreUsuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Usuario_ObtenerPorNombre", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        comando.Parameters.Add("@NombreUsuario", SqlDbType.VarChar, 50).Value = nombreUsuario;
        conexion.Open();
        using SqlDataReader lector = comando.ExecuteReader();
        return lector.Read() ? Mapear(lector, incluirCredenciales: true) : null;
    }

    public List<UsuarioSistema> Listar()
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Usuario_Listar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        conexion.Open();
        using SqlDataReader lector = comando.ExecuteReader();
        var usuarios = new List<UsuarioSistema>();
        while (lector.Read()) usuarios.Add(Mapear(lector, incluirCredenciales: false));
        return usuarios;
    }

    public bool RequiereAdministradorInicial()
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Usuario_RequiereAdministradorInicial", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        conexion.Open();
        return Convert.ToBoolean(comando.ExecuteScalar());
    }

    public void CrearAdministradorInicial(UsuarioSistema usuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Usuario_CrearAdministradorInicial", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarDatos(comando, usuario);
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    public void Registrar(UsuarioSistema usuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Usuario_Registrar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        AgregarDatos(comando, usuario);
        comando.Parameters.Add("@Rol", SqlDbType.VarChar, 20).Value = usuario.Rol;
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    public void Desactivar(int idUsuario)
    {
        using SqlConnection conexion = Conexion.Crear();
        using SqlCommand comando = new("sp_Usuario_Desactivar", conexion)
        {
            CommandType = CommandType.StoredProcedure
        };
        comando.Parameters.Add("@IdUsuario", SqlDbType.Int).Value = idUsuario;
        conexion.Open();
        comando.ExecuteNonQuery();
    }

    private static void AgregarDatos(SqlCommand comando, UsuarioSistema usuario)
    {
        comando.Parameters.Add("@NombreUsuario", SqlDbType.VarChar, 50).Value = usuario.NombreUsuario;
        comando.Parameters.Add("@NombreCompleto", SqlDbType.VarChar, 100).Value = usuario.NombreCompleto;
        comando.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, 64).Value = usuario.PasswordHash;
        comando.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, 32).Value = usuario.PasswordSalt;
    }

    private static UsuarioSistema Mapear(SqlDataReader lector, bool incluirCredenciales)
    {
        var usuario = new UsuarioSistema
        {
            IdUsuario = lector.GetInt32(lector.GetOrdinal("IdUsuario")),
            NombreUsuario = lector.GetString(lector.GetOrdinal("NombreUsuario")),
            NombreCompleto = lector.GetString(lector.GetOrdinal("NombreCompleto")),
            Rol = lector.GetString(lector.GetOrdinal("Rol")),
            Activo = lector.GetBoolean(lector.GetOrdinal("Activo"))
        };
        if (incluirCredenciales)
        {
            usuario.PasswordHash = (byte[])lector["PasswordHash"];
            usuario.PasswordSalt = (byte[])lector["PasswordSalt"];
        }
        return usuario;
    }
}
