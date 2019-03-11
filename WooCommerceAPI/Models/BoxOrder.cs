using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Models
{
    public class BoxOrder
    {
        [BsonElement("item_1")]
        public string Item1 { get; set; }

        [BsonElement("item_2")]
        public string Item2 { get; set; }

        [BsonElement("item_3")]
        public string Item3 { get; set; }

        [BsonElement("item_4")]
        public string Item4 { get; set; }

        [BsonElement("item_5")]
        public string Item5 { get; set; }

        [BsonElement("item_6")]
        public string Item6 { get; set; }

        [BsonElement("item_7")]
        public string Item7 { get; set; }

        [BsonElement("item_8")]
        public string Item8 { get; set; }
    }

    public class BoxOrderDTO : BoxOrder
    {
        [BsonId]
        [BsonElement("box_order_id")]
        public string BoxOrderID { get; set; }
    }
}
