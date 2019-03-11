using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Connection to woo
// TODO: Replace wording with more cryptic explanation 

namespace WooCommerceAPI.Models
{
    public class ConnectWC
    {
        public string StoreURL { get; set; }
        public string Key1 { get; set; }
        public string Key2 { get; set; }
    }
}
