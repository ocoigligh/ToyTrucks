using System.Threading.Tasks;
using ToyTrucks.Web.Models.Api;

namespace ToyTrucks.Web.Services
{
    public interface IBasketService
    {
        Task<CustomerBasket> GetBasket(string userId);
        Task<CustomerBasket> AddLine(string basketId, BasketItem basketItem);
        Task<bool> HasLineItems(string basketId);
        Task DeleteBasket(string userId);
        Task UpdateLine(string basketId, BasketLineForUpdate basketLineForUpdate);
        Task<CustomerBasket> RemoveLine(string basketId, string lineId);
        Task<BasketForCheckout> Checkout(string basketId, BasketForCheckout basketForCheckout);
    }
}