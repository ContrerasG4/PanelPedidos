namespace PanelPedidos.Api.Seguridad;

public interface IGeneradorRefreshToken
{
    string Generar();
    string CalcularHash(string token);
}