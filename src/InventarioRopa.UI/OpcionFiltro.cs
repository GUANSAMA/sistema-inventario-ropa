namespace InventarioRopa.UI;

internal sealed record OpcionFiltro<T>(T Valor, string Texto)
{
    public override string ToString() => Texto;
}
