using AutoMapper;
using ToyTrucks.Messaging.Events;
using MongoDB.Bson;
using ToyTrucks.Orders.Api.Entities;
using ToyTrucks.Orders.Api.Models;

namespace ToyTrucks.Orders.Api.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {

            CreateMap<OrderLineDto, OrderItem>()
                    .ReverseMap();
            CreateMap<Order, OrderDto>()
                    .ReverseMap();
            CreateMap<Order, BasketCheckoutEvent>()
                    .ForMember(d => d.Basket, s => s.MapFrom(a => a.OrderItems))
                    .ReverseMap();
        }
    }
}