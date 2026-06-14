using System.Data;
using Dapper;
using Npgsql;

namespace Api.Data.Repositories;

public class BaseRepository(string connectionString)
{
    protected IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);

    protected async Task<int> ExecuteAsync(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteAsync(sql, parameters);
    }

    protected async Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<T>(sql, parameters);
    }
}
