using System;

namespace ToyTrucks.Web.Models
{
    public class Settings
    {
        public string BasketIdCookieName => "BasketId";
        public Guid UserId = Guid.Parse("{251acb12-0ce3-430e-babe-490e24fb0ad6}");

    }
}