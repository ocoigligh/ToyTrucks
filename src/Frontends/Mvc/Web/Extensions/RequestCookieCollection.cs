using System;
using Microsoft.AspNetCore.Http;
using ToyTrucks.Web.Models;

namespace ToyTrucks.Web.Extensions
{
    public static class RequestCookieCollection
    {
        public static string GetCurrentBasketId(this IRequestCookieCollection cookies, Settings settings)
        {
            //Guid.TryParse(cookies[settings.BasketIdCookieName], out Guid baskedId);
            return cookies[settings.BasketIdCookieName];
        }
    }
}