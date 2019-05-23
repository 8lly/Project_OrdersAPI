using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Wrapper;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Assistants
{
    // Create JSONResult message
    public class APIJsonResultResponseHelper
    { 
        public JsonResult CreateJsonResultResponse(ProviderResponseWrapper providerResponse)
        {
            JsonResult jsonResult = new JsonResult(providerResponse.ResponseMessage)
            {
                ContentType = "application/json",
                StatusCode = (int)providerResponse.ResponseHTMLType
            };

            return jsonResult;
        }

    }
}
