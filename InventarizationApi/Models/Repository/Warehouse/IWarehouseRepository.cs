namespace InventarizationApi.Models.Repository.Warehouse;

public interface IWarehouseRepository
{
    public Task<Entities.Warehouse> Create(Entities.Warehouse warehouse);

    public Task<ICollection<Entities.Warehouse>> GetAllByIds(List<long> ids);
    
}