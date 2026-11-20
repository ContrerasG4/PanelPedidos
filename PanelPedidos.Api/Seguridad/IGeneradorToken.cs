namespace PanelPedidos.Api.Seguridad;

public interface IGeneradorToken
{
	string GenerarAccessToken(
		long usuarioId,
		string correo,
		string rol,
		IEnumerable<string> permisos);
}