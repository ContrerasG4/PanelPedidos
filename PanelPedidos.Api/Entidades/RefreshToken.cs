namespace PanelPedidos.Api.Entidades;

public sealed class RefreshToken
{
    public long Id { get; set; }
    public Guid SesionId { get; set; }
    public string HashToken { get; set; } = string.Empty;
    public DateTime CreadoEn { get; set; }
    public DateTime? UtilizadoEn { get; set; }
}