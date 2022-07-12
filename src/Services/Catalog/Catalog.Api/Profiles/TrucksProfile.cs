using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ToyTrucks.Catalog.Api.Entities;

namespace ToyTrucks.Catalog.Api.Profiles
{
    public class TrucksProfile : Profile
    {
        public TrucksProfile()
        {
            CreateMap<Truck, Models.TruckDto>();
            CreateMap<Models.TruckDto, Truck>()
                 .ReverseMap();
        }
    }
}
