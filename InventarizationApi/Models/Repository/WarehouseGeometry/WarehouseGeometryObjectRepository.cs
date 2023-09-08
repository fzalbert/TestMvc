using System.Data;
using System.Text.Json;
using InventarizationApi.Models.Entities;
using InventarizationApi.Models.Service.Db;
using InventarizationApi.Utils;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using Npgsql;

namespace InventarizationApi.Models.Repository.WarehouseGeometry;

public class WarehouseGeometryObjectRepository : IWarehouseGeometryObjectsRepository
{
    private readonly IConnectorCreator _connectorCreator;
    private readonly IOptions<JsonOptions> _jsOptions;
    private readonly GeometryToJsonSerializer _geometrySerializer;
    
    private const string INSERT_QUERY_PREFIX = "INSERT INTO public.warehouse_object (geom, warehouse_id) VALUES";
    
    private const string INSERT_QUERY = @"
            INSERT INTO public.warehouse_object (geom, warehouse_id)
            VALUES(ST_GeomFromGeoJSON(''@geom''), @warehouse_id)
            RETURNING id;
";

    private const string SELECT_INTERSECTED_OBJECT = @"
select distinct wo.id, wo.geom, wo.warehouse_id from warehouse_object as wo
inner join (select geom from warehouse_object
	where ST_Intersects(geom, st_point(@lat, @lon, 4326))) as wo2
on ST_Intersects(wo.geom, wo2.geom);
";
    
    public WarehouseGeometryObjectRepository(
        IConnectorCreator connectorCreator,
        IOptions<JsonOptions> jsOptions, 
        GeometryToJsonSerializer geometrySerializer
        )
    {
        _connectorCreator = connectorCreator;
        _jsOptions = jsOptions;
        _geometrySerializer = geometrySerializer;
    }

    public async Task<WarehouseGeometryObject> Create(WarehouseGeometryObject entity)
    {
        await using var connect = _connectorCreator.Create();
        await using var cmd = new NpgsqlCommand(INSERT_QUERY, connect);
        
        var geoJson = _geometrySerializer.Serialize(entity.GeoObject);
        cmd.Parameters.AddWithValue("@geom", geoJson);
        cmd.Parameters.AddWithValue("@warehouse_id", entity.WarehouseId);
        
        await connect.OpenAsync();
        
        var result = await cmd.ExecuteScalarAsync();
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
            var geoJson = _geometrySerializer.Serialize(item.GeoObject);
            resultSql += $" (ST_GeomFromGeoJSON('{geoJson}'), {item.WarehouseId}),";
        }
        
        resultSql = resultSql[..^1] + ";";
        await using var cmd = new NpgsqlCommand(resultSql, connect);
        await cmd.ExecuteScalarAsync();
        
    }

    public async Task<ICollection<WarehouseGeometryObject>> GetIntersected(double lat, double lon)
    {
        await using var connect = _connectorCreator.Create();
        await using var cmd = new NpgsqlCommand(SELECT_INTERSECTED_OBJECT, connect);
        await connect.OpenAsync();
        
        cmd.Parameters.AddWithValue("@lat", lat);
        cmd.Parameters.AddWithValue("@lon", lon);
        
        var sqlDataReader = await cmd.ExecuteReaderAsync();
        
        var result = new List<WarehouseGeometryObject>();
        
        while (sqlDataReader.Read())
        {
            var id = sqlDataReader.GetInt64(0);
            var geometry = sqlDataReader[1] as Geometry;
            var warehouseId = sqlDataReader.GetInt64(2);
            result.Add(new WarehouseGeometryObject(id, geometry!, warehouseId));
        }
        
        return result;
    }
}