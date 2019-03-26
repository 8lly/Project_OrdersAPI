using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrdersAPI.Helpers;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        PRWBuilderHelper prwBuilderHelper = new PRWBuilderHelper();
        DoesOrderContainItemsHelper doesOrderHelper = new DoesOrderContainItemsHelper();

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
                    string repositoryResponseJson = JsonConvert.SerializeObject(allOrders);
                    return prwBuilderHelper.PRWBuilder(repositoryResponseJson, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                else
                {
                    return prwBuilderHelper.PRWBuilder("No orders have been saved!", HTTPResponseCodes.HTTP_NOT_FOUND);
                }
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
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
                    return prwBuilderHelper.PRWBuilder(repositoryResponseJson, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                else
                {
                    return prwBuilderHelper.PRWBuilder("No orders have been saved!", HTTPResponseCodes.HTTP_NOT_FOUND);
                }
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
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
                        return prwBuilderHelper.PRWBuilder(repositoryMessage, HTTPResponseCodes.HTTP_OK_RESPONSE);
                    }
                    return prwBuilderHelper.PRWBuilder(repositoryMessage, HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
                }
                else
                {
                    return prwBuilderHelper.PRWBuilder("Some fields are completed incorrect. Please re-enter values again.", HTTPResponseCodes.HTTP_BAD_REQUEST);
                }
            }
            catch (NullReferenceException ex)
            {
                return prwBuilderHelper.PRWBuilder("The form has not been fully complete, please send a completed form.", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public ProviderResponseWrapperCopy RemoveCompletedOrders()
        {
            try
            {
                string repositoryResponse = _ordersRepository.RemoveCompletedOrders();
                if (repositoryResponse == "Completed Orders Cleared")
                {
                    return prwBuilderHelper.PRWBuilder(repositoryResponse, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                else
                {
                    return prwBuilderHelper.PRWBuilder("Database failure. Please try again sure.", HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
                }
            } 
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public ProviderResponseWrapperCopy ModifyOrderStatus(string orderID, string statusType)
        {
            try
            {
                string repositoryResponse = _ordersRepository.ModifyOrderStatus(orderID, statusType);
                if (repositoryResponse == "Stock status has been adjusted.")
                {
                    return prwBuilderHelper.PRWBuilder(repositoryResponse, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                else
                {
                    return prwBuilderHelper.PRWBuilder("Database failure. Please try again sure.", HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
                }
            }
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
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
                    
                    // Send request to OrderFullfillmentStock EndPoint in Stock API
                    HttpClient client = new HttpClient();
                    string uri = "http://localhost:55001/api/Stock/OrderFulfillmentStock?orderSKU=" + sku;

                    // Returned Message
                    HttpResponseMessage httpResponse = client.GetAsync(uri).Result;
                    // Read stock in message into var
                    string response = await httpResponse.Content.ReadAsStringAsync();

                    // Deserialize http response string into return format
                    string boxStock = JsonConvert.DeserializeObject<string>(response);
                    return prwBuilderHelper.PRWBuilder(boxStock, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                return prwBuilderHelper.PRWBuilder("The SKU field is null. Please enter something in the field.", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (ArgumentException)
            {
                return prwBuilderHelper.PRWBuilder("This SKU does not exist. Please try another SKU.", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (HttpRequestException)
            {
                return prwBuilderHelper.PRWBuilder("Failed to contact to Stock API. Please try later.", HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
            catch (NullReferenceException ex)
            {
                return prwBuilderHelper.PRWBuilder("Not enough stock is eligible to fulfill the order.", HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
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
                    return prwBuilderHelper.PRWBuilder(responseRepository, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                return prwBuilderHelper.PRWBuilder(responseRepository, HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
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
                    return prwBuilderHelper.PRWBuilder(json, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                return prwBuilderHelper.PRWBuilder("No record found with given Order ID", HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            catch (NullReferenceException ex)
            {
                return prwBuilderHelper.PRWBuilder("No Order ID was given, please enter an Order ID", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.Message, HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public async Task<ProviderResponseWrapperCopy> RemoveOrder(string orderID)
        {
            try
            {
                // Delete record, save copy to return freed stock
                OrderDTO removedOrder = _ordersRepository.RemoveOrder(orderID);
                if (removedOrder != null)
                {
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

                    // Only run this method if the removed order contained items
                    if (doesOrderHelper.DoesOrderContainItems(removedOrder.SKU, allocatedItems) == true)
                    {
                        await ReallocatedRemovedOrderStock(allocatedItems);
                    }
                    return prwBuilderHelper.PRWBuilder("Record has been successfully removed", HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                // Order ID does not exist, or is incorrect 
                return prwBuilderHelper.PRWBuilder("No record matches given Order ID", HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            // Back-end failures 
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public async Task<ProviderResponseWrapperCopy> ReallocatedRemovedOrderStock(List<string> reallocatedStockList)
        {
            try
            {
                HttpClient client = new HttpClient();
                string requestURI = "http://localhost:55001/api/Stock/UpdateStockAddAllocation";

                for (int x = 0; x < reallocatedStockList.Count; x++)
                {
                    // How to get rid of this?
                    var content = new StringContent("");
                    HttpResponseMessage response = await client.PostAsync($"{requestURI}/?body={reallocatedStockList[x]}", content);
                }

                return prwBuilderHelper.PRWBuilder("Order Successfully Removed", HTTPResponseCodes.HTTP_OK_RESPONSE);
            }
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
    }
    }
}
