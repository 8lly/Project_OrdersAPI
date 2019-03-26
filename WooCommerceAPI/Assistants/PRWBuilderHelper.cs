using OrdersAPI.Wrapper;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Assistants
{
    public class PRWBuilderHelper
    {
        // Build exception messages 
        public ProviderResponseWrapper PRWBuilder(string json, HTTPResponseCodes httpResponse)
        {
            ProviderResponseWrapper response = new ProviderResponseWrapper
            {
                ResponseMessage = json,
                ResponseHTMLType = httpResponse
            };
            return response;
        }
    }
}
