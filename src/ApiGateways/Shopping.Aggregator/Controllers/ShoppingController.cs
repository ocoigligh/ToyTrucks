using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/shopping")]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService _catalogService;

        public ShoppingController(ICatalogService catalogService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<ActionResult<CategoryModel>> GetCategories()
        {
            var result = await _catalogService.GetCategories();
            return Ok(result);
        }
    }
}