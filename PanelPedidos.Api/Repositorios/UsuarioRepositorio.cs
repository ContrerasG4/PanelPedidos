using Dapper;
using Npgsql;
using PanelPedidos.Api.Entidades;

namespace PanelPedidos.Api.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly NpgsqlDataSource _dataSource;

    public UsuarioRepositorio(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                u.id AS Id,
                u.correo AS Correo,
                u.hash_contrasena AS HashContrasena,
                u.nombre_completo AS NombreCompleto,
                u.rol_id AS RolId,
                u.esta_activo AS EstaActivo,
                u.intentos_fallidos AS IntentosFallidos,
                u.bloqueado_hasta AS BloqueadoHasta,
                u.creado_en AS CreadoEn,
                u.actualizado_en AS ActualizadoEn,
                r.nombre AS Rol
            FROM usuarios u
            INNER JOIN roles r ON r.id = u.rol_id
            WHERE u.correo = @Correo;
            """;

        await using var conexion =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        return await conexion.QuerySingleOrDefaultAsync<Usuario>(
            new CommandDefinition(
                sql,
                new { Correo = correo },
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<IEnumerable<string>> ObtenerPermisosAsync(
        long rolId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT p.codigo
            FROM permisos p
            INNER JOIN rol_permisos rp
                ON rp.permiso_id = p.id
            WHERE rp.rol_id = @RolId;
            """;

        await using var conexion =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        return await conexion.QueryAsync<string>(
            new CommandDefinition(
                sql,
                new { RolId = rolId },
                cancellationToken: cancellationToken
            )
        );
    }

    public async Task<bool> RegistrarIntentoFallidoAsync(long usuarioId,string hashContrasena,int maximoIntentos,int minutosBloqueo,CancellationToken cancellationToken = default)
    {
        const string sql = """
        UPDATE usuarios
        SET
            intentos_fallidos = CASE
                WHEN bloqueado_hasta IS NOT NULL THEN 1
                ELSE intentos_fallidos + 1
            END,
            bloqueado_hasta = CASE
                WHEN (
                    CASE
                        WHEN bloqueado_hasta IS NOT NULL THEN 1
                        ELSE intentos_fallidos + 1
                    END
                ) >= @MaximoIntentos
                THEN NOW() + make_interval(mins => @MinutosBloqueo)
                ELSE NULL
            END
        WHERE id = @UsuarioId
            AND esta_activo = TRUE
            AND hash_contrasena = @HashContrasena
            AND (
                bloqueado_hasta IS NULL
                OR bloqueado_hasta <= NOW()
            );
        """;

        await using var conexion =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        var filasAfectadas = await conexion.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    UsuarioId = usuarioId,
                    HashContrasena = hashContrasena,
                    MaximoIntentos = maximoIntentos,
                    MinutosBloqueo = minutosBloqueo
                },
                cancellationToken: cancellationToken
            )
        );

        return filasAfectadas == 1;
    }

    public async Task<bool> ReiniciarIntentosFallidosAsync(long usuarioId,string hashContrasena,CancellationToken cancellationToken = default)
    {
        const string sql = """
        UPDATE usuarios
        SET
            intentos_fallidos = 0,
            bloqueado_hasta = NULL
        WHERE id = @UsuarioId
            AND esta_activo = TRUE
            AND hash_contrasena = @HashContrasena
            AND (
                bloqueado_hasta IS NULL
                OR bloqueado_hasta <= NOW()
            );
        """;

        await using var conexion =
            await _dataSource.OpenConnectionAsync(cancellationToken);

        var filasAfectadas = await conexion.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    UsuarioId = usuarioId,
                    HashContrasena = hashContrasena
                },
                cancellationToken: cancellationToken
            )
        );

        return filasAfectadas == 1;
    }
}