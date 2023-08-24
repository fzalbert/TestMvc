using Npgsql;

namespace InventarizationApi.Models.Service.Db;

public interface IConnectorCreator
{
    public NpgsqlConnection Create();
}