using System.ComponentModel.DataAnnotations;

namespace PanelPedidos.Api.Dtos.Autenticacion;

public sealed class LoginRequestDto
{
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(72)]
    public string Contrasena { get; set; } = string.Empty;
}