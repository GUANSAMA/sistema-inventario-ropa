using System.Drawing;
using InventarioRopa.BLL;
using InventarioRopa.Entities;

namespace InventarioRopa.UI;

public sealed class FormularioAuditoria : Form
{
    private readonly AuditoriaServicio _servicio = new();
    private readonly UsuarioSistema _usuario;
    private readonly DataGridView _grilla = new();
    private readonly DateTimePicker _desde = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Checked = false };
    private readonly DateTimePicker _hasta = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Checked = false };

    public FormularioAuditoria(UsuarioSistema usuario)
    {
        _usuario = usuario;
        Text = "Historial de auditoría"; StartPosition = FormStartPosition.CenterParent; Size = new Size(1100, 700);
        var barra = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42, Padding = new Padding(8) };
        barra.Controls.Add(new Label { Text = "Desde", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
        barra.Controls.Add(_desde);
        barra.Controls.Add(new Label { Text = "Hasta", AutoSize = true, Padding = new Padding(8, 8, 0, 0) });
        barra.Controls.Add(_hasta);
        var actualizar = new Button { Text = "Consultar", AutoSize = true };
        actualizar.Click += (_, _) => Cargar(); barra.Controls.Add(actualizar);
        _grilla.Dock = DockStyle.Fill; _grilla.ReadOnly = true; _grilla.AllowUserToAddRows = false;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        Controls.Add(_grilla); Controls.Add(barra);
        Load += (_, _) => Cargar();
    }

    private void Cargar()
    {
        try
        {
            DateTime? desde = _desde.Checked ? _desde.Value.Date : null;
            DateTime? hasta = _hasta.Checked ? _hasta.Value.Date.AddDays(1).AddTicks(-1) : null;
            _grilla.DataSource = _servicio.Listar(_usuario, desde, hasta);
        }
        catch
        {
            MessageBox.Show("No se pudo consultar la auditoría. Revise la conexión y sus permisos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
