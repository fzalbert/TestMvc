using inventarization_api.Controllers;
using InventarizationApi.Models.Repository.Warehouse;
using InventarizationApi.Models.Repository.WarehouseGeometry;

namespace InventarizationApi.Models.Service;

public class WarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWarehouseGeometryObjectsRepository _warehouseGeometryObjectsRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository,
        IWarehouseGeometryObjectsRepository warehouseGeometryObjectsRepository)
    {
        _warehouseRepository = warehouseRepository;
        _warehouseGeometryObjectsRepository = warehouseGeometryObjectsRepository;
    }

    public async Task Create(WarehouseModel model)
    {
        var warehouse = new Entities.Warehouse(model.Name, model.ActivityType);

        var result = await _warehouseRepository.Create(warehouse);
        var warehouseId = result.Id;
        var items = model.GeoObjects
            .Select(x => new Entities.WarehouseGeometryObject(x, warehouseId))
            .ToList();

        await _warehouseGeometryObjectsRepository.Create(items);
    }
}