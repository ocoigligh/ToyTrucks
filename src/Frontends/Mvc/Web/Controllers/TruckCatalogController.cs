using System;
using System.Threading.Tasks;
using ToyTrucks.Web.Models;
using ToyTrucks.Web.Extensions;
using ToyTrucks.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ToyTrucks.Web.Models.View;
using Microsoft.AspNetCore.Http;

namespace ToyTrucks.Web.Controllers
{
    public class TruckCatalogController : Controller
    {
        public const string SessionCategoryName = "Category_Truck";
        public string SessionInfo_Category { get; private set; }
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly Settings _settings;

        public TruckCatalogController(ICatalogService service, IBasketService basketService, Settings settings)
        {
            _catalogService = service;
            _basketService = basketService;
            _settings = settings;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var currentBasketId = Request.Cookies.GetCurrentBasketId(_settings);
            if (!categoryId.HasValue && !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionCategoryName)))
            {
                var result = int.MinValue;
                if (int.TryParse(HttpContext.Session.GetString(SessionCategoryName), out result))
                    categoryId = result;
            }

            var getTrucks = (categoryId.HasValue) ? _catalogService.GetTrucksByCategoryId(categoryId.Value) : _catalogService.GetTrucks();
            var getBasket = _basketService.GetBasket(currentBasketId);
            var getCategories = _catalogService.GetCategories();

            await Task.WhenAll(new Task[] { getTrucks, getCategories, getBasket });
            var numberOfItems = getBasket.Result?.Items.Count ?? 0;
            var tm = new TruckListModel
            {
                Trucks = getTrucks.Result,
                Categories = getCategories.Result,
                NumberOfItems = numberOfItems,
                SelectedCategory = categoryId.HasValue ? categoryId.Value : null
            };
            return View(tm);
        }

        [HttpPost]
        public IActionResult SelectCategory(int? selectedCategory)
        {
            HttpContext.Session.SetString(SessionCategoryName, selectedCategory.ToString());
            return RedirectToAction("Index", new { categoryId = selectedCategory });
        }

        public async Task<IActionResult> Detail(Guid truckId)
        {
            var truck = await _catalogService.GetTruckById(truckId);
            return View(new TruckDetailViewModel { Truck = truck });
        }
    }
}