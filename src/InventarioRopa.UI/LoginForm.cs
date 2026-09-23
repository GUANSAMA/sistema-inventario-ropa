using System.Drawing;
using InventarioRopa.BLL;
using InventarioRopa.Entities;

namespace InventarioRopa.UI;

public sealed class LoginForm : Form
{
    private readonly AutenticacionServicio _autenticacion = new();
    private readonly TextBox _usuario = new();
    private readonly TextBox _nombreCompleto = new();
    private readonly TextBox _clave = new();
    private readonly TextBox _confirmar = new();
    private readonly Label _etiquetaNombre = new() { Text = "Nombre completo" };
    private readonly Label _etiquetaConfirmar = new() { Text = "Confirmar contraseña" };
    private readonly Label _mensaje = new() { AutoSize = true, ForeColor = Color.Firebrick };
    private readonly Button _aceptar = new() { Text = "Iniciar sesión", AutoSize = true };
    private bool _altaInicial;

    public UsuarioSistema? UsuarioAutenticado { get; private set; }

    public LoginForm()
    {
        Text = "Acceso - Inventario de Ropa";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(430, 350);
        ConstruirInterfaz();
        Shown += RevisarPrimeraEjecucion;
        _aceptar.Click += Procesar;
        AcceptButton = _aceptar;
    }

    private void ConstruirInterfaz()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 2, RowCount = 7 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        for (int i = 0; i < 6; i++) layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var encabezado = new Label { Text = "Sistema de Inventario de Ropa", Font = new Font(Font.FontFamily, 15, FontStyle.Bold), AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
        layout.Controls.Add(encabezado, 0, 0); layout.SetColumnSpan(encabezado, 2);
        _usuario.PlaceholderText = "Usuario";
        _nombreCompleto.PlaceholderText = "Nombre y apellido";
        _clave.UseSystemPasswordChar = true;
        _confirmar.UseSystemPasswordChar = true;
        AgregarFila(layout, 1, "Usuario", _usuario);
        AgregarFila(layout, 2, _etiquetaNombre.Text, _nombreCompleto);
        AgregarFila(layout, 3, "Contraseña", _clave);
        AgregarFila(layout, 4, _etiquetaConfirmar.Text, _confirmar);
        _mensaje.Dock = DockStyle.Fill;
        layout.Controls.Add(_mensaje, 0, 5); layout.SetColumnSpan(_mensaje, 2);
        _aceptar.Anchor = AnchorStyles.Right;
        layout.Controls.Add(_aceptar, 1, 6);
        Controls.Add(layout);
        CambiarAModoLogin();
    }

    private static void AgregarFila(TableLayoutPanel layout, int fila, string texto, Control control)
    {
        var etiqueta = new Label { Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        control.Dock = DockStyle.Fill;
        layout.Controls.Add(etiqueta, 0, fila);
        layout.Controls.Add(control, 1, fila);
    }

    private void RevisarPrimeraEjecucion(object? sender, EventArgs e)
    {
        try
        {
            if (_autenticacion.RequiereAdministradorInicial())
            {
                _altaInicial = true;
                Text = "Configuración inicial";
                _aceptar.Text = "Crear Administrador";
                _etiquetaNombre.Visible = _nombreCompleto.Visible = true;
                _etiquetaConfirmar.Visible = _confirmar.Visible = true;
                _mensaje.Text = "Cree la cuenta inicial de Administrador para comenzar.";
            }
        }
        catch
        {
            _mensaje.Text = "No se pudo conectar a InventarioRopaDB. Revise la instancia y ejecute los scripts de database.";
            _aceptar.Enabled = false;
        }
    }

    private void CambiarAModoLogin()
    {
        _etiquetaNombre.Visible = _nombreCompleto.Visible = false;
        _etiquetaConfirmar.Visible = _confirmar.Visible = false;
    }

    private void Procesar(object? sender, EventArgs e)
    {
        try
        {
            if (_altaInicial)
            {
                if (_clave.Text != _confirmar.Text) throw new ArgumentException("Las contraseñas no coinciden.");
                _autenticacion.CrearAdministradorInicial(_usuario.Text, _nombreCompleto.Text, _clave.Text);
                _mensaje.ForeColor = Color.DarkGreen;
                _mensaje.Text = "Administrador creado. Inicie sesión con sus datos.";
                _altaInicial = false;
                CambiarAModoLogin();
                _aceptar.Text = "Iniciar sesión";
                _clave.Clear(); _confirmar.Clear();
                return;
            }

            UsuarioAutenticado = _autenticacion.IniciarSesion(_usuario.Text, _clave.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (ArgumentException ex)
        {
            _mensaje.ForeColor = Color.Firebrick;
            _mensaje.Text = ex.Message;
        }
        catch
        {
            _mensaje.ForeColor = Color.Firebrick;
            _mensaje.Text = "No se pudo completar la operación. Revise la conexión o los datos ingresados.";
        }
    }
}
