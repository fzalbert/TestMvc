using InventarizationApi.Models.Config;
using Microsoft.Extensions.Options;
using Npgsql;

namespace InventarizationApi.Models.Service.Db;

public class PostgresConnectorCreator : IConnectorCreator, IDisposable
{
    private readonly NpgsqlDataSource  _dataSource;
    
    public PostgresConnectorCreator(IOptions<DataSource> dataSource)
    {
        var connectionString = dataSource.Value.GetConnectionString();
        
        var builder = new NpgsqlDataSourceBuilder(connectionString);
        builder.UseNetTopologySuite();
        _dataSource = builder.Build();
    }

    public NpgsqlConnection Create()
    {
        return _dataSource.CreateConnection();
    }
    
    public void Dispose()
    {
    }
}