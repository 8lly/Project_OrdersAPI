using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Models
{
    public class Order
    {
        [BsonElement("number")]
        public string Order_Number { get; set; }

        [BsonElement("first_name")]
        public string Customer_First { get; set; }

        [BsonElement("last_name")]
        public string Customer_Last { get; set; }

        // Identify order type
        [BsonElement("sku")]
        public string SKU { get; set; }

        [BsonElement("status")]
        public string Status { get; set; }

    }

    public class OrderDTO : Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("order_created")]
        public DateTime Order_Created { get; set; }

        [BsonElement("item_1")]
        public string ItemOneName { get; set; }

        [BsonElement("item_2")]
        public string ItemTwoName { get; set; }

        [BsonElement("item_3")]
        public string ItemThreeName { get; set; }

        [BsonElement("item_4")]
        public string ItemFourName { get; set; }

        [BsonElement("item_5")]
        public string ItemFiveName { get; set; }

        [BsonElement("item_6")]
        public string ItemSixName { get; set; }

        [BsonElement("item_7")]
        public string ItemSevenName { get; set; }

        [BsonElement("item_8")]
        public string ItemEightName { get; set; }

        public bool IsValid()
        {
            if (!String.IsNullOrEmpty(Order_Number) &&
                (!String.IsNullOrEmpty(Customer_First)) &&
                (!String.IsNullOrEmpty(Customer_Last)) &&
                (!String.IsNullOrEmpty(SKU)) &&
                (!String.IsNullOrEmpty(Status)))
            {
                return true;
            }
            return false;
        }
    }
}
