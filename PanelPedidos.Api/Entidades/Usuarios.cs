namespace PanelPedidos.Api.Entidades;

public class Usuario
{
    public long Id { get; set; }
    public string Correo { get; set; } = string.Empty;
    public string HashContrasena { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public long RolId { get; set; }
    public bool EstaActivo { get; set; }
    public int IntentosFallidos { get; set; }
    public DateTime? BloqueadoHasta { get; set; }
    public DateTime CreadoEn { get; set; }
    public DateTime ActualizadoEn { get; set; }

    public string Rol { get; set; } = string.Empty;
}