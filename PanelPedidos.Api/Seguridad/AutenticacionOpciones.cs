namespace PanelPedidos.Api.Seguridad;

public sealed class AutenticacionOpciones
{
    public const string Seccion = "Autenticacion";

    public int MaximoIntentos { get; set; } = 5;
    public int MinutosBloqueo { get; set; } = 15;
    public int DiasSesion { get; set; } = 7;
}