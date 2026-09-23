using InventarioRopa.DAL;
using InventarioRopa.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventarioRopa.BLL;

public sealed class ReporteServicio
{
    private readonly PrendaRepositorio _prendas = new();
    private readonly AuditoriaRepositorio _auditoria = new();

    public void GenerarInventario(string ruta, UsuarioSistema usuario)
    {
        ExigirAdministrador(usuario);
        QuestPDF.Settings.License = LicenseType.Community;
        var filas = _prendas.Listar();
        Documento("Inventario de prendas", container =>
        {
            container.Table(tabla =>
            {
                tabla.ColumnsDefinition(col =>
                {
                    col.ConstantColumn(90); col.RelativeColumn(3); col.RelativeColumn(2);
                    col.ConstantColumn(55); col.ConstantColumn(75); col.ConstantColumn(50);
                });
                EncabezadoTabla(tabla, ["SKU", "Prenda", "Categoría / Talla / Color", "Precio", "Stock / Mín.", "Estado"]);
                foreach (Prenda p in filas)
                {
                    Celda(tabla, p.CodigoSKU);
                    Celda(tabla, p.Nombre);
                    Celda(tabla, $"{p.CategoriaNombre} / {p.TallaNombre} / {p.ColorNombre}");
                    Celda(tabla, p.Precio.ToString("N0"));
                    Celda(tabla, $"{p.Stock} / {p.StockMinimo}");
                    Celda(tabla, p.Activo ? "Activa" : "Inactiva");
                }
            });
        }).GeneratePdf(ruta);
    }

    public void GenerarAuditoria(string ruta, UsuarioSistema usuario)
    {
        ExigirAdministrador(usuario);
        QuestPDF.Settings.License = LicenseType.Community;
        var filas = _auditoria.Listar();
        Documento("Historial de auditoría", container =>
        {
            container.Table(tabla =>
            {
                tabla.ColumnsDefinition(col =>
                {
                    col.ConstantColumn(48); col.ConstantColumn(68); col.RelativeColumn(2);
                    col.ConstantColumn(78); col.ConstantColumn(105); col.RelativeColumn(2);
                });
                EncabezadoTabla(tabla, ["ID", "Operación", "Prenda", "Usuario", "Fecha y hora", "Valores anteriores → nuevos"]);
                foreach (AuditoriaInventario a in filas)
                {
                    Celda(tabla, a.IdAuditoria.ToString());
                    Celda(tabla, a.Operacion);
                    Celda(tabla, $"{a.CodigoSKU} - {a.PrendaNombre}");
                    Celda(tabla, a.NombreUsuario);
                    Celda(tabla, a.FechaHora.ToString("dd/MM/yyyy HH:mm:ss"));
                    Celda(tabla, $"{a.ValoresAnteriores ?? "—"} → {a.ValoresNuevos ?? "—"}", 7);
                }
            });
        }).GeneratePdf(ruta);
    }

    private static void ExigirAdministrador(UsuarioSistema usuario)
    {
        if (!usuario.Activo || usuario.Rol != "Administrador")
            throw new UnauthorizedAccessException("Solo el Administrador puede generar reportes.");
    }

    private static IDocument Documento(string titulo, Action<ColumnDescriptor> contenido) =>
        Document.Create(documento => documento.Page(pagina =>
        {
            pagina.Size(PageSizes.A4.Landscape());
            pagina.Margin(1.4f, Unit.Centimetre);
            pagina.DefaultTextStyle(estilo => estilo.FontSize(8));
            pagina.Header().Column(col =>
            {
                col.Item().Text("TIENDA MODA URBANA").FontSize(10).Bold().FontColor(Colors.Blue.Darken2);
                col.Item().Text(titulo).FontSize(16).Bold();
                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}").FontSize(8).FontColor(Colors.Grey.Darken2);
            });
            pagina.Content().PaddingVertical(12).Column(contenido);
            pagina.Footer().AlignRight().Text(texto =>
            {
                texto.Span("Página "); texto.CurrentPageNumber(); texto.Span(" de "); texto.TotalPages();
            });
        }));

    private static void EncabezadoTabla(TableDescriptor tabla, IEnumerable<string> titulos)
    {
        tabla.Header(encabezado =>
        {
            foreach (string titulo in titulos)
                encabezado.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(titulo).Bold();
        });
    }

    private static void Celda(TableDescriptor tabla, string texto, float tamano = 8)
    {
        tabla.Cell().BorderBottom(0.4f).BorderColor(Colors.Grey.Lighten1).Padding(4).Text(texto).FontSize(tamano);
    }
}
