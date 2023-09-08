using NetTopologySuite.Geometries;

namespace inventarization_api.Controllers;

public class WarehouseCreateRequest
{
    
    public WarehouseCreateRequest(List<Geometry> geoObjects)
    {
        GeoObjects = geoObjects;
    }

    public WarehouseCreateRequest(string name, string activityType, List<Geometry> geoObjects)
    {
        Name = name;
        ActivityType = activityType;
        GeoObjects = geoObjects;
    }

    /**
     * наименование
     */
    public string Name { get; set; }

    /**
     * вид деятельности
     */
    public string ActivityType { get; set; }

    /**
     * Гео объекты
     */
    public List<Geometry> GeoObjects { get; set; }
}