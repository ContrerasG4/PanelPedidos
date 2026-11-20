using PanelPedidos.Api.Repositorios;
using PanelPedidos.Api.Servicios;
using PanelPedidos.Api.Seguridad;

namespace PanelPedidos.Api.Configuracion;

public static class ConfiguracionDependencias
{
    public static IServiceCollection AgregarDependencias(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AutenticacionOpciones>()
            .Bind(configuration.GetSection(
                AutenticacionOpciones.Seccion))
            .Validate(
                opciones => opciones.MaximoIntentos > 0 &&
                            opciones.MinutosBloqueo > 0,
                "Configuración de autenticación inválida.")
            .ValidateOnStart();

        return services
            .AddScoped<IUsuarioRepositorio, UsuarioRepositorio>()
            .AddScoped<IAutenticacionServicio, AutenticacionServicio>()
            .AddScoped<ISesionRepositorio, SesionRepositorio>()
            .AddSingleton<IGeneradorRefreshToken, GeneradorRefreshToken>();

    }
}