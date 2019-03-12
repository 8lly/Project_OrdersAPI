using OrdersAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WooCommerceAPI.BLL
{
    public interface IOrdersProvider
    {
        string GetOrders();
        string GetLateOrders();
        string CreateOrderDocument(Order newOrder);
        string RemoveCompletedOrders();
        string ModifyOrderStatus(string orderID, string statusType);
        string BoxOrderCreate(string orderID);
        string AssignOrderItems(string orderID, string jsonOrder, string jsonBoxOrderCreate);
        string GetOrder(string orderID);
    }
}
