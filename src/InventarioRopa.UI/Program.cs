using InventarioRopa.BLL;

namespace InventarioRopa.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        try
        {
            var autenticacion = new AutenticacionServicio();
            var usuarios = autenticacion.ListarUsuarios();
            var usuario = usuarios.FirstOrDefault(u => u.Activo && u.Rol == "Administrador")
                ?? usuarios.FirstOrDefault(u => u.Activo);

            if (usuario is null && autenticacion.RequiereAdministradorInicial())
            {
                using var configuracion = new LoginForm();
                if (configuracion.ShowDialog() != DialogResult.OK || configuracion.UsuarioAutenticado is null)
                    return;
                usuario = configuracion.UsuarioAutenticado;
            }

            if (usuario is null)
            {
                MessageBox.Show(
                    "No hay usuarios activos en la base de datos. Active un usuario para registrar las operaciones de inventario.",
                    "Sistema de Inventario de Ropa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Application.Run(new FormularioPrincipal(usuario));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo abrir el inventario. Compruebe la conexión con InventarioRopaDB.\n\nDetalle: {ex.Message}",
                "Error de conexión",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
