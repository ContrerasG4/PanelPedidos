using PanelPedidos.Api.Entidades;

namespace PanelPedidos.Api.Repositorios;

public interface IUsuarioRepositorio
{
    Task<Usuario?> ObtenerPorCorreoAsync(
        string correo,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> ObtenerPermisosAsync(
        long rolId,
        CancellationToken cancellationToken = default);
    Task<bool> RegistrarIntentoFallidoAsync(
    long usuarioId,
    string hashContrasena,
    int maximoIntentos,
    int minutosBloqueo,
    CancellationToken cancellationToken = default);

    Task<bool> ReiniciarIntentosFallidosAsync(
        long usuarioId,
        string hashContrasena,
        CancellationToken cancellationToken = default);
}

