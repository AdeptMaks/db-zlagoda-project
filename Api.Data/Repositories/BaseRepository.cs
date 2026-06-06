using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace Benefitwerks.DataAccess.Repositories;

public class BaseRepository
{
    protected string _connectionString;

    /// <summary>
    /// Instantiate an empty repository.
    /// </summary>
    public BaseRepository()
    {
    }

    /// <summary>
    /// Instantiate a repository with the given configuration.
    /// </summary>
    /// <param name="config"></param>
    public BaseRepository(string connectionString) : base()
    {
        SetConfiguration(connectionString);
    }

    /// <summary>
    /// Set the repository's configuration to the one provided.
    /// </summary>
    /// <param name="config"></param>
    protected void SetConfiguration(string connectionString)
    {
        this._connectionString = connectionString;
    }

    protected async Task<SqlConnection> GetConnection()
    {
        if (!_connectionString.Contains("database.windows.net", System.StringComparison.OrdinalIgnoreCase))
        {
            var localConnection = new SqlConnection(_connectionString);
            return localConnection;
        }
        var factory = RedisAzureTokenFactory.Instance;

        SqlConnection connection = await factory.CreateConnectionAsync(_connectionString);

        return connection;
    }

    /// <summary>
    /// Execute SQL query, no data to return
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    public async Task<int> ExecuteQuery(string query, object parameters)
    {
        using var connection = await GetConnection();

        var output = await connection.ExecuteAsync(
            query,
            param: parameters,
            commandType: CommandType.Text);

        return output;
    }

    /// <summary>
    /// Implement the GetCommand for a generic Model. Retrieves a single result.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public virtual async Task<TModel> Get<TModel>(string query, object parameters) where TModel : new()
    {
        using var connection = await GetConnection();

        TModel model = await connection.QuerySingleOrDefaultAsync<TModel>
            (query, param: parameters, commandType: CommandType.Text);

        return model;
    }

    /// <summary>
    /// Implement the GetCommand for a string. Retrieves a single result.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public virtual async Task<string> Get(string query, object parameters)
    {
        using var connection = await GetConnection();

        return await connection.QuerySingleOrDefaultAsync<string>
            (query, param: parameters, commandType: CommandType.Text);
    }

    /// <summary>
    /// Implement the GetCommand for a generic Model. Retrieves resultset.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<TModel>> Query<TModel>(string query, object parameters)
        where TModel : new()
    {
        using var connection = await GetConnection();

        IEnumerable<TModel> models = await connection.QueryAsync<TModel>(query, param: parameters, commandType: CommandType.Text);
        return models;
    }


}

