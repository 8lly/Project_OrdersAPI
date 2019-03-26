﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrdersAPI.Assistants;
using OrdersAPI.Models;
using OrdersAPI.Wrapper;
using WooCommerceAPI.BLL;

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
                ProviderResponseWrapper providerResponse = _ordersProvider.GetLateOrders();
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            }
            // If error with upper API level
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
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
                ProviderResponseWrapper providerResponse = _ordersProvider.GetOrders();
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            } 
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
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
                ProviderResponseWrapper providerResponse = _ordersProvider.GetOrder(orderID);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            }
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
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
                ProviderResponseWrapper providerResponse = _ordersProvider.CreateOrderDocument(newOrder);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);         
            }
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
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
                ProviderResponseWrapper jsonOrder = _ordersProvider.GetOrder(orderID);
                //  GET response from StockAPI with OrderFullfillment Stock
                ProviderResponseWrapper response = await _ordersProvider.BoxOrderCreateAsync(orderID);
                string jsonBoxOrderCreate = response.ResponseMessage;

                // Okay response from BoxOrderCreateAsync 
                if ((int)response.ResponseHTMLType == 200)
                {
                    // Save items to the order document 
                    ProviderResponseWrapper providerResponseAsProviderResponseWrapper = _ordersProvider.AssignOrderItems(orderID, jsonOrder.ResponseMessage, jsonBoxOrderCreate);
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
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
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
                ProviderResponseWrapper providerResponse = _ordersProvider.ModifyOrderStatus(orderID, statusType);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);      
            }
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
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
                ProviderResponseWrapper providerResponse =  _ordersProvider.RemoveCompletedOrders();
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);                
            }
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
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
                ProviderResponseWrapper providerResponse = await _ordersProvider.RemoveOrder(orderID);
                return apiJsonResponse.CreateJsonResultResponse(providerResponse);
            }
            catch (Exception ex)
            {
                ProviderResponseWrapper apiUnavaliableResponse = new ProviderResponseWrapper
                {
                    ResponseMessage = ex.ToString(),
                    ResponseHTMLType = HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE
                };
                return apiJsonResponse.CreateJsonResultResponse(apiUnavaliableResponse);
            }
        }
    }
}
