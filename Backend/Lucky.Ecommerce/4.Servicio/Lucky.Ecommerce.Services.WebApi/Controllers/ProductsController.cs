using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Lucky.Ecommerce.Application.Dto;
using Lucky.Ecommerce.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Lucky.Ecommerce.Services.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IProductsApplication _productsApplication;
        private readonly ILogger<ProductsController> _logger;
 
        public ProductsController(IProductsApplication productsApplication, ILogger<ProductsController> logger)
        {
            _productsApplication = productsApplication;
            _logger = logger;

        }

        #region Métodos Asincronos
        [Authorize]
        [HttpPost("async")]
        public async Task<IActionResult> InsertAsync([FromBody] ProductsDto productsDto)
        {
            if (productsDto == null)
            {
                return BadRequest();
            }
            var response = await _productsApplication.InsertAsync(productsDto);
            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpPut("async")]
        public async Task<IActionResult> UpdateAsync([FromBody] ProductsDto productsDto)
        {
            if (productsDto == null)
            {
                return BadRequest();
            }
            var response = await _productsApplication.UpdateAsync(productsDto);
            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpDelete("async/{productId}")]

        public async Task<IActionResult> DeleteAsync(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest(new { message = "El ProductId debe ser un número válido mayor que 0." });
            }

            try
            {
                var response = await _productsApplication.DeleteAsync(productId);

                if (!response.IsSuccess)
                {
                    _logger.LogError("Error al eliminar el producto con ID {ProductId}: {Message}", productId, response.Message);
                    return StatusCode(500, new { message = "Error interno al eliminar el producto." });
                }

                if (response.Data == false) // Producto no existe o ya fue eliminado
                {
                    _logger.LogWarning("Intento de eliminar un producto inexistente. ID: {ProductId}", productId);
                    return NotFound(new { message = "El producto no existe o ya ha sido eliminado." });
                }

                _logger.LogInformation("Producto eliminado correctamente. ID: {ProductId}", productId);
                return Ok(new { message = "Producto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al intentar eliminar el producto con ID {ProductId}", productId);
                return StatusCode(500, new { message = "Error inesperado al procesar la solicitud." });
            }
        }

        [Authorize]
        [HttpGet("async/{productId}")]
        public async Task<IActionResult> GetAsync(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest(new { message = "El ProductId debe ser un número válido mayor que 0." });
            }
            var response = await _productsApplication.GetAsync(productId);
            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response.Message);
        }

        [Authorize]
        [HttpGet("async")]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = await _productsApplication.GetAllAsync();
            if (response.IsSuccess)
                return Ok(response);

            return BadRequest(response.Message);
        }

        #endregion

    }
}
