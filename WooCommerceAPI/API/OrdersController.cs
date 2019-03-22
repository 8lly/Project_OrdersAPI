using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Models;
using StockAPI.Models;
using WooCommerceAPI.BLL;
using WooCommerceAPI.Models;

namespace WooCommerceAPI.Controllers
{

    [Route("api/[controller]")]
    public class WooController : Controller
    {
        private readonly IOrdersProvider _ordersProvider;

        // Create instance of Gather Orders Provider
        public WooController(IOrdersProvider op)
        {
            _ordersProvider = op;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Index")]
        public IActionResult Index()
        {
            return Redirect($"{Request.Scheme}://{Request.Host}/swagger"); //Home page is Swagger doc
        }

        [HttpGet]
        [Route("GatherLateOrders")]
        public string GetLateOrders()
        {
            try
            {
                return _ordersProvider.GetLateOrders();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // GET: ALL ORDER DOCUMENTS
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("GetOrders")]
        public JsonResult GetOrders()
        {
            try
            {
               ProviderResponseWrapperCopy providerResponse =  _ordersProvider.GetOrders();

                // If stock was successfully sent back
                if (providerResponse.ResponseType == 1)
                {
                    JsonResult okJsonResult = new JsonResult(providerResponse.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 200
                    };
                    return okJsonResult;
                }
                // If stock was not found
                else if (providerResponse.ResponseType == 2)
                {
                    JsonResult userInvalidJsonResult = new JsonResult(providerResponse.ResponseMessage)

                    {
                        ContentType = "application/json",
                        StatusCode = 400
                    };
                    return userInvalidJsonResult;
                }
                else
                {
                    JsonResult serverInvalidJsonResult = new JsonResult(providerResponse.ResponseMessage)

                    {
                        ContentType = "application/json",
                        StatusCode = 500
                    };
                    return serverInvalidJsonResult;
                }
            } 
            catch (Exception ex)
            {
                JsonResult unavaliableJsonResult = new JsonResult(ex.ToString())
                {
                    ContentType = "application/json",
                    StatusCode = 500
                };
                return unavaliableJsonResult;
            }
        }

        [HttpGet]
        [Route("GetOrder")]
        public string GetOrder(string orderID)
        {
            try
            {
                return _ordersProvider.GetOrder(orderID);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost]
        [Route("ImportOrder")]
        public JsonResult CreateOrderDocument([FromBody] Order newOrder)
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse = _ordersProvider.CreateOrderDocument(newOrder);
                
                if (providerResponse.ResponseType == 1)
                {
                    JsonResult okJsonResult = new JsonResult(providerResponse.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 200
                    };
                    return okJsonResult;
                }
                else if (providerResponse.ResponseType == 2)
                {
                    JsonResult userErrorJsonResult = new JsonResult(providerResponse.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 400
                    };
                    return userErrorJsonResult;
                }
                else
                {
                    JsonResult serverErrorJsonResult = new JsonResult(providerResponse.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 500
                    };
                    return serverErrorJsonResult;
                }
            }
            catch (Exception ex)
            {
                JsonResult apiErrorJsonResult = new JsonResult(ex.ToString())
                {
                    ContentType = "application/json",
                    StatusCode = 500
                };
                return apiErrorJsonResult;
            }
        }

        // POST: ASSIGN ITEMS TO ORDER 
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost]
        [Route("AssignOrderItems")]
        public async Task<JsonResult> AssignOrderItems(string orderID)
        {
            try
            {
                string jsonOrder = GetOrder(orderID);
                //  GET response from StockAPI with OrderFullfillment Stock
                ProviderResponseWrapperCopy response = await _ordersProvider.BoxOrderCreateAsync(orderID);
                string jsonBoxOrderCreate = response.ResponseMessage;

                if (response.ResponseType == 1)
                {
                    ProviderResponseWrapperCopy providerResponseAsProviderResponseWrapperCopy = _ordersProvider.AssignOrderItems(orderID, jsonOrder, jsonBoxOrderCreate);

                    // If stock is successfully added to order 
                    if (providerResponseAsProviderResponseWrapperCopy.ResponseType == 1)
                    {
                        JsonResult okJsonResult = new JsonResult(response.ResponseMessage)
                        {
                            ContentType = "application/json",
                            StatusCode = 200
                        };
                        return okJsonResult;
                    }
                    else if (providerResponseAsProviderResponseWrapperCopy.ResponseType == 3)
                    {
                        JsonResult userInvalidJsonResult = new JsonResult(response.ResponseMessage)
                        {
                            ContentType = "application/json",
                            StatusCode = 500
                        };
                        return userInvalidJsonResult;
                    }
                }
                // If orderfullfillmentstock returns client error
                else if (response.ResponseType == 2)
                {
                    JsonResult clientJsonErrorResult = new JsonResult(response.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 300
                    };
                    return clientJsonErrorResult;
                }
                // If orderfullfillment is interfered with back-end issues 
                else if (response.ResponseType == 3)
                {
                    JsonResult serverJsonErrorResult = new JsonResult(response.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 500
                    };
                    return serverJsonErrorResult;
                }
                throw new Exception();
            }
            catch (Exception ex)
            {
                JsonResult failedJsonResult = new JsonResult(ex.ToString())
                {
                    ContentType = "application/json",
                    StatusCode = 500
                };
                return failedJsonResult;
            }
        }

        [HttpPut]
        [Route("ModifyOrder")]
        public string ModifyOrderStatus(string orderID, string statusType)
        {
            try
            {
                return _ordersProvider.ModifyOrderStatus(orderID, statusType);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpDelete]
        [Route("RemoveCompletedOrders")]
        public string RemoveCompletedOrders()
        {
            try
            {
                return _ordersProvider.RemoveCompletedOrders();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpDelete]
        [Route("RemoveOrder")]
        public string RemoveOrder(string orderID)
        {
            try
            {
                string reallocatedStock = _ordersProvider.RemoveOrder(orderID);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }        
    }
}
