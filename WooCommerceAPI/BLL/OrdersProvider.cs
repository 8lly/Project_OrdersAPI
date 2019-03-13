using Newtonsoft.Json;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WooCommerceAPI.DAL;

namespace WooCommerceAPI.BLL
{
    public class OrdersProvider : IOrdersProvider
    {

        // Repository obj
        private readonly IOrdersRepository _ordersRepository;

        public OrdersProvider(IOrdersRepository gop)
        {
            _ordersRepository = gop;
        }

        public string GetOrders()
        {
            try
            {
                var allOrders = _ordersRepository.GetOrders();
                string json = JsonConvert.SerializeObject(allOrders);
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetLateOrders()
        {
            try
            {
                List<OrderDTO> lateOrders = _ordersRepository.GetLateOrders();
                string json = JsonConvert.SerializeObject(lateOrders);
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CreateOrderDocument(Order newOrder)
        {
            OrderDTO newOrderDTO = new OrderDTO
            {
                Order_Number = newOrder.Order_Number,
                Customer_First = newOrder.Customer_First,
                Customer_Last = newOrder.Customer_Last,
                SKU = newOrder.SKU,
                Status = newOrder.Status,
                Order_Created = DateTime.Now
            };

            try
            {
                return _ordersRepository.CreateOrderDocument(newOrderDTO);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string RemoveCompletedOrders()
        {
            try
            {
                return _ordersRepository.RemoveCompletedOrders();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ModifyOrderStatus(string orderID, string statusType)
        {
            try
            {
                return _ordersRepository.ModifyOrderStatus(orderID, statusType);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string BoxOrderCreate(string orderID)
        {

            OrderDTO selectedOrder = _ordersRepository.GetOrder(orderID);
            string sku = selectedOrder.SKU;

            HttpClient client = new HttpClient();
            string uri = "http://localhost:55001/api/Stock/OrderFulfillmentStock?orderSKU=" + sku;

            HttpResponseMessage response = client.GetAsync(uri).Result;
            string responseMessage = response.Content.ReadAsStringAsync().Result;
            List<StockCopyDTO> validStock = JsonConvert.DeserializeObject<List<StockCopyDTO>>(responseMessage);


            string json = JsonConvert.SerializeObject(validStock);
            return json;
        }
 
        public string AssignOrderItems(string orderID, string jsonOrder, string jsonBoxOrderCreate)
        {
            List<StockCopyDTO> orderItems = JsonConvert.DeserializeObject<List<StockCopyDTO>>(jsonBoxOrderCreate);
            OrderDTO orderObject = JsonConvert.DeserializeObject<OrderDTO>(jsonOrder);
            return _ordersRepository.AssignOrderItems(orderID, orderItems, orderObject);
        }

        public string GetOrder(string orderID)
        {
            try
            {
                var order = _ordersRepository.GetOrder(orderID);
                string json = JsonConvert.SerializeObject(order);
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string RemoveOrder(string orderID)
        {
            try
            {
                // Delete record, save copy to return freed stock
                OrderDTO removedOrder = _ordersRepository.RemoveOrder(orderID);
                // Stock items returning to stock db
                List<string> allocatedItems = new List<string>()
                {
                    removedOrder.ItemOneName,
                    removedOrder.ItemTwoName,
                    removedOrder.ItemThreeName,
                    removedOrder.ItemFourName,
                    removedOrder.ItemFiveName,
                    removedOrder.ItemSixName,
                    removedOrder.ItemSevenName,
                    removedOrder.ItemEightName,
                };

                // Remove nulls from list 
                // allocatedItems.RemoveAll(string.IsNullOrWhiteSpace);

                ReallocatedRemovedOrderStock(allocatedItems);

                string json = JsonConvert.SerializeObject(allocatedItems);
                return json;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task <List<string>> ReallocatedRemovedOrderStock(List<string> reallocatedStockList)
        {

            HttpClient client = new HttpClient();
            string requestURI = "http://localhost:55001/api/Stock/UpdateStockAddAllocation";

            for (int x = 0; x < reallocatedStockList.Count; x++)
            {
                // How to get rid of this?
                var content = new StringContent("");

                HttpResponseMessage response = await client.PostAsync($"{requestURI}/?body={reallocatedStockList[x]}", content);
            }

            return new List<string>();
 
    }
}
}
