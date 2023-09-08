using System.Data.Common;
using System.Text;
using InventarizationApi.Models.Service.Db;
using Microsoft.AspNetCore.Mvc;
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

    private const string SELECT_ALL_IN_ID = @"
    SELECT * FROM public.warehouse
    WHERE id IN({0})
";

    private readonly ILogger<WarehouseRepository> _logger;
    private readonly IConnectorCreator _connectorCreator;

    public WarehouseRepository(IConnectorCreator connectorCreator, ILogger<WarehouseRepository> logger)
    {
        _connectorCreator = connectorCreator;
        _logger = logger;
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

    public async Task<ICollection<Entities.Warehouse>> GetAllByIds(List<long> ids)
    {
        await using var connect = _connectorCreator.Create();
        await using var cmd = new NpgsqlCommand(SELECT_ALL_IN_ID, connect);
        await connect.OpenAsync();
        
        var idsText = new StringBuilder("");
        for (var i = 0; i < ids.Count - 1; i++)
        {
            idsText.Append(ids[i] + ", ");
        }
        if(ids.Count > 0)
            idsText.Append(ids[^1]);

        cmd.CommandText = string.Format(SELECT_ALL_IN_ID, idsText.ToString());
        
        NpgsqlDataReader reader;
        try
        {
            reader = await cmd.ExecuteReaderAsync();
        }
        catch (DbException ex)
        {
            _logger.LogCritical(ex, ex.Message, null);
            return new List<Entities.Warehouse>();
        }
        
        var result = new List<Entities.Warehouse>();

        while (reader.Read())
        {
            var id = reader.GetInt64(0);
            var name = reader.GetString(1);
            var type = reader.GetString(2);
            
            result.Add(new Entities.Warehouse(id, name, type));
        }

        return result;
    }
}