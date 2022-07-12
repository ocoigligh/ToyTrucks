using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ToyTrucks.Catalog.Api.Entities;

namespace ToyTrucks.Catalog.Api.Profiles
{
    public class CategoriesProfile : Profile
    {
        public CategoriesProfile()
        {

            CreateMap<Models.CategoryDto, Category>()
               .ReverseMap();

        }
    }
}
