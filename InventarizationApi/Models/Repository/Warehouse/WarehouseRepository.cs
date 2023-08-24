using InventarizationApi.Models.Service.Db;
using Npgsql;

namespace InventarizationApi.Models.Repository.Warehouse;

public class WarehouseRepository : IWarehouseRepository
{
    private const string INSERT_QUERY = @"
            INSERT INTO public.warehouse
        (company_name, activity_type)
        VALUES(@company_name, @activity_type)
        RETURNING id;
";

    private const string SELECT_ALL_QUERY = $@"
        SELECT * FROM public.warehouse;
";

    private readonly IConnectorCreator _connectorCreator;

    public WarehouseRepository(IConnectorCreator connectorCreator)
    {
        _connectorCreator = connectorCreator;
    }

    public async Task<Entities.Warehouse> Create(Entities.Warehouse entity)
    {
        await using var connect = _connectorCreator.Create();
        await using var cmd = new NpgsqlCommand(INSERT_QUERY, connect);
        await connect.OpenAsync();

        cmd.Parameters.AddWithValue("@company_name", entity.Name);
        cmd.Parameters.AddWithValue("@activity_type", entity.ActivityType);
        
        var result = await cmd.ExecuteScalarAsync();
        
        if (result == null)
            throw new Exception();
        
        entity.Id = (int) result;
        
        return entity;
    }
}