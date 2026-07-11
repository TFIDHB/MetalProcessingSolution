using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateService([FromForm] MetalServiceDto dto, CancellationToken ct)
            => Ok(await service.CreateServiceAsync(dto, ct));

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

        [HttpGet("products/{category}")]
        public async Task<IActionResult> GetProducts(string category, CancellationToken ct)
            => Ok(await service.GetProductsByCategoryAsync(category, ct));

        [HttpPost("products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductDto dto, CancellationToken ct)
            => Ok(await service.CreateProductAsync(dto, ct));

        [HttpPut("products/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto dto, CancellationToken ct)
        {
            await service.UpdateProductAsync(id, dto, ct);
            return Ok(new { success = true });
        }

        [HttpDelete("products/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken ct)
        {
            await service.DeleteProductAsync(id, ct);
            return Ok(new { success = true });
        }

        [HttpGet("admin/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStats(CancellationToken ct)
            => Ok(await service.GetStatsAsync(ct));

        [HttpPost("admin/adjust-service-prices")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustServicePrices([FromBody] PriceAdjustmentDto dto, CancellationToken ct)
        {
            await service.AdjustServicePricesAsync(dto, ct);
            return Ok(new { success = true });
        }

        [HttpPost("admin/adjust-product-prices/{category}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustProductPrices(string category, [FromBody] PriceAdjustmentDto dto, CancellationToken ct)
        {
            await service.AdjustProductPricesAsync(category, dto, ct);
            return Ok(new { success = true });
        }
    }
}