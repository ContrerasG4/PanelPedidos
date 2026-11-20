namespace PanelPedidos.Api.Dtos.Autenticacion;

public sealed class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public long UsuarioId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public IEnumerable<string> Permisos { get; set; } = [];
    public int ExpiraEnSegundos { get; set; }
}