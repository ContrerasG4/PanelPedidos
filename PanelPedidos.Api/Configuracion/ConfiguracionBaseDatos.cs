using Npgsql;

namespace PanelPedidos.Api.Configuracion;

public static class ConfiguracionBaseDatos
{
    public static IServiceCollection AgregarBaseDatos(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cadenaConexion = configuration
            .GetConnectionString("Principal");

        if (string.IsNullOrWhiteSpace(cadenaConexion))
        {
            throw new InvalidOperationException(
                "No se ha configurado la conexión a la base de datos.");
        }

        services.AddSingleton<NpgsqlDataSource>(_ =>
            NpgsqlDataSource.Create(cadenaConexion));

        return services;
    }
}