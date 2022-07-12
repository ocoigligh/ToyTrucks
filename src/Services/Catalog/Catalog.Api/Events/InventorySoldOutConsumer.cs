
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;

using ToyTrucks.Catalog.Api.Services;

using ToyTrucks.Messaging.Events;
using MassTransit;

namespace ToyTrucks.Catalog.Api.Events
{
    public class InventorySoldOutConsumer : IConsumer<InventorySoldOutEvent>
    {
        private readonly ITruckRepository _service;
        private readonly IMapper _mapper;
        private readonly ILogger<InventorySoldOutConsumer> _logger;

        public InventorySoldOutConsumer(IMapper mapper, ILogger<InventorySoldOutConsumer> logger, ITruckRepository service)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task Consume(ConsumeContext<InventorySoldOutEvent> context)
        {
            System.Console.WriteLine($"IN Catalog InventorySoldOutConsumer with {context.Message}");
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            Guid truckId = context.Message.TruckId;
            var truck = await _service.GetTruckById(truckId);
            if (truck != null)
            {
                truck.OutOfStock = true;
                await _service.UpdateTruck(truck);
            }
            else
            {
                _logger.LogError($"Truck doesn't exist for id of {truckId}");
            }

        }
    }
}