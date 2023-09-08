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
    public async Task<ActionResult> Create([FromBody] WarehouseCreateRequest request)
    {
        await _warehouseService.Create(request);
        return NoContent();
    }

    [HttpGet("")]
    public async Task<IActionResult> GetIntersecting([FromQuery] double? lat,[FromQuery] double? lon)
    {
        if (lat == null || lon == null)
            return StatusCode(400);

        var result = await _warehouseService.GetIntersects(lat.Value, lon.Value);
        
        return Ok(result);
    }
}