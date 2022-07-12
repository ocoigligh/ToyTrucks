using System;
using System.ComponentModel.DataAnnotations;

namespace ToyTrucks.Web.Models.Api
{
    public class BasketLineForUpdate
    {
        [Required]
        public string LineId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
