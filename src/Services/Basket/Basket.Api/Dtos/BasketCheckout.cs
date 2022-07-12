using System;
namespace ToyTrucks.Basket.Api.Dtos
{
    public class BasketCheckout
    {
        public string BasketId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalPrice { get; set; }

        // BillingAddress
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        // Payment
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiration { get; set; }
        public string CvvCode { get; set; }
        public int PaymentMethod { get; set; }
    }
}