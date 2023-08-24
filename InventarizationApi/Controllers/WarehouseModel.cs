using NetTopologySuite.Geometries;

namespace inventarization_api.Controllers;


public class WarehouseModel
{
    /**
     * наименование
     */
    public string Name { get; set; } = string.Empty;

    /**
     * вид деятельности
     */
    public string ActivityType { get; set; } = string.Empty;
    
    /**
     * Гео объекты
     */
    public List<Geometry> GeoObjects { get; set; }
}