using System.Globalization;
using inventarization_api.Controllers;
using InventarizationApi.Models.Service;
using Microsoft.AspNetCore.Mvc;

namespace InventarizationApi.Controllers;

[Route("api/warehouse")]
public class WarehouseController : Controller
{
    private readonly WarehouseService _warehouseService;

    public WarehouseController(WarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost("")]
    public async Task<ActionResult> Create([FromBody] WarehouseModel request)
    {
        await _warehouseService.Create(request);
        return NoContent();
    }
}