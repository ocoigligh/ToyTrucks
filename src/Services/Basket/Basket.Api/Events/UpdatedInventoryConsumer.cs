using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using ToyTrucks.Messaging.Events;
using ToyTrucks.Basket.Api.Services;
using AutoMapper;

namespace ToyTrucks.Basket.Api.Events
{
    public class UpdatedInventoryConsumer : IConsumer<UpdatedInventory>
    {

        private readonly ILogger<UpdatedInventoryConsumer> _logger;
        private readonly IBasketRepository _service;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public UpdatedInventoryConsumer(ILogger<UpdatedInventoryConsumer> logger, IBasketRepository service, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Consume(ConsumeContext<UpdatedInventory> context)
        {
            if (context == null)
            {
                _logger.LogWarning("Failed to consume updated inventory as context is null");
                return;
            }
            var updatedInventory = (UpdatedInventory)context.Message;

            var baskets = await _service.GetBasketsWithTruck(updatedInventory.TruckId);
            foreach (var basket in baskets)
            {
                var item = basket.Items.First(truck => truck.TruckId == updatedInventory.TruckId);
                if (item != null)
                {
                    if (updatedInventory.NewQuantity == 0)
                    {
                        item.OutOfStock = true;
                        await _service.UpdateBasketAsync(basket);
                        _logger.LogInformation($"Truck {item.Name} out of stock for basket {basket.UserId}");
                    }
                    else if (item.Quantity > updatedInventory.NewQuantity)
                    {
                        item.Quantity = updatedInventory.NewQuantity;
                        item.StockDecreased = true;
                        await _service.UpdateBasketAsync(basket);
                        _logger.LogInformation($"Truck {item.Name} decreased stock to {item.Quantity} for basket {basket.UserId}");
                    }
                }
                else{
                    _logger.LogInformation($"Truck {item.Name} is not in other baskets");
                }
            }
        }
    }
}