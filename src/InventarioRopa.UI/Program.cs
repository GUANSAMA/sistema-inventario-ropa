namespace InventarioRopa.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        using var acceso = new LoginForm();
        if (acceso.ShowDialog() != DialogResult.OK || acceso.UsuarioAutenticado is null) return;
        Application.Run(new FormularioPrincipal(acceso.UsuarioAutenticado));
    }
}
