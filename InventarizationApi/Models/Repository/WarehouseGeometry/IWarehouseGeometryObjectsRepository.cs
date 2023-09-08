namespace InventarizationApi.Models.Repository.WarehouseGeometry;

public interface IWarehouseGeometryObjectsRepository
{
    public Task<Entities.WarehouseGeometryObject> Create(Entities.WarehouseGeometryObject warehouse);

    public Task Create(IEnumerable<Entities.WarehouseGeometryObject> items);

    public Task<ICollection<Entities.WarehouseGeometryObject>> GetIntersected(double lat, double lon);
}