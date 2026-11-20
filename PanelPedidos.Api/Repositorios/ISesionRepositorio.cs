using PanelPedidos.Api.Entidades;

namespace PanelPedidos.Api.Repositorios;

public interface ISesionRepositorio
{
    Task CrearSesionAsync(
        Sesion sesion,
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);

    Task<long?> RotarRefreshTokenAsync(
    string hashTokenActual,
    string hashTokenNuevo,
    CancellationToken cancellationToken = default);

}