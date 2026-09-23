using System.Diagnostics;
using System.Drawing;
using InventarioRopa.BLL;
using InventarioRopa.Entities;

namespace InventarioRopa.UI;

public sealed class FormularioReportes : Form
{
    private readonly ReporteServicio _reportes = new();
    private readonly UsuarioSistema _usuario;

    public FormularioReportes(UsuarioSistema usuario)
    {
        _usuario = usuario;
        Text = "Reportes PDF"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false; ClientSize = new Size(430, 200);
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(24) };
        panel.Controls.Add(new Label { Text = "Seleccione el reporte que desea generar:", AutoSize = true, Font = new Font(Font.FontFamily, 12) });
        var inventario = new Button { Text = "Reporte PDF de inventario", AutoSize = true, Height = 36 };
        var auditoria = new Button { Text = "Reporte PDF de auditoría", AutoSize = true, Height = 36 };
        inventario.Click += (_, _) => Generar(true);
        auditoria.Click += (_, _) => Generar(false);
        panel.Controls.Add(inventario); panel.Controls.Add(auditoria);
        Controls.Add(panel);
    }

    private void Generar(bool inventario)
    {
        using var dialogo = new SaveFileDialog
        {
            Filter = "Documento PDF (*.pdf)|*.pdf",
            Title = "Guardar reporte PDF",
            FileName = inventario ? $"Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf" : $"Auditoria_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
        };
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            if (inventario) _reportes.GenerarInventario(dialogo.FileName, _usuario);
            else _reportes.GenerarAuditoria(dialogo.FileName, _usuario);
            if (MessageBox.Show("Reporte generado. ¿Desea abrirlo?", "Reporte PDF", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                Process.Start(new ProcessStartInfo(dialogo.FileName) { UseShellExecute = true });
        }
        catch
        {
            MessageBox.Show("No se pudo generar el PDF. Revise la ruta elegida y la conexión a la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
