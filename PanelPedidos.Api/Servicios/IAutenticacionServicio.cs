using PanelPedidos.Api.Dtos.Autenticacion;

namespace PanelPedidos.Api.Servicios;

public interface IAutenticacionServicio
{
    Task<LoginResponseDto?> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);
}