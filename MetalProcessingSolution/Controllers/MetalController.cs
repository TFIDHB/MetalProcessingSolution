using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetalProcessingSolution.Controllers
{
    [ApiController]
    [Route("api")]
    public class MetalController(IMetalProcessingService service) : ControllerBase
    {
        [HttpGet("services")]
        public async Task<IActionResult> GetServices(CancellationToken ct)
            => Ok(await service.GetServicesAsync(ct));

        [HttpPost("services")]
        public async Task<IActionResult> CreateService([FromForm] MetalServiceDto dto, CancellationToken ct)
            => Ok(await service.CreateServiceAsync(dto, ct));

        [HttpPut("services/{id:int}")]
        public async Task<IActionResult> UpdateService(int id, [FromForm] MetalServiceDto dto, CancellationToken ct)
        {
            await service.UpdateServiceAsync(id, dto, ct);
            return Ok(new { success = true });
        }

        [HttpDelete("services/{id:int}")]
        public async Task<IActionResult> DeleteService(int id, CancellationToken ct)
        {
            await service.DeleteServiceAsync(id, ct);
            return Ok(new { success = true });
        }

        [HttpGet("unliquid")]
        public async Task<IActionResult> GetUnliquid(CancellationToken ct)
            => Ok(await service.GetUnliquidProductsAsync(ct));

        [HttpPost("unliquid")]
        public async Task<IActionResult> CreateUnliquid([FromForm] UnliquidProductDto dto, CancellationToken ct)
            => Ok(await service.CreateUnliquidAsync(dto, ct));

        [HttpPut("unliquid/{id:int}")]
        public async Task<IActionResult> UpdateUnliquid(int id, [FromForm] UnliquidProductDto dto, CancellationToken ct)
        {
            await service.UpdateUnliquidAsync(id, dto, ct);
            return Ok(new { success = true });
        }

        [HttpDelete("unliquid/{id:int}")]
        public async Task<IActionResult> DeleteUnliquid(int id, CancellationToken ct)
        {
            await service.DeleteUnliquidAsync(id, ct);
            return Ok(new { success = true });
        }
    }
}