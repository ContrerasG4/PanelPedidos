using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PanelPedidos.Api.Seguridad;

namespace PanelPedidos.Api.Configuracion;

public static class ConfiguracionJwt
{
    public static IServiceCollection AgregarJwt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var opciones = configuration
            .GetSection(JwtOpciones.Seccion)
            .Get<JwtOpciones>()
            ?? throw new InvalidOperationException(
                "No se encontró la configuración JWT.");

        if (string.IsNullOrWhiteSpace(opciones.Clave) ||
            string.IsNullOrWhiteSpace(opciones.Emisor) ||
            string.IsNullOrWhiteSpace(opciones.Audiencia) ||
            opciones.ExpiracionMinutos <= 0)
        {
            throw new InvalidOperationException(
                "La configuración JWT está incompleta.");
        }

        byte[] clave;

        try
        {
            clave = Convert.FromBase64String(opciones.Clave);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "La clave JWT debe estar codificada en Base64.", ex);
        }

        if (clave.Length < 32)
        {
            throw new InvalidOperationException(
                "La clave JWT debe contener al menos 32 bytes.");
        }

        services.Configure<JwtOpciones>(
            configuration.GetSection(JwtOpciones.Seccion));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        RoleClaimType = "role",
                        NameClaimType = "sub",

                        ValidateIssuer = true,
                        ValidIssuer = opciones.Emisor,

                        ValidateAudience = true,
                        ValidAudience = opciones.Audiencia,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(clave),

                        RequireSignedTokens = true,

                        ValidAlgorithms =
                            new[] { SecurityAlgorithms.HmacSha256 },

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

        services.AddAuthorization();
        services.AddSingleton<IGeneradorToken, GeneradorToken>();
        return services;
    }
}