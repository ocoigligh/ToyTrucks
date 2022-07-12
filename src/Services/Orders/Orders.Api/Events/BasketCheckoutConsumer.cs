using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ToyTrucks.Orders.Api.Services;
using ToyTrucks.Orders.Api.Models;
using ToyTrucks.Messaging.Events;
using ToyTrucks.Orders.Api.Helpers;

namespace ToyTrucks.Orders.Api.Entities
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IOrdersRepository _service;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketCheckoutConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly TokenValidationService _tokenValidationService;
        private readonly IConfiguration _config;

        public BasketCheckoutConsumer(IMapper mapper, ILogger<BasketCheckoutConsumer> logger, IOrdersRepository service, IPublishEndpoint publishEndpoint, TokenValidationService tokenValidationService, IConfiguration config)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _tokenValidationService = tokenValidationService;
            _config = config;
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var basketCheckoutDetails = (BasketCheckoutEvent)context.Message;

            
            // if (!await _tokenValidationService.ValidateTokenAsync(basketCheckoutDetails.SecurityContext.AccessToken, basketCheckoutDetails.CreationDateTime))
            // {
            //     _logger.LogError("Token is not valid! Order was not completed.");
            //     return;
            // }
         
            var order = new Order
            {
                UserId = basketCheckoutDetails.UserId,
                OrderPaid = false,
                OrderPlaced = DateTime.Now,
                TotalPrice = basketCheckoutDetails.BasketTotal,
                OrderTotal = (basketCheckoutDetails.Basket != null) ? basketCheckoutDetails.Basket.Count : -1,
            };
            order.OrderId = Guid.NewGuid();
            order.OrderItems = new List<OrderItem>();
            foreach (var bLine in basketCheckoutDetails.Basket)
            {
                OrderItem item = new OrderItem
                {
                    OrderLineId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    Price = bLine.Price,
                    Quantity = bLine.Quantity,
                    TruckId = bLine.TruckId,
                    TruckName = bLine.Name,
                    PhotoPath = bLine.DefaultPhotoPath,

                };
                order.OrderItems.Add(item);
            }
            
            var returnOrder = await _service.AddOrderAsync(order);
            await UpdateInventory(context.Message.Basket);
            _logger.LogInformation($"Order added {returnOrder.OrderId}");
        }

        private async Task UpdateInventory(List<BasketItemEvent> cart)
        {
            if (cart == null || cart.Any() == false)
                return;
            foreach (var item in cart)
            {

                var eventMessage = new InventoryToUpdate();
                eventMessage.TruckId = item.TruckId;
                eventMessage.TruckName = item.Name;
                eventMessage.Quantity = item.Quantity;
                _logger.LogInformation($"trying to update inventory for {eventMessage.TruckName } TruckId = {eventMessage.TruckId} ProductId = {item.Id}");
                await _publishEndpoint.Publish(eventMessage);
                _logger.LogInformation($"Update Inventory event published for Name {eventMessage.TruckName} with new quantity {eventMessage.Quantity}");
            }

        }
    }
}