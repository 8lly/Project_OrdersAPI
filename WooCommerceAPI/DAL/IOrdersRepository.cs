using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WooCommerceAPI.Models;
using WooCommerceNET.WooCommerce.v2;

namespace WooCommerceAPI.DAL
{
    public interface IOrdersRepository
    {
        List<OrderDTO> GetOrders();
        List<OrderDTO> GetLateOrders();
        string CreateOrderDocument(OrderDTO newOrderDTO);
        string RemoveCompletedOrders();
        string ModifyOrderStatus(string orderID, string statusType);
        OrderDTO GetOrder(string orderID);
        string AssignOrderItems(string orderID, List<StockCopyDTO> orderItems, OrderDTO orderObject);
    }
}
