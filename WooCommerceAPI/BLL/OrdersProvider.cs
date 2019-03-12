using Newtonsoft.Json;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
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
    }
}
