using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockAPI.Models
{
    public class StockCopy
    {
        [BsonElement("item_name")]
        public string Product_Title { get; set; }

        [BsonElement("price_per_unit")]
        public Decimal Price_Per_Unit { get; set; }

        [BsonElement("stockist_id")]
        public string Stockist_ID { get; set; }

        [BsonElement("stockist_name")]
        public string Stockist_Name { get; set; }

        [BsonElement("stockist_postcode")]
        public string Stockist_Postcode { get; set; }

        [BsonElement("stockist_contact_number")]
        public string Stockist_Contact_Number { get; set; }

        [BsonElement("use_by_date")]
        public DateTime UBD { get; set; }

        [BsonElement("current_stock")]
        public int Current_Stock { get; set; }

        // stock currently allocated to box's 
        [BsonElement("stock_reserved")]
        public int Stock_Reserved { get; set; }

        [BsonElement("stock_alert")]
        public int Stock_Alert { get; set; }

        [BsonElement("stock_refill_to")]
        public int Stock_Refill_To { get; set; }

        // Set for the 6 different box types avaliable on MeatBox

        [BsonElement("box_type_1")]
        public bool InBoxOne { get; set; }

        [BsonElement("box_type_2")]
        public bool InBoxTwo { get; set; }

        [BsonElement("box_type_3")]
        public bool InBoxThree { get; set; }

        [BsonElement("box_type_4")]
        public bool InBoxFour { get; set; }

        [BsonElement("box_type_5")]
        public bool InBoxFive { get; set; }

        [BsonElement("box_type_6")]
        public bool InBoxSix { get; set; }
    }

    // allows mongodb to set id
    public class StockCopyDTO : StockCopy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("stock_archived")]
        public string Stock_Archived { get; set; }
    }
}
