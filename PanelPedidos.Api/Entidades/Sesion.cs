namespace PanelPedidos.Api.Entidades;

public sealed class Sesion
{
    public Guid Id { get; set; }
    public long UsuarioId { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime ExpiraEn { get; set; }
    public DateTime? RevocadoEn { get; set; }
    public string? MotivoRevocacion { get; set; }
}