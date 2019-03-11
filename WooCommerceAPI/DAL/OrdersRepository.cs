using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WooCommerceAPI.Models;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v2;
using MongoDB.Driver;
using System.IO;
using MongoDB.Bson.IO;
using System.Text;
using OrdersAPI.Models;

namespace WooCommerceAPI.DAL
{
    public class OrdersRepository : IOrdersRepository
    {

        private readonly IMongoCollection<OrderDTO> _order;

        // When Woo Becomes Avaliable
        /**
        string Trimstr(string json)
        {
            return json.Trim(new char[] { '\uFEFF', '\u200B' });
        }
        */

        /**
        public OrdersRepository(IOptions<ConnectWC> wcOptions, IOptions<MongoSettings> mOptions)
        {
            RestAPI rest = new RestAPI("http://www.meatbox.com/wp-json/wc/v2/", options.Value.Key1,options.Value.Key2);
            wc = new WCObject(rest);

            Controller containing connection to MongoDB
            var client = new MongoClient(mOptions.Value.ConnectionString);
            var db = client.GetDatabase(mOptions.Value.Database);
            _order = db.GetCollection<Order>("meatbox_orders");
        }
           */

        public OrdersRepository (IOptions<MongoDBSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.Database);
            _order = db.GetCollection<OrderDTO>("orders");
        }

        public List<OrderDTO> GetOrders()
        {
            return _order.Find(_ => true).ToList();
        }

        public List<OrderDTO> GetLateOrders()
        {
           
            var today = DateTime.Now;
            var lateDay = today.AddDays(-7);
            return _order.Find(x => x.Order_Created < lateDay).ToList();
        }

        public string CreateOrderDocument(OrderDTO newOrderDTO)
        {
            try
            {
                _order.InsertOne(newOrderDTO);
                return "New item Inserted, Stock ID: " + newOrderDTO.Id.ToString();
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
                _order.DeleteOneAsync(x => x.Status == "Completed");
                return "Completed Orders Cleared";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ModifyOrderStatus(string orderID, string statusType)
        {
            OrderDTO modifiedOrder = _order.Find(OrderDTO => OrderDTO.Id == orderID).FirstOrDefault();
            modifiedOrder.Status = statusType;
            _order.ReplaceOneAsync(x => x.Id == modifiedOrder.Id, modifiedOrder);
            return "Stock status has been adjusted.";
        }

        public OrderDTO GetOrder(string orderID)
        {
            return _order.Find(OrderDTO => OrderDTO.Id == orderID).FirstOrDefault();
        }

        public string AssignOrderItems(string orderID, List<StockCopyDTO> orderItems, OrderDTO orderObject)
        {
            OrderDTO modifiedOrder = _order.Find(OrderDTO => OrderDTO.Id == orderID).FirstOrDefault();
            modifiedOrder.ItemOneID = orderItems[0].Product_Title;
            modifiedOrder.ItemTwoID = orderItems[1].Product_Title;
            modifiedOrder.ItemThreeID = orderItems[2].Product_Title;
            modifiedOrder.ItemFourID = orderItems[3].Product_Title;
            modifiedOrder.ItemFiveID = orderItems[4].Product_Title;
            modifiedOrder.ItemSixID = orderItems[5].Product_Title;
            if (orderItems.Count > 6)
            {
                modifiedOrder.ItemSevenID = orderItems[6].Product_Title;
            }
            if (orderItems.Count > 7)
            {
                modifiedOrder.ItemEightID = orderItems[7].Product_Title;
            }
            _order.ReplaceOneAsync(x => x.Id == modifiedOrder.Id, modifiedOrder);
            return "Order record has been updated with allocated items";
        }
    }
}
