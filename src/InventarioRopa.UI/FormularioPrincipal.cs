using System.Drawing;
using InventarioRopa.Entities;

namespace InventarioRopa.UI;

public partial class FormularioPrincipal : Form
{
    private readonly UsuarioSistema _usuario;

    public FormularioPrincipal(UsuarioSistema usuario)
    {
        InitializeComponent();
        _usuario = usuario;
        Text = "Sistema de Inventario de Ropa";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(900, 600);
        ConstruirMenu();
        var bienvenida = new Label
        {
            Text = $"Bienvenido/a, {_usuario.NombreCompleto} ({_usuario.Rol})\n\nSeleccione una opción del menú para comenzar.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(Font.FontFamily, 16)
        };
        Controls.Add(bienvenida);
    }

    private void ConstruirMenu()
    {
        var menu = new MenuStrip();
        var inventario = new ToolStripMenuItem("Inventario");
        inventario.Click += (_, _) => new FormularioPrendas(_usuario).ShowDialog(this);
        menu.Items.Add(inventario);

        if (_usuario.Rol == "Administrador")
        {
            var auditoria = new ToolStripMenuItem("Auditoría");
            auditoria.Click += (_, _) => new FormularioAuditoria(_usuario).ShowDialog(this);
            menu.Items.Add(auditoria);
            var usuarios = new ToolStripMenuItem("Usuarios");
            usuarios.Click += (_, _) => new FormularioUsuarios(_usuario).ShowDialog(this);
            menu.Items.Add(usuarios);
            var reportes = new ToolStripMenuItem("Reportes PDF");
            reportes.Click += (_, _) => new FormularioReportes(_usuario).ShowDialog(this);
            menu.Items.Add(reportes);
        }
        menu.Items.Add(new ToolStripMenuItem("Cerrar sesión", null, (_, _) => Close()));
        MainMenuStrip = menu;
        Controls.Add(menu);
    }
}
