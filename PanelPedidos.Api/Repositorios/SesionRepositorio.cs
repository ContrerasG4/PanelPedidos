using Dapper;
using Npgsql;
using PanelPedidos.Api.Entidades;

namespace PanelPedidos.Api.Repositorios;

public sealed class SesionRepositorio : ISesionRepositorio
{
    private readonly NpgsqlDataSource _dataSource;

    public SesionRepositorio(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }
    //FUNCIÓN PARA CREAR UNA NUEVA SESIÓN, SE UTILIZA PARA LA AUTENTICACIÓN DE USUARIOS
    public async Task CrearSesionAsync(
        Sesion sesion,
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        const string sqlSesion = """
            INSERT INTO sesiones (
                id,
                usuario_id,
                creado_en,
                expira_en
            )
            VALUES (
                @Id,
                @UsuarioId,
                @CreadoEn,
                @ExpiraEn
            );
            """;

        const string sqlRefreshToken = """
            INSERT INTO refresh_tokens (
                sesion_id,
                hash_token,
                creado_en
            )
            VALUES (
                @SesionId,
                @HashToken,
                @CreadoEn
            );
            """;

        await using var conexion =
            await _dataSource.OpenConnectionAsync(
                cancellationToken);

        await using var transaccion =
            await conexion.BeginTransactionAsync(
                cancellationToken);

        try
        {
            await conexion.ExecuteAsync(
                new CommandDefinition(
                    sqlSesion,
                    sesion,
                    transaction: transaccion,
                    cancellationToken: cancellationToken));

            await conexion.ExecuteAsync(
                new CommandDefinition(
                    sqlRefreshToken,
                    new
                    {
                        SesionId = sesion.Id,
                        refreshToken.HashToken,
                        refreshToken.CreadoEn
                    },
                    transaction: transaccion,
                    cancellationToken: cancellationToken));

            await transaccion.CommitAsync(
                cancellationToken);
        }
        catch
        {
            await transaccion.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

    //FUNCIÓN PARA ROTAR EL REFRESH TOKEN, SE UTILIZA PARA LA AUTENTICACIÓN DE USUARIOS

    public async Task<long?> RotarRefreshTokenAsync(
        string hashTokenActual,
        string hashTokenNuevo,
        CancellationToken cancellationToken = default)
    {
        const string sqlBuscarSesionId = """
        SELECT sesion_id
        FROM refresh_tokens
        WHERE hash_token = @HashToken;
        """;

        const string sqlBloquearSesion = """
        SELECT
            id AS Id,
            usuario_id AS UsuarioId,
            creado_en AS CreadoEn,
            expira_en AS ExpiraEn,
            revocado_en AS RevocadoEn
        FROM sesiones
        WHERE id = @SesionId
        FOR UPDATE;
        """;

        const string sqlConsultarToken = """
        SELECT
            id AS Id,
            sesion_id AS SesionId,
            hash_token AS HashToken,
            creado_en AS CreadoEn,
            utilizado_en AS UtilizadoEn
        FROM refresh_tokens
        WHERE hash_token = @HashToken
            AND sesion_id = @SesionId;
        """;

        const string sqlRevocarSesion = """
        UPDATE sesiones
        SET
            revocado_en = NOW(),
            motivo_revocacion = 'reutilizacion_token'
        WHERE id = @SesionId;
        """;

        const string sqlConsumirToken = """
        UPDATE refresh_tokens
        SET utilizado_en = NOW()
        WHERE id = @TokenId;
        """;

        const string sqlInsertarToken = """
        INSERT INTO refresh_tokens (
            sesion_id,
            hash_token
        )
        VALUES (
            @SesionId,
            @HashToken
        );
        """;

        await using var conexion =
            await _dataSource.OpenConnectionAsync(
                cancellationToken);

        await using var transaccion =
            await conexion.BeginTransactionAsync(
                System.Data.IsolationLevel.ReadCommitted,
                cancellationToken);

        try
        {
            var sesionId = await conexion
                .QuerySingleOrDefaultAsync<Guid?>(
                    new CommandDefinition(
                        sqlBuscarSesionId,
                        new { HashToken = hashTokenActual },
                        transaction: transaccion,
                        cancellationToken: cancellationToken));

            if (sesionId is null)
                return null;

            var sesion = await conexion
                .QuerySingleOrDefaultAsync<Sesion>(
                    new CommandDefinition(
                        sqlBloquearSesion,
                        new { SesionId = sesionId.Value },
                        transaction: transaccion,
                        cancellationToken: cancellationToken));

            if (sesion is null ||
                sesion.RevocadoEn.HasValue ||
                sesion.ExpiraEn <= DateTime.UtcNow)
            {
                return null;
            }

            var token = await conexion
                .QuerySingleOrDefaultAsync<RefreshToken>(
                    new CommandDefinition(
                        sqlConsultarToken,
                        new
                        {
                            HashToken = hashTokenActual,
                            SesionId = sesion.Id
                        },
                        transaction: transaccion,
                        cancellationToken: cancellationToken));

            if (token is null)
                return null;

            if (token.UtilizadoEn.HasValue)
            {
                await conexion.ExecuteAsync(
                    new CommandDefinition(
                        sqlRevocarSesion,
                        new { SesionId = sesion.Id },
                        transaction: transaccion,
                        cancellationToken: cancellationToken));

                await transaccion.CommitAsync(
                    cancellationToken);

                return null;
            }

            await conexion.ExecuteAsync(
                new CommandDefinition(
                    sqlConsumirToken,
                    new { TokenId = token.Id },
                    transaction: transaccion,
                    cancellationToken: cancellationToken));

            await conexion.ExecuteAsync(
                new CommandDefinition(
                    sqlInsertarToken,
                    new
                    {
                        SesionId = sesion.Id,
                        HashToken = hashTokenNuevo
                    },
                    transaction: transaccion,
                    cancellationToken: cancellationToken));

            await transaccion.CommitAsync(
                cancellationToken);

            return sesion.UsuarioId;
        }
        catch
        {
            await transaccion.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

}