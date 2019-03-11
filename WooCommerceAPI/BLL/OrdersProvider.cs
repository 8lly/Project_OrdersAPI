using Newtonsoft.Json;
using OrdersAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            int itemCount = SKUPropertyItemCount(selectedOrder.SKU);
            string sku = selectedOrder.SKU;

            Random random = new Random();
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.GetAsync("http://localhost:55001/api/Stock/OrderFulfillmentStock?skuID=" + sku).Result;
            string responseMessage = response.Content.ReadAsStringAsync().Result;
            List<StockCopyDTO> validStock = JsonConvert.DeserializeObject<List<StockCopyDTO>>(responseMessage);

            if (validStock.Count < itemCount)
               {
                 return "Not enough stock is eligible to fulfill the order";
               }
            else
               {
                // Select random items to fill order
                while (validStock.Count != itemCount)
                {
                int randItem = random.Next(0, validStock.Count);
                validStock.RemoveAt(randItem);
                }
            }

            string json = JsonConvert.SerializeObject(validStock);
            return json;
        }

        public int SKUPropertyItemCount(string SKU)
        {
            if (SKU == "SKU000001")
            {
                return 6;
            }
            else if (SKU == "SKU000002")
            {
                return 6;
            }
            else if (SKU == "SKU000003")
            {
                return 8;
            }
            else if (SKU == "SKU000004")
            {
                return 6;
            }
            else if (SKU == "SKU000005")
            {
                return 8;
            }
            else if (SKU == "SKU000006")
            {
                return 8;
            }
            // TODO: Better way to do this
            else
            {
                return 0;
            }
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

        public string UpdateAllocatedStock(string jsonBoxOrderCreate)
        {
            try
            {
                OrderDTO orderObject = JsonConvert.DeserializeObject<OrderDTO>(jsonBoxOrderCreate);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
