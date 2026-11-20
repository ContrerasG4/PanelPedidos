using System.Security.Cryptography;
using System.Text;

namespace PanelPedidos.Api.Seguridad;

public sealed class GeneradorRefreshToken : IGeneradorRefreshToken
{
    public string Generar()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToHexString(bytes);
    }

    public string CalcularHash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}