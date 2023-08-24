using InventarizationApi.Models.Config;
using Microsoft.Extensions.Options;
using Npgsql;

namespace InventarizationApi.Models.Service.Db;

public class PostgresConnectorCreator : IConnectorCreator, IDisposable
{
    private readonly DataSource  _dataSource;
    
    public PostgresConnectorCreator(IOptions<DataSource> dataSource)
    {
        _dataSource = dataSource.Value;
    }

    public NpgsqlConnection Create()
    {
        var connectionString = _dataSource.GetConnectionString();
        var dbConnection = new NpgsqlConnection(connectionString);
        
        // var selectCommand = new NpgsqlCommand("SELECT * FROM MyTable", _connection);
        // using var results = selectCommand.ExecuteReader();
        // results.Read();
        
        return dbConnection;
    }
    
    public void Dispose()
    {
    }
}