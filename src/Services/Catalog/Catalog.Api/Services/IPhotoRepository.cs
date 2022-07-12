using ToyTrucks.Catalog.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToyTrucks.Catalog.Api.Services
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> GetPhotosByTruckId(Guid truckId);
        Task AddPhoto(Photo photo);
        void UpdatePhoto(Photo photo);
        Task<bool> SaveChanges();
    }
}
