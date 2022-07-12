using System.Collections.Generic;
namespace ToyTrucks.Web.Models.Api
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool IsMiniTruck { get; set; }
        public int CategoryOrder { get; set; }
        public virtual ICollection<Truck> Trucks { get; set; }
    }
}