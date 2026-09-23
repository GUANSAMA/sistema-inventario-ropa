using System.Globalization;
using System.Drawing;
using InventarioRopa.BLL;
using InventarioRopa.Entities;
using ColorCatalogo = InventarioRopa.Entities.Color;

namespace InventarioRopa.UI;

public sealed class FormularioPrendas : Form
{
    private readonly UsuarioSistema _usuario;
    private readonly PrendaServicio _prendas = new();
    private readonly CatalogoServicio _catalogos = new();
    private readonly DataGridView _grilla = new();
    private readonly TextBox _sku = new();
    private readonly TextBox _nombre = new();
    private readonly TextBox _marca = new();
    private readonly TextBox _precio = new();
    private readonly TextBox _stock = new();
    private readonly TextBox _stockMinimo = new();
    private readonly TextBox _buscar = new();
    private readonly ComboBox _categoria = Lista();
    private readonly ComboBox _talla = Lista();
    private readonly ComboBox _color = Lista();
    private readonly ComboBox _filtroCategoria = Lista();
    private readonly ComboBox _filtroTalla = Lista();
    private readonly ComboBox _filtroColor = Lista();
    private readonly ComboBox _filtroEstado = Lista();
    private int? _idSeleccionado;
    private bool _modoEdicion;

    public FormularioPrendas(UsuarioSistema usuario)
    {
        _usuario = usuario;
        Text = "Mantenedor de prendas";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(1050, 700);
        Size = new Size(1200, 820);
        ConstruirInterfaz();
        Load += (_, _) => CargarDatosIniciales();
    }

    private void ConstruirInterfaz()
    {
        var raiz = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 1, RowCount = 4 };
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 190));
        raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        raiz.Controls.Add(ConstruirFiltros(), 0, 0);
        raiz.Controls.Add(ConstruirCampos(), 0, 1);
        ConfigurarGrilla();
        raiz.Controls.Add(_grilla, 0, 2);
        raiz.Controls.Add(ConstruirBotones(), 0, 3);
        Controls.Add(raiz);
    }

    private Control ConstruirFiltros()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = true };
        _buscar.Width = 210; _buscar.PlaceholderText = "Buscar por SKU o nombre";
        PrepararFiltro(_filtroCategoria, "Todas las categorías");
        PrepararFiltro(_filtroTalla, "Todas las tallas");
        PrepararFiltro(_filtroColor, "Todos los colores");
        _filtroEstado.Items.Add(new OpcionFiltro<bool?>(null, "Todos los estados"));
        _filtroEstado.Items.Add(new OpcionFiltro<bool?>(true, "Activas"));
        _filtroEstado.Items.Add(new OpcionFiltro<bool?>(false, "Inactivas"));
        _filtroEstado.SelectedIndex = 0;
        var buscar = Boton("Buscar", (_, _) => BuscarPrendas());
        panel.Controls.AddRange([_buscar, _filtroCategoria, _filtroTalla, _filtroColor, _filtroEstado, buscar]);
        return panel;
    }

    private Control ConstruirCampos()
    {
        var grupo = new GroupBox { Text = "Datos de la prenda", Dock = DockStyle.Fill, Padding = new Padding(8) };
        var tabla = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 2 };
        for (int i = 0; i < 6; i++) tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.67f));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        AgregarCampo(tabla, 0, 0, "SKU", _sku);
        AgregarCampo(tabla, 1, 0, "Nombre", _nombre);
        AgregarCampo(tabla, 2, 0, "Categoría", _categoria);
        AgregarCampo(tabla, 3, 0, "Talla", _talla);
        AgregarCampo(tabla, 4, 0, "Color", _color);
        AgregarCampo(tabla, 5, 0, "Marca", _marca);
        AgregarCampo(tabla, 0, 1, "Precio (CLP)", _precio);
        AgregarCampo(tabla, 1, 1, "Stock", _stock);
        AgregarCampo(tabla, 2, 1, "Stock mínimo", _stockMinimo);
        grupo.Controls.Add(tabla);
        return grupo;
    }

    private Control ConstruirBotones()
    {
        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        panel.Controls.Add(Boton("Nuevo", (_, _) => Limpiar()));
        panel.Controls.Add(Boton("Guardar", (_, _) => Guardar()));
        panel.Controls.Add(Boton("Editar", (_, _) => PrepararEdicion()));
        if (_usuario.Rol == "Administrador") panel.Controls.Add(Boton("Desactivar", (_, _) => Desactivar()));
        panel.Controls.Add(Boton("Limpiar", (_, _) => Limpiar()));
        return panel;
    }

    private void ConfigurarGrilla()
    {
        _grilla.Dock = DockStyle.Fill;
        _grilla.ReadOnly = true;
        _grilla.MultiSelect = false;
        _grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grilla.AllowUserToAddRows = false;
        _grilla.RowHeadersVisible = false;
        _grilla.CellDoubleClick += (_, _) => PrepararEdicion();
    }

    private void CargarDatosIniciales()
    {
        try
        {
            var categorias = _catalogos.ListarCategorias();
            var tallas = _catalogos.ListarTallas();
            var colores = _catalogos.ListarColores();
            _categoria.DataSource = categorias; _categoria.DisplayMember = nameof(Categoria.Nombre); _categoria.ValueMember = nameof(Categoria.IdCategoria);
            _talla.DataSource = tallas; _talla.DisplayMember = nameof(Talla.Nombre); _talla.ValueMember = nameof(Talla.IdTalla);
            _color.DataSource = colores; _color.DisplayMember = nameof(ColorCatalogo.Nombre); _color.ValueMember = nameof(ColorCatalogo.IdColor);
            CargarFiltros(categorias, tallas, colores);
            BuscarPrendas();
        }
        catch
        {
            MessageBox.Show("No se pudieron cargar los catálogos. Revise la conexión con InventarioRopaDB.", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }

    private void CargarFiltros(List<Categoria> categorias, List<Talla> tallas, List<ColorCatalogo> colores)
    {
        CargarFiltro(_filtroCategoria, categorias.Select(x => new OpcionFiltro<int?>(x.IdCategoria, x.Nombre)));
        CargarFiltro(_filtroTalla, tallas.Select(x => new OpcionFiltro<int?>(x.IdTalla, x.Nombre)));
        CargarFiltro(_filtroColor, colores.Select(x => new OpcionFiltro<int?>(x.IdColor, x.Nombre)));
    }

    private static void CargarFiltro(ComboBox combo, IEnumerable<OpcionFiltro<int?>> opciones)
    {
        combo.Items.Clear(); combo.Items.Add(new OpcionFiltro<int?>(null, "Todos"));
        foreach (var opcion in opciones) combo.Items.Add(opcion);
        combo.SelectedIndex = 0;
    }

    private static void PrepararFiltro(ComboBox combo, string texto)
    {
        combo.Width = 145;
        combo.Items.Add(new OpcionFiltro<int?>(null, texto));
        combo.SelectedIndex = 0;
    }

    private void BuscarPrendas()
    {
        try
        {
            int? categoria = (_filtroCategoria.SelectedItem as OpcionFiltro<int?>)?.Valor;
            int? talla = (_filtroTalla.SelectedItem as OpcionFiltro<int?>)?.Valor;
            int? color = (_filtroColor.SelectedItem as OpcionFiltro<int?>)?.Valor;
            bool? estado = (_filtroEstado.SelectedItem as OpcionFiltro<bool?>)?.Valor;
            _grilla.DataSource = _prendas.Buscar(_buscar.Text, categoria, talla, color, estado);
            foreach (string columna in new[] { nameof(Prenda.IdPrenda), nameof(Prenda.CategoriaId), nameof(Prenda.TallaId), nameof(Prenda.ColorId) })
                if (_grilla.Columns[columna] is { } columnaOculta) columnaOculta.Visible = false;
            _grilla.Columns[nameof(Prenda.CodigoSKU)]!.HeaderText = "SKU";
            _grilla.Columns[nameof(Prenda.CategoriaNombre)]!.HeaderText = "Categoría";
            _grilla.Columns[nameof(Prenda.TallaNombre)]!.HeaderText = "Talla";
            _grilla.Columns[nameof(Prenda.ColorNombre)]!.HeaderText = "Color";
            _grilla.Columns[nameof(Prenda.StockMinimo)]!.HeaderText = "Stock mínimo";
            _grilla.Columns[nameof(Prenda.Activo)]!.HeaderText = "Activa";
        }
        catch
        {
            MessageBox.Show("No se pudo consultar el inventario. Revise la conexión con la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Guardar()
    {
        try
        {
            var prenda = LeerFormulario();
            if (_modoEdicion && _idSeleccionado.HasValue)
            {
                prenda.IdPrenda = _idSeleccionado.Value;
                _prendas.Actualizar(prenda, _usuario);
            }
            else _prendas.Registrar(prenda, _usuario);
            MessageBox.Show("La prenda se guardó correctamente.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Limpiar(); BuscarPrendas();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Revise los datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch
        {
            MessageBox.Show("No se pudo guardar la prenda. Revise el SKU, las referencias y la conexión.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private Prenda LeerFormulario()
    {
        if (!int.TryParse(_categoria.SelectedValue?.ToString(), out int categoriaId) ||
            !int.TryParse(_talla.SelectedValue?.ToString(), out int tallaId) ||
            !int.TryParse(_color.SelectedValue?.ToString(), out int colorId))
            throw new ArgumentException("Seleccione categoría, talla y color.");
        if (!decimal.TryParse(_precio.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal precio))
            throw new ArgumentException("Escriba un precio válido en pesos chilenos.");
        if (!int.TryParse(_stock.Text, out int stock) || !int.TryParse(_stockMinimo.Text, out int minimo))
            throw new ArgumentException("Stock y stock mínimo deben ser números enteros.");
        return new Prenda
        {
            CodigoSKU = _sku.Text.Trim(), Nombre = _nombre.Text.Trim(), CategoriaId = categoriaId,
            TallaId = tallaId, ColorId = colorId, Marca = string.IsNullOrWhiteSpace(_marca.Text) ? null : _marca.Text.Trim(),
            Precio = precio, Stock = stock, StockMinimo = minimo
        };
    }

    private void PrepararEdicion()
    {
        if (_grilla.CurrentRow?.DataBoundItem is not Prenda prenda)
        {
            MessageBox.Show("Seleccione una prenda de la lista.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        _idSeleccionado = prenda.IdPrenda;
        _modoEdicion = true;
        _sku.Text = prenda.CodigoSKU; _nombre.Text = prenda.Nombre; _marca.Text = prenda.Marca ?? string.Empty;
        _precio.Text = prenda.Precio.ToString("0", CultureInfo.CurrentCulture);
        _stock.Text = prenda.Stock.ToString(); _stockMinimo.Text = prenda.StockMinimo.ToString();
        _categoria.SelectedValue = prenda.CategoriaId; _talla.SelectedValue = prenda.TallaId; _color.SelectedValue = prenda.ColorId;
        _sku.Focus();
    }

    private void Desactivar()
    {
        if (_usuario.Rol != "Administrador") return;
        if (_grilla.CurrentRow?.DataBoundItem is not Prenda prenda)
        {
            MessageBox.Show("Seleccione una prenda para desactivar.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show($"¿Desactivar la prenda {prenda.CodigoSKU}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            _prendas.Desactivar(prenda.IdPrenda, _usuario);
            Limpiar(); BuscarPrendas();
        }
        catch
        {
            MessageBox.Show("No se pudo desactivar la prenda.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Limpiar()
    {
        _idSeleccionado = null; _modoEdicion = false;
        _sku.Clear(); _nombre.Clear(); _marca.Clear(); _precio.Clear(); _stock.Clear(); _stockMinimo.Clear();
        if (_categoria.Items.Count > 0) _categoria.SelectedIndex = 0;
        if (_talla.Items.Count > 0) _talla.SelectedIndex = 0;
        if (_color.Items.Count > 0) _color.SelectedIndex = 0;
    }

    private static void AgregarCampo(TableLayoutPanel tabla, int columna, int fila, string texto, Control control)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, Margin = new Padding(3) };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 45)); panel.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        panel.Controls.Add(new Label { Text = texto, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        control.Dock = DockStyle.Fill;
        panel.Controls.Add(control, 0, 1);
        tabla.Controls.Add(panel, columna, fila);
    }

    private static ComboBox Lista() => new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 145 };
    private static Button Boton(string texto, EventHandler accion)
    {
        var boton = new Button { Text = texto, AutoSize = true, Height = 32, Margin = new Padding(4) };
        boton.Click += accion;
        return boton;
    }
}
