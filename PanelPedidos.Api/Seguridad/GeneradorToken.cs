using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace PanelPedidos.Api.Seguridad;

public sealed class GeneradorToken : IGeneradorToken
{
    private readonly JwtOpciones _opciones;

    public GeneradorToken(IOptions<JwtOpciones> opciones)
    {
        _opciones = opciones.Value;
    }

    public string GenerarAccessToken(
        long usuarioId,
        string correo,
        string rol,
        IEnumerable<string> permisos)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new(JwtRegisteredClaimNames.Email, correo),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("role", rol)
        };

        claims.AddRange(
            permisos.Select(permiso => new Claim("permiso", permiso))
        );

        var clave = new SymmetricSecurityKey(
            Convert.FromBase64String(_opciones.Clave)
        );

        var credenciales = new SigningCredentials(
            clave,
            SecurityAlgorithms.HmacSha256
        );

        var ahora = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: _opciones.Emisor,
            audience: _opciones.Audiencia,
            claims: claims,
            notBefore: ahora,
            expires: ahora.AddMinutes(_opciones.ExpiracionMinutos),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}