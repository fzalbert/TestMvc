using NetTopologySuite.Geometries;

namespace InventarizationApi.Controllers;

public class WarehouseModel
{
    public WarehouseModel(long id, string name, string activityType, List<Geometry> geoObjects)
    {
        Id = id;
        Name = name;
        ActivityType = activityType;
        GeoObjects = geoObjects;
    }

    public long Id { get; set; }

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