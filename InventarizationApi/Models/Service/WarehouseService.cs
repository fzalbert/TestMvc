using inventarization_api.Controllers;
using InventarizationApi.Controllers;
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

    public async Task Create(WarehouseCreateRequest createRequest)
    {
        var warehouse = new Entities.Warehouse(createRequest.Name, createRequest.ActivityType);

        var result = await _warehouseRepository.Create(warehouse);
        var warehouseId = result.Id;
        var items = createRequest.GeoObjects
            .Select(x => new Entities.WarehouseGeometryObject(x, warehouseId))
            .ToList();

        await _warehouseGeometryObjectsRepository.Create(items);
    }

    public async Task<ICollection<WarehouseModel>> GetIntersects(double lat, double lon)
    {
        var geometriesObjects = await _warehouseGeometryObjectsRepository
            .GetIntersected(lat, lon);

        if (!geometriesObjects.Any())
            return new List<WarehouseModel>();
        
        var warehouses = await _warehouseRepository
            .GetAllByIds(
                geometriesObjects
                    .Select(x => x.WarehouseId)
                    .ToList()
                );

        return warehouses
            .Select(x =>
                new WarehouseModel(
                    x.Id,
                    x.Name,
                    x.ActivityType, 
                    geometriesObjects.Where(warehouseGeo => warehouseGeo.WarehouseId == x.Id)
                        .Select(geo => geo.GeoObject)
                        .ToList()
                    )
            )
            .ToList();
    }
}