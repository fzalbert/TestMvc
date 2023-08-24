namespace InventarizationApi.Models.Repository.WarehouseGeometry;

public interface IWarehouseGeometryObjectsRepository
{
    public Task<Entities.WarehouseGeometryObject> Create(Entities.WarehouseGeometryObject warehouse);

    public Task Create(IEnumerable<Entities.WarehouseGeometryObject> items);
}