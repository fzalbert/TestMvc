using NetTopologySuite.Geometries;

namespace InventarizationApi.Models.Entities;

public class Warehouse
{
    public Warehouse(string name, string activityType)
    {
        Name = name;
        ActivityType = activityType;
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
}