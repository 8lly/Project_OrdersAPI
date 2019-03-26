using Microsoft.AspNetCore.Mvc;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersAPI.Helpers
{
    public class APIJsonResultResponseHelper
    { 
        public JsonResult CreateJsonResultResponse(ProviderResponseWrapperCopy providerResponse)
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
