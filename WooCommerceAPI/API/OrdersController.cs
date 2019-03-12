using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersAPI.Models;
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
        
        [HttpGet]
        [Route("GatherOrders")]
        public string GetOrders()
        {
            try
            {
                return _ordersProvider.GetOrders();
            } 
            catch (Exception ex)
            {
                return ex.Message; 
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

        [HttpPost]
        [Route("ImportOrder")]
        public string CreateOrderDocument(Order newOrder)
        {
            try
            {
                return _ordersProvider.CreateOrderDocument(newOrder);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        [Route("SaveOrderUpdateStock")]
        public string OrderAndStockUpdate(string orderID)
        {
            try
            {
                string jsonOrder = GetOrder(orderID);
                string jsonBoxOrderCreate = BoxOrderCreate(orderID);
                try
                {
                    return _ordersProvider.AssignOrderItems(orderID, jsonOrder, jsonBoxOrderCreate);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        [Route("BoxOrderCreate")]
        public string BoxOrderCreate(string orderID)
        {
            try
            {
                return _ordersProvider.BoxOrderCreate(orderID);
            }
            catch (Exception ex)
            {
                return ex.Message;
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
    }
}
