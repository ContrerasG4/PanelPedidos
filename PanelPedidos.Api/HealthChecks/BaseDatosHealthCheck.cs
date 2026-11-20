using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace PanelPedidos.Api.HealthChecks;

public class BaseDatosHealthCheck : IHealthCheck
{
    private readonly NpgsqlDataSource _dataSource;

    public BaseDatosHealthCheck(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        await using var conexion =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = "SELECT 1";

        var resultado = await conexion.ExecuteScalarAsync<int>(
            new CommandDefinition(
                sql,
                cancellationToken: cancellationToken
            )
        );

        return resultado == 1
            ? HealthCheckResult.Healthy("Base de datos disponible")
            : HealthCheckResult.Unhealthy("Base de datos no disponible");
    }
}