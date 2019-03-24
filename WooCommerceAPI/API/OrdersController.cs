using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrdersAPI.Helpers;
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
        // How do I initialise this class
        APIJsonResultResponseHelper apiJsonResponse = new APIJsonResultResponseHelper();

        // Create instance of Provider
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

        // GET: ALL LATE ORDER DOCUMENTS
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("GetLateOrders")]
        public JsonResult GetLateOrders()
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse = _ordersProvider.GetLateOrders();

                // If late orders was successfully sent back
                if (providerResponse.ResponseType == 1)
                {
                    JsonResult okJsonResult = new JsonResult(providerResponse.ResponseMessage)
                    {
                        ContentType = "application/json",
                        StatusCode = 200
                    };
                    return okJsonResult;
                }
                // If late orders was not found
                else if (providerResponse.ResponseType == 2)
                {
                    JsonResult userInvalidJsonResult = new JsonResult(providerResponse.ResponseMessage)

                    {
                        ContentType = "application/json",
                        StatusCode = 400
                    };
                    return userInvalidJsonResult;
                }
                // If error with back-end
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
            // If error with upper API level
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

        // GET: ALL ORDER DOCUMENTS
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("GetOrders")]
        public JsonResult GetOrders()
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse = _ordersProvider.GetOrders();
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            } 
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
            }
        }

        // GET: SPECIFIC DOCUMENT
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("GetOrder")]
        public JsonResult GetOrder(string orderID)
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse = _ordersProvider.GetOrder(orderID);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            }
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
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
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);         
            }
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
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
                ProviderResponseWrapperCopy jsonOrder = _ordersProvider.GetOrder(orderID);
                //  GET response from StockAPI with OrderFullfillment Stock
                ProviderResponseWrapperCopy response = await _ordersProvider.BoxOrderCreateAsync(orderID);
                string jsonBoxOrderCreate = response.ResponseMessage;

                // Okay response from BoxOrderCreateAsync 
                if ((int)response.ResponseHTMLType == 200)
                {
                    // Save items to the order document 
                    ProviderResponseWrapperCopy providerResponseAsProviderResponseWrapper = _ordersProvider.AssignOrderItems(orderID, jsonOrder.ResponseMessage, jsonBoxOrderCreate);
                    return apiJsonResponse.CreateJsonResultResponse(providerResponseAsProviderResponseWrapper);
                }
                // Error Response
                else
                {
                    return apiJsonResponse.CreateJsonResultResponse(response);
                }
            }
            // API error
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
            }
        }

        // TODO
        [HttpPut]
        [Route("ModifyOrder")]
        public JsonResult ModifyOrderStatus(string orderID, string statusType)
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse = _ordersProvider.ModifyOrderStatus(orderID, statusType);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);      
            }
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
            }
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpDelete]
        [Route("RemoveCompletedOrders")]
        public JsonResult RemoveCompletedOrders()
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse =  _ordersProvider.RemoveCompletedOrders();
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);                
            }
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
            }
        }

        [HttpDelete]
        [Route("RemoveOrder")]
        public async Task<JsonResult> RemoveOrder(string orderID)
        {
            try
            {
                ProviderResponseWrapperCopy providerResponse = await _ordersProvider.RemoveOrder(orderID);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            }
            catch (Exception ex)
            {
                ProviderResponseWrapperCopy apiUnavaliableResponse = new ProviderResponseWrapperCopy
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
            }
        }
    }
}
