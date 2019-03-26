﻿using OrdersAPI.Models;
using StockAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WooCommerceAPI.BLL
{
    public interface IOrdersProvider
    {
        ProviderResponseWrapperCopy GetOrders();
        ProviderResponseWrapperCopy GetLateOrders();
        ProviderResponseWrapperCopy CreateOrderDocument(Order newOrder);
        ProviderResponseWrapperCopy RemoveCompletedOrders();
        ProviderResponseWrapperCopy ModifyOrderStatus(string orderID, string statusType);
        Task<ProviderResponseWrapperCopy> BoxOrderCreateAsync(string orderID);
        ProviderResponseWrapperCopy AssignOrderItems(string orderID, string jsonOrder, string jsonBoxOrderCreate);
        ProviderResponseWrapperCopy GetOrder(string orderID);
        Task<ProviderResponseWrapperCopy> RemoveOrder(string orderID);
        Task<ProviderResponseWrapperCopy> ReallocatedRemovedOrderStock(List<string> reallocatedStock);
    }
}
