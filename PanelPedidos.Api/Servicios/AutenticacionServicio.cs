using System.Text;
using Microsoft.Extensions.Options;
using PanelPedidos.Api.Dtos.Autenticacion;
using PanelPedidos.Api.Repositorios;
using PanelPedidos.Api.Seguridad;

namespace PanelPedidos.Api.Servicios;

public sealed class AutenticacionServicio: IAutenticacionServicio
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IGeneradorToken _generadorToken;
    private readonly JwtOpciones _jwtOpciones;
    private readonly AutenticacionOpciones _autenticacionOpciones;

    public AutenticacionServicio(
        IUsuarioRepositorio usuarioRepositorio,
        IGeneradorToken generadorToken,
        IOptions<JwtOpciones> jwtOpciones,
        IOptions<AutenticacionOpciones> autenticacionOpciones)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _generadorToken = generadorToken;
        _jwtOpciones = jwtOpciones.Value;
        _autenticacionOpciones = autenticacionOpciones.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (Encoding.UTF8.GetByteCount(request.Contrasena) > 72)
        {
            return null;
        }

        var usuario = await _usuarioRepositorio
            .ObtenerPorCorreoAsync(
                request.Correo.Trim(),
                cancellationToken);

        if (usuario is null || !usuario.EstaActivo)
        {
            return null;
        }

        if (usuario.BloqueadoHasta.HasValue &&
            usuario.BloqueadoHasta.Value > DateTime.UtcNow)
        {
            return null;
        }

        bool contrasenaValida = BCrypt.Net.BCrypt.Verify(
            request.Contrasena,
            usuario.HashContrasena);

        if (!contrasenaValida)
        {
            await _usuarioRepositorio.RegistrarIntentoFallidoAsync(
                usuario.Id,
                usuario.HashContrasena,
                _autenticacionOpciones.MaximoIntentos,
                _autenticacionOpciones.MinutosBloqueo,
                cancellationToken);

            return null;
        }

        var intentosReiniciados = await _usuarioRepositorio
            .ReiniciarIntentosFallidosAsync(
                usuario.Id,
                usuario.HashContrasena,
                cancellationToken);

        if (!intentosReiniciados)
        {
            return null;
        }

        var permisos = (await _usuarioRepositorio
            .ObtenerPermisosAsync(
                usuario.RolId,
                cancellationToken))
            .ToArray();

        var accessToken = _generadorToken.GenerarAccessToken(
            usuario.Id,
            usuario.Correo,
            usuario.Rol,
            permisos);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            UsuarioId = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Correo = usuario.Correo,
            Rol = usuario.Rol,
            Permisos = permisos,
            ExpiraEnSegundos =
                _jwtOpciones.ExpiracionMinutos * 60
        };
    }
}