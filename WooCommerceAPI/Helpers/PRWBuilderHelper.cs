using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Helpers
{
    public class PRWBuilderHelper
    {
        // Build exception messages 
        public ProviderResponseWrapperCopy PRWBuilder(string json, HTTPResponseCodes httpResponse)
        {
            ProviderResponseWrapperCopy response = new ProviderResponseWrapperCopy
            {
                ResponseMessage = json,
                ResponseHTMLType = httpResponse
            };
            return response;
        }
    }
}
