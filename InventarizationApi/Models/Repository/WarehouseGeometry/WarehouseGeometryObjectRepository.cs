using System.Text.Json;
using InventarizationApi.Models.Entities;
using InventarizationApi.Models.Service.Db;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Npgsql;

namespace InventarizationApi.Models.Repository.WarehouseGeometry;

public class WarehouseGeometryObjectRepository : IWarehouseGeometryObjectsRepository
{
    private readonly IConnectorCreator _connectorCreator;
    private readonly IOptions<JsonOptions> _jsOptions;

    private const string INSERT_QUERY_PREFIX = "INSERT INTO public.warehouse_object (geom, warehouse_id) VALUES";
    
    private const string INSERT_QUERY = @"
            INSERT INTO public.warehouse_object (geom, warehouse_id)
            VALUES(ST_GeomFromGeoJSON(''@geom''), @warehouse_id)
            RETURNING id;
";
    
    public WarehouseGeometryObjectRepository(
        IConnectorCreator connectorCreator,
        IOptions<JsonOptions> jsOptions
        )
    {
        _connectorCreator = connectorCreator;
        _jsOptions = jsOptions;
    }

    public async Task<WarehouseGeometryObject> Create(WarehouseGeometryObject entity)
    {
        await using var connect = _connectorCreator.Create();
        await using var cmd = new NpgsqlCommand(INSERT_QUERY, connect);
        await connect.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        var geoJson = JsonSerializer.Serialize(entity.GeoObject, _jsOptions.Value.SerializerOptions);
        
        cmd.Parameters.AddWithValue("@geom", geoJson);
        cmd.Parameters.AddWithValue("@warehouse_id", entity.WarehouseId);
        
        if (result == null)
            throw new Exception();
        
        entity.Id = (int) result;
        
        return entity;
    }

    public async Task Create(IEnumerable<WarehouseGeometryObject> items)
    {
        if(!items.Any())
            return;
        
        await using var connect = _connectorCreator.Create();
        await connect.OpenAsync();
        
        var resultSql = INSERT_QUERY_PREFIX;
        
        foreach (var item in items)
        {
            var jsOptions = new JsonOptions();
            jsOptions.SerializerOptions.Converters.Add(
                new NetTopologySuite.IO.Converters.GeoJsonConverterFactory()
            );
            var geoJson = JsonSerializer.Serialize(item.GeoObject, jsOptions.SerializerOptions);
            resultSql += $" (ST_GeomFromGeoJSON('{geoJson}'), {item.WarehouseId}),";
        }
        resultSql = resultSql[..^1] + ";";
        await using var cmd = new NpgsqlCommand(resultSql, connect);
        await cmd.ExecuteScalarAsync();
        
    }
}