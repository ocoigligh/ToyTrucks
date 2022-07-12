using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using ToyTrucks.Messaging.Events;
using ToyTrucks.Catalog.Api.Services;
using ToyTrucks.Catalog.Api.Models;
using ToyTrucks.Catalog.Api.Entities;
using AutoMapper;

namespace ToyTrucks.Catalog.Api.Events
{
    public class InventoryToUpdateConsumer : IConsumer<InventoryToUpdate>
    {
        private readonly ILogger<InventoryToUpdateConsumer> _logger;
        private readonly ITruckRepository _service;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        public InventoryToUpdateConsumer(ILogger<InventoryToUpdateConsumer> logger, IMapper mapper, IPublishEndpoint publishEndpoint, ITruckRepository service)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task Consume(ConsumeContext<InventoryToUpdate> context)
        {
            if (context == null)
            {
                _logger.LogWarning("Failed to consume Inventory update as context is null");
                return;
            }
            TruckInventory tid = _mapper.Map<TruckInventory>(context.Message);
            if (await _service.UpdateTruckInventory(tid))
            {
                TruckInventory newDetails = await _service.GetTruckInventory(tid.TruckId);

                var eventMessage = new UpdatedInventory();
                eventMessage.TruckId = newDetails.TruckId;
                // eventMessage.TruckName = newDetails.TruckName;
                eventMessage.NewQuantity = newDetails.Quantity;
                _logger.LogInformation($"Updated Catalog. New qty for {eventMessage.TruckId} is {eventMessage.NewQuantity}.  Name is {eventMessage.TruckName}");
                await _publishEndpoint.Publish(eventMessage);
            }
        }
    }
}