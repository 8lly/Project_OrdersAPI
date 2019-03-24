﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WooCommerceAPI.DAL;

namespace WooCommerceAPI.BLL
{
    public class OrdersProvider : IOrdersProvider
    {

        // Repository obj
        private readonly IOrdersRepository _ordersRepository;

        public OrdersProvider(IOrdersRepository gop)
        {
            _ordersRepository = gop;
        }

        public ProviderResponseWrapperCopy GetOrders()
        {
            try
            {
                List<OrderDTO> allOrders = _ordersRepository.GetOrders();
                if (allOrders.Count > 0)
                {
                    string providerRepositoryJson = JsonConvert.SerializeObject(allOrders);
                    return PRWBuilder(providerRepositoryJson, 1);
                }
                else
                {
                    return PRWBuilder("No orders have been saved!", 2);
                }
            }
            catch (Exception ex1)
            {
                return PRWBuilder(ex1.ToString(), 3);
            }
        }

        public ProviderResponseWrapperCopy GetLateOrders()
        {
            try
            {
                List<OrderDTO> lateOrders = _ordersRepository.GetLateOrders();
                if (lateOrders.Count > 0)
                {
                    string repositoryResponseJson = JsonConvert.SerializeObject(lateOrders);
                    return PRWBuilder(repositoryResponseJson, 1);
                }
                else
                {
                    return PRWBuilder("No late orders have been saved!", 2);
                }
            }
            catch (Exception ex1)
            {
                return PRWBuilder(ex1.ToString(), 3);
            }
        }

        public ProviderResponseWrapperCopy CreateOrderDocument(Order newOrder)
        {
            try
            {
                OrderDTO newOrderDTO = new OrderDTO
                {
                    Order_Number = newOrder.Order_Number,
                    Customer_First = newOrder.Customer_First,
                    Customer_Last = newOrder.Customer_Last,
                    SKU = newOrder.SKU,
                    Status = newOrder.Status,
                    Order_Created = DateTime.Now
                };

                if (newOrderDTO.IsValid() == true)
                {
                    string repositoryMessage = _ordersRepository.CreateOrderDocument(newOrderDTO);
                    if (repositoryMessage == "New item Inserted")
                    {
                        return PRWBuilder(repositoryMessage, 1);
                    }
                    return PRWBuilder(repositoryMessage, 3);
                }
                else
                {
                    return PRWBuilder("Some fields are completed incorrect. Please re-enter values again.", 2);
                }
            }
            catch (ArgumentNullException ex)
            {
                return PRWBuilder(ex.ToString(), 2);
            }
            catch (Exception ex1)
            {
                return PRWBuilder(ex1.ToString(), 3);
            }
        }

        public string RemoveCompletedOrders()
        {
            try
            {
                return _ordersRepository.RemoveCompletedOrders();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ModifyOrderStatus(string orderID, string statusType)
        {
            try
            {
                return _ordersRepository.ModifyOrderStatus(orderID, statusType);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<ProviderResponseWrapperCopy> BoxOrderCreateAsync(string orderID)
        {
            try
            {
                if (orderID != null)
                {
                    OrderDTO selectedOrder = _ordersRepository.GetOrder(orderID);
                    string sku = selectedOrder.SKU;
                    
                    HttpClient client = new HttpClient();
                    string uri = "http://localhost:55001/api/Stock/OrderFulfillmentStock?orderSKU=" + sku;

                    HttpResponseMessage httpResponse = client.GetAsync(uri).Result;
                    string response = await httpResponse.Content.ReadAsStringAsync();

                    /* 
                    JsonResult jsonResponse = new JsonResult(JsonConvert.DeserializeObject(response))
                    {
                        StatusCode = (int)httpResponse.StatusCode,
                        ContentType = httpResponse.Content.Headers.ContentType.ToString()
                    };
                    */
                    
                    // Deserialize http response string into return format
                    string boxStock = JsonConvert.DeserializeObject<string>(response);
                    return PRWBuilder(boxStock, 1);
                }
                return PRWBuilder("The SKU field is null. Please enter something in the field.", 2);
            }
            catch (ArgumentException)
            {
                return PRWBuilder("This SKU does not exist. Please try another SKU.", 2);
            }
            catch (HttpRequestException)
            {
                return PRWBuilder("Failed to contact to Stock API. Please try later.", 3);
            }
            catch (NullReferenceException ex)
            {
                return PRWBuilder("Not enough stock is eligible to fulfill the order.", 2);
            }
            catch (Exception ex)
            {
                return PRWBuilder(ex.ToString(), 3);
            }
        }
        
        public ProviderResponseWrapperCopy AssignOrderItems(string orderID, string jsonOrder, string jsonBoxOrderCreate)
        {
            try
            {
                List<StockCopyDTO> orderItems = JsonConvert.DeserializeObject<List<StockCopyDTO>>(jsonBoxOrderCreate);
                OrderDTO orderObject = JsonConvert.DeserializeObject<OrderDTO>(jsonOrder);
                string responseRepository = _ordersRepository.AssignOrderItems(orderID, orderItems, orderObject);

                if (responseRepository == "Order record has been updated with allocated items")
                {
                    return PRWBuilder(responseRepository, 1);
                }
                return PRWBuilder(responseRepository, 3);
            }
            catch (Exception ex)
            {
                return PRWBuilder(ex.ToString(), 3);
            }
        }

        public ProviderResponseWrapperCopy GetOrder(string orderID)
        {
            try
            {
                OrderDTO order = _ordersRepository.GetOrder(orderID);
                if (order != null)
                {
                    string json = JsonConvert.SerializeObject(order);
                    return PRWBuilder(json, 1);
                }
                return PRWBuilder("No record found with given Order ID", 2);
            }
            catch (ArgumentNullException ex)
            {
                return PRWBuilder("No Order ID was given, please enter an Order ID", 2);
            }
            catch (Exception ex1)
            {
                return PRWBuilder(ex1.Message, 3);
            }
        }

        public string RemoveOrder(string orderID)
        {
            try
            {
                // Delete record, save copy to return freed stock
                OrderDTO removedOrder = _ordersRepository.RemoveOrder(orderID);
                // Stock items returning to stock db
                List<string> allocatedItems = new List<string>()
                {
                    removedOrder.ItemOneName,
                    removedOrder.ItemTwoName,
                    removedOrder.ItemThreeName,
                    removedOrder.ItemFourName,
                    removedOrder.ItemFiveName,
                    removedOrder.ItemSixName,
                    removedOrder.ItemSevenName,
                    removedOrder.ItemEightName,
                };

                // Remove nulls from list 
                // allocatedItems.RemoveAll(string.IsNullOrWhiteSpace);

                ReallocatedRemovedOrderStock(allocatedItems);

                string json = JsonConvert.SerializeObject(allocatedItems);
                return json;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task <List<string>> ReallocatedRemovedOrderStock(List<string> reallocatedStockList)
        {

            HttpClient client = new HttpClient();
            string requestURI = "http://localhost:55001/api/Stock/UpdateStockAddAllocation";

            for (int x = 0; x < reallocatedStockList.Count; x++)
            {
                // How to get rid of this?
                var content = new StringContent("");

                HttpResponseMessage response = await client.PostAsync($"{requestURI}/?body={reallocatedStockList[x]}", content);
            }

            return new List<string>();
 
    }

        // Build exception messages 
        public ProviderResponseWrapperCopy PRWBuilder(string json, int responseType)
        {
            ProviderResponseWrapperCopy response = new ProviderResponseWrapperCopy
            {
                ResponseMessage = json,
                ResponseType = responseType
            };
            return response;
        }
    }
}
