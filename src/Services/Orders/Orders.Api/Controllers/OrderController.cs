using Microsoft.AspNetCore.Mvc;
//using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToyTrucks.Orders.Api.Entities;
using System.Net;
using AutoMapper;
using ToyTrucks.Orders.Api.Services;
using ToyTrucks.Orders.Api.Models;

using Microsoft.Extensions.Logging;

namespace ToyTrucks.Orders.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        // private readonly IMediator _mediatr;
        private readonly IOrdersRepository _service;
        private IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrdersRepository service, IMapper mapper, ILogger<OrderController> logger)
        {

            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> OrdersByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest();
            var orders = await _service.GetOrdersByUserId(userId);
            var ordersToReturn = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(ordersToReturn);
        }

        [HttpGet("{orderId}", Name = "OrderByOrderId")]
        [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrderDto>> OrderByOrderId(Guid orderId)
        {
            if (orderId == Guid.Empty)
                return BadRequest();
            var order = await _service.GetByOrderIdAsync(orderId);
            var orderToReturn = _mapper.Map<OrderDto>(order);
            orderToReturn.OrderLines = _mapper.Map<List<OrderLineDto>>(order.OrderItems);
            return Ok(orderToReturn);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> AllOrders()
        {
            // var query = new GetOrdersListQuery(userName);
            //var orders = await _mediatr.Send(query);
            var orders = await _service.GetOrdersAsync();
            var ordersToReturn = _mapper.Map<IEnumerable<OrderDto>>(orders);
            return Ok(ordersToReturn);
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<int>> UpdateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
                return BadRequest();

            var order = _mapper.Map<Order>(orderDto);
            try
            {
                await _service.UpdateOrderAsync(order);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with UserId of {order.UserId} as it does not exist");
                return NotFound();
            }

            //await _mediatr.Send(order);
            return NoContent();
        }

        [HttpDelete("{orderId:alpha}", Name = "DeleteOrder")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<int>> DeleteOrder(Guid orderId)
        {
            if (orderId == Guid.Empty)
                return BadRequest();
            try
            {
                await _service.DeleteOrderAsync(orderId);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with id of {orderId} as it does not exist");
                return NotFound();
            }
            return NoContent();
        }
    }
}