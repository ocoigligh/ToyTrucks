using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using ToyTrucks.Web.Models;
using ToyTrucks.Web.Models.Api;
using ToyTrucks.Web.Services;
using ToyTrucks.Web.Models.View;

namespace ToyTrucks.Web.Controllers
{

    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly Settings _settings;

        public OrderController(Settings settings, IOrderService orderService)
        {
            _settings = settings;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersForUser(_settings.UserId);
            var numberOfMessages = (from order in orders
                                    where order.Message != null
                                    select order).Count();

            bool hasMessages = numberOfMessages > 0;

            var vm = new OrderListViewModel
            {
                HasMessage = hasMessages,
                Orders = orders
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(Guid orderId)
        {
            var ev = await _orderService.GetOrderDetails(orderId);
            return View(ev);
        }

    }
}