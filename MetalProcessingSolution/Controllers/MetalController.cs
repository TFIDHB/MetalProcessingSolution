using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetalProcessingSolution.Controllers;

[ApiController]
[Route("api")]
public class MetalController(IMetalProcessingService service) : ControllerBase
{
    [HttpGet("services")]
    public async Task<IActionResult> GetServices(CancellationToken ct)
    {
        var result = await service.GetServicesAsync(ct);
        return Ok(result);
    }

    [HttpPost("services")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateService([FromForm] MetalServiceDto dto, CancellationToken ct)
    {
        await service.CreateServiceAsync(dto, ct);
        return Ok();
    }

    [HttpPut("services/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateService(int id, [FromForm] MetalServiceDto dto, CancellationToken ct)
    {
        await service.UpdateServiceAsync(id, dto, ct);
        return Ok(new { success = true });
    }

    [HttpDelete("services/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteService(int id, CancellationToken ct)
    {
        await service.DeleteServiceAsync(id, ct);
        return Ok(new { success = true });
    }

    [HttpGet("unliquid")]
    public async Task<IActionResult> GetUnliquid(CancellationToken ct)
    {
        var result = await service.GetUnliquidProductsAsync(ct);
        return Ok(result);
    }

    [HttpPost("unliquid")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUnliquid([FromForm] UnliquidProductDto dto, CancellationToken ct)
    {
        await service.CreateUnliquidAsync(dto, ct);
        return Ok(new { success = true });
    }

    [HttpPut("unliquid/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUnliquid(int id, [FromForm] UnliquidProductDto dto, CancellationToken ct)
    {
        await service.UpdateUnliquidAsync(id, dto, ct);
        return Ok(new { success = true });
    }

    [HttpDelete("unliquid/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUnliquid(int id, CancellationToken ct)
    {
        await service.DeleteUnliquidAsync(id, ct);
        return Ok(new { success = true });
    }

    [HttpGet("admin/stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var result = await service.GetStatsAsync(ct);
        return Ok(result);
    }

    [HttpPost("admin/adjust-service-prices")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdjustServicePrices([FromBody] PriceAdjustmentDto dto, CancellationToken ct)
    {
        await service.AdjustServicePricesAsync(dto, ct);
        return Ok(new { success = true });
    }

    [HttpPost("admin/adjust-unliquid-prices")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdjustUnliquidPrices([FromBody] PriceAdjustmentDto dto, CancellationToken ct)
    {
        await service.AdjustUnliquidPricesAsync(dto, ct);
        return Ok(new { success = true });
    }
}