using OrdersAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrdersAPI.Wrapper;

namespace WooCommerceAPI.BLL
{
    public interface IOrdersProvider
    {
        ProviderResponseWrapper GetOrders();
        ProviderResponseWrapper GetLateOrders();
        ProviderResponseWrapper CreateOrderDocument(Order newOrder);
        ProviderResponseWrapper RemoveCompletedOrders();
        ProviderResponseWrapper ModifyOrderStatus(string orderID, string statusType);
        Task<ProviderResponseWrapper> BoxOrderCreateAsync(string orderID);
        ProviderResponseWrapper AssignOrderItems(string orderID, string jsonOrder, string jsonBoxOrderCreate);
        ProviderResponseWrapper GetOrder(string orderID);
        Task<ProviderResponseWrapper> RemoveOrder(string orderID);
        Task<ProviderResponseWrapper> ReallocatedRemovedOrderStock(List<string> reallocatedStock);
    }
}
