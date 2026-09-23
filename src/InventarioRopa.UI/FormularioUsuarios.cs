using System.Drawing;
using InventarioRopa.BLL;
using InventarioRopa.Entities;

namespace InventarioRopa.UI;

public sealed class FormularioUsuarios : Form
{
    private readonly UsuarioSistema _solicitante;
    private readonly AutenticacionServicio _servicio = new();
    private readonly DataGridView _grilla = new();
    private readonly TextBox _nombreUsuario = new() { PlaceholderText = "Nombre de usuario", Width = 170 };
    private readonly TextBox _nombreCompleto = new() { PlaceholderText = "Nombre completo", Width = 210 };
    private readonly TextBox _clave = new() { PlaceholderText = "Contraseña inicial", UseSystemPasswordChar = true, Width = 170 };
    private readonly ComboBox _rol = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };

    public FormularioUsuarios(UsuarioSistema solicitante)
    {
        _solicitante = solicitante;
        Text = "Gestión de usuarios"; StartPosition = FormStartPosition.CenterParent; Size = new Size(1000, 650);
        ConstruirInterfaz();
        Load += (_, _) => Cargar();
    }

    private void ConstruirInterfaz()
    {
        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(6), WrapContents = false };
        _rol.Items.AddRange(["Administrador", "Operador de Bodega"]); _rol.SelectedIndex = 1;
        var agregar = new Button { Text = "Crear usuario", AutoSize = true };
        agregar.Click += (_, _) => Crear();
        var desactivar = new Button { Text = "Desactivar seleccionado", AutoSize = true };
        desactivar.Click += (_, _) => Desactivar();
        barra.Controls.AddRange([_nombreUsuario, _nombreCompleto, _clave, _rol, agregar, desactivar]);
        _grilla.Dock = DockStyle.Fill; _grilla.ReadOnly = true; _grilla.AllowUserToAddRows = false;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        Controls.Add(_grilla); Controls.Add(barra);
    }

    private void Cargar()
    {
        try { _grilla.DataSource = _servicio.ListarUsuarios();
            if (_grilla.Columns[nameof(UsuarioSistema.PasswordHash)] is { } hashColumn) hashColumn.Visible = false;
            if (_grilla.Columns[nameof(UsuarioSistema.PasswordSalt)] is { } saltColumn) saltColumn.Visible = false; }
        catch { MessageBox.Show("No se pudo cargar la lista de usuarios.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void Crear()
    {
        try
        {
            _servicio.RegistrarUsuario(_nombreUsuario.Text, _nombreCompleto.Text, _clave.Text, _rol.SelectedItem?.ToString() ?? "", _solicitante);
            _nombreUsuario.Clear(); _nombreCompleto.Clear(); _clave.Clear();
            Cargar();
            MessageBox.Show("Usuario creado correctamente.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch { MessageBox.Show("No se pudo crear el usuario. Compruebe que el nombre no esté repetido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }

    private void Desactivar()
    {
        if (_grilla.CurrentRow?.DataBoundItem is not UsuarioSistema usuario)
        {
            MessageBox.Show("Seleccione un usuario de la lista.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"¿Desactivar la cuenta {usuario.NombreUsuario}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try { _servicio.DesactivarUsuario(usuario.IdUsuario, _solicitante); Cargar(); }
        catch (ArgumentException ex) { MessageBox.Show(ex.Message, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        catch { MessageBox.Show("No se pudo desactivar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
