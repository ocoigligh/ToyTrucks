using System;
using System.Linq;
using System.Threading.Tasks;
using ToyTrucks.Web.Models;
using ToyTrucks.Web.Services;
using Microsoft.AspNetCore.Mvc;
using ToyTrucks.Web.Models.View;
using ToyTrucks.Web.Extensions;
using ToyTrucks.Web.Models.Api;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ToyTrucks.Web.Controllers
{
    public class ShoppingBasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly ICatalogService _catalogService;
        private readonly Settings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ShoppingBasketController(IBasketService basketService,
            Settings settings, ICatalogService catalogService, IHttpContextAccessor httpContextAccessor)
        {
            _basketService = basketService;
            _settings = settings;
            _catalogService = catalogService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var basketViewModel = await CreateBasketViewModel();

            return View(basketViewModel);
        }

        private async Task<BasketViewModel> CreateBasketViewModel()
        {
            var basketId = Request.Cookies.GetCurrentBasketId(_settings);
            CustomerBasket basket = await _basketService.GetBasket(basketId);

            List<BasketLineViewModel> lineViewModels = new List<BasketLineViewModel>();
            foreach (var bl in basket.Items)
            {
                var truckInventory = await _catalogService.GetTruckInventory(bl.TruckId);

                lineViewModels.Add(new BasketLineViewModel
                {
                    LineId = bl.Id,
                    TruckId = bl.TruckId,
                    Name = bl.Name,
                    Price = bl.Price,
                    Year = bl.Year,
                    Quantity = bl.Quantity,
                    DefaultPhotoPath = bl.DefaultPhotoPath,
                    TruckQuantity = truckInventory.Quantity,
                    OutOfStock = bl.OutOfStock,
                    StockDecreased = bl.StockDecreased

                });
            }

            var basketViewModel = new BasketViewModel
            {
                BasketLines = lineViewModels
            };
            basketViewModel.ShoppingCartTotal = basketViewModel.BasketLines.Where(bl => bl.OutOfStock == false).Sum(bl => bl.Price * bl.Quantity);

            return basketViewModel;
        }


        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLine(BasketItem basketItem)
        {
            if (basketItem == null)
            {
                HandleException(new ArgumentNullException(nameof(basketItem)));
                return RedirectToAction("Index");
            }
            var basketId = Request.Cookies.GetCurrentBasketId(_settings);

            await _basketService.AddLine(basketId, basketItem);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateLine(BasketLineForUpdate basketLineForUpdate)
        {
            if (string.IsNullOrWhiteSpace(basketLineForUpdate.LineId))
            {
                HandleException(new ArgumentNullException(nameof(basketLineForUpdate.LineId)));
            }
            else
            {
                var basketId = Request.Cookies.GetCurrentBasketId(_settings);
                await _basketService.UpdateLine(basketId, basketLineForUpdate);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveLine(string lineId)
        {
            if (string.IsNullOrWhiteSpace(lineId))
            {
                HandleException(new ArgumentNullException(nameof(lineId)));
            }
            else
            {
                var basketId = Request.Cookies.GetCurrentBasketId(_settings);
                await _basketService.RemoveLine(basketId, lineId);
                if (!await _basketService.HasLineItems(basketId))
                {
                    return RedirectToAction("Index", "TruckCatalog");
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(BasketCheckoutViewModel basketCheckoutViewModel)
        {
            try
            {
                var basketId = Request.Cookies.GetCurrentBasketId(_settings);
                if (ModelState.IsValid)
                {
                    var basketForCheckout = new BasketForCheckout
                    {
                        FirstName = basketCheckoutViewModel.FirstName,
                        LastName = basketCheckoutViewModel.LastName,
                        Email = basketCheckoutViewModel.Email,
                        Address = basketCheckoutViewModel.Address,
                        Address2 = basketCheckoutViewModel.Address2,
                        City = basketCheckoutViewModel.City,
                        State = basketCheckoutViewModel.State,
                        ZipCode = basketCheckoutViewModel.ZipCode,
                        Country = basketCheckoutViewModel.Country,
                        CardNumber = basketCheckoutViewModel.CardNumber,
                        CardName = basketCheckoutViewModel.CardName,
                        CardExpiration = basketCheckoutViewModel.CardExpiration,
                        CvvCode = basketCheckoutViewModel.CvvCode,
                        BasketId = basketId,
                        UserId = _settings.UserId
                        //UserId = 
                    };

                    await _basketService.Checkout(basketId, basketForCheckout);

                    // clear the basket Id, as it's been removed from the cache on checkout
                    Response.Cookies.Delete(_settings.BasketIdCookieName);

                    return RedirectToAction("CheckoutComplete");
                }

                return View(basketCheckoutViewModel);
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = e.Message;
                return View(basketCheckoutViewModel);
            }
        }

        public IActionResult CheckoutComplete()
        {
            return View();
        }

        private void HandleException(Exception ex)
        {
            ViewBag.BasketInoperativeMsg = $"Basket Service is inoperative {ex.GetType().Name} - {ex.Message}";
        }
    }
}