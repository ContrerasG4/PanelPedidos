using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PanelPedidos.Api.Dtos.Autenticacion;
using PanelPedidos.Api.Servicios;

namespace PanelPedidos.Api.Controladores;

[ApiController]
[Route("api/autenticacion")]
public sealed class AutenticacionControlador : ControllerBase
{
    private readonly IAutenticacionServicio _autenticacionServicio;

    public AutenticacionControlador(
        IAutenticacionServicio autenticacionServicio)
    {
        _autenticacionServicio = autenticacionServicio;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var respuesta = await _autenticacionServicio
            .LoginAsync(request, cancellationToken);

        if (respuesta is null)
        {
            return Unauthorized(new
            {
                Mensaje = "Credenciales incorrectas o acceso no disponible."
            });
        }

        return Ok(respuesta);
    }
}