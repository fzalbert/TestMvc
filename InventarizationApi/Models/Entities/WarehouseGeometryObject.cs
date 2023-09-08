using NetTopologySuite.Geometries;

namespace InventarizationApi.Models.Entities;

public class WarehouseGeometryObject
{

    public long Id { get; set; }
    
    /**
     * Гео объекты
     */
    public Geometry GeoObject { get; set; }
    
    /**
     * Warehouse fk
     */
    public long WarehouseId { get; set; }
    
    public WarehouseGeometryObject(Geometry geoObject, long warehouseId)
    {
        GeoObject = geoObject;
        WarehouseId = warehouseId;
    }

    public WarehouseGeometryObject(long id, Geometry geoObject, long warehouseId)
    {
        Id = id;
        GeoObject = geoObject;
        WarehouseId = warehouseId;
    }
}