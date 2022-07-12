using AutoMapper;
using ToyTrucks.Basket.Api.Events;
using ToyTrucks.Basket.Api.Dtos;
using ToyTrucks.Messaging.Events;
namespace ToyTrucks.Basket.Api.Profiles
{
    public class AllProfiles : Profile
    {
        public AllProfiles()
        {
            CreateMap<BasketCheckoutEvent, BasketCheckout>()
                .ReverseMap();
            CreateMap<BasketItemEvent, BasketItem>()
           .ReverseMap();
        }
    }
}