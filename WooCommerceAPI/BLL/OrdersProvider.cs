using Newtonsoft.Json;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using OrdersAPI.Wrapper;
using WooCommerceAPI.DAL;
using OrdersAPI.Assistants;
using OrdersAPI.Interfaces;

namespace WooCommerceAPI.BLL
{
    public class OrdersProvider : IOrdersProvider
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IHttpClientWrapper _httpClient;
        PRWBuilderHelper prwBuilderHelper = new PRWBuilderHelper();

        public OrdersProvider(IOrdersRepository gop, IHttpClientWrapper hcw)
        {
            _ordersRepository = gop;
            _httpClient = hcw;
        }

        public ProviderResponseWrapper GetOrders()
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

        public ProviderResponseWrapper GetLateOrders()
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
                    return prwBuilderHelper.PRWBuilder("No late orders", HTTPResponseCodes.HTTP_NOT_FOUND);
                }
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public ProviderResponseWrapper CreateOrderDocument(Order newOrder)
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
                    return prwBuilderHelper.PRWBuilder("The form has missing information or is incorrect. Please re-enter values again.", HTTPResponseCodes.HTTP_BAD_REQUEST);
                }
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public ProviderResponseWrapper RemoveCompletedOrders()
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
                    return prwBuilderHelper.PRWBuilder("No Completed Orders Exist", HTTPResponseCodes.HTTP_NOT_FOUND);
                }
            } 
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public ProviderResponseWrapper ModifyOrderStatus(string orderID, string statusType)
        {
            try
            {
                string repositoryResponse = _ordersRepository.ModifyOrderStatus(orderID, statusType);
                return prwBuilderHelper.PRWBuilder(repositoryResponse, HTTPResponseCodes.HTTP_OK_RESPONSE);
            }
            catch (FormatException)
            {
                return prwBuilderHelper.PRWBuilder("Order ID does not exist", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (NullReferenceException)
            {
                return prwBuilderHelper.PRWBuilder("No Order ID was given, please enter an Order ID",
                    HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public async Task<ProviderResponseWrapper> BoxOrderCreateAsync(string orderID)
        {
            try
            {
                OrderDTO selectedOrder = _ordersRepository.GetOrder(orderID);
                string sku = selectedOrder.SKU;

                // Send request to OrderFullfillmentStock EndPoint in Stock API
                HttpClient client = new HttpClient();
                string uri = "http://localhost:52772/api/Stock/OrderFulfillmentStock?orderSKU=" + sku;

                // Returned Message
                HttpResponseMessage httpResponse = _httpClient.GetAsync(uri).Result;
                // Read stock in message into var
                string response = await httpResponse.Content.ReadAsStringAsync();

                // Deserialize http response string into return format
                if (httpResponse.IsSuccessStatusCode)
                {
                  return prwBuilderHelper.PRWBuilder(response, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                // If response from StockAPI gave 4XX or 5XX error
                return prwBuilderHelper.PRWBuilder(response, HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (ArgumentException)
            {
                return prwBuilderHelper.PRWBuilder("This SKU does not exist. Please try another SKU.", HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            catch (FormatException)
            {
                return prwBuilderHelper.PRWBuilder("This is not a valid Order ID, please try re-entering an ID.", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (HttpRequestException)
            {
                return prwBuilderHelper.PRWBuilder("Failed to contact to Stock API. Please try later.", HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
            catch (NullReferenceException)
            {
                return prwBuilderHelper.PRWBuilder("Please enter a Order ID.", HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }
        
        public ProviderResponseWrapper AssignOrderItems(string orderID, string jsonOrder, string jsonBoxOrderCreate)
        {
            try
            {
                string boxOrderCreate = JsonConvert.DeserializeObject<string>(jsonBoxOrderCreate);
                List<StockCopyDTO> orderItems = JsonConvert.DeserializeObject<List<StockCopyDTO>>(boxOrderCreate);
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

        public ProviderResponseWrapper GetOrder(string orderID)
        {
            try
            {
                OrderDTO order = _ordersRepository.GetOrder(orderID);
                if (order != null)
                {
                    string json = JsonConvert.SerializeObject(order);
                    return prwBuilderHelper.PRWBuilder(json, HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                return prwBuilderHelper.PRWBuilder("No Order ID document could be found.",
                    HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (NullReferenceException)
            {
                return prwBuilderHelper.PRWBuilder("No Order ID was given, please enter an Order ID",
                    HTTPResponseCodes.HTTP_BAD_REQUEST);
            }
            catch (FormatException)
            {
                return prwBuilderHelper.PRWBuilder("The ID was not in the appropiate format, please enter a valid Order ID ",
                    HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            catch (Exception ex1)
            {
                return prwBuilderHelper.PRWBuilder(ex1.Message, HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public async Task<ProviderResponseWrapper> RemoveOrder(string orderID)
        {
            try
            {
                if (orderID != null)
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

                    DoesOrderContainItemsHelper doesOrderHelper = new DoesOrderContainItemsHelper();

                    // Only run this method if the removed order contained items
                    if (doesOrderHelper.DoesOrderContainItems(removedOrder.SKU, allocatedItems) == true)
                    {
                        await ReallocatedRemovedOrderStock(allocatedItems);
                    }

                    return prwBuilderHelper.PRWBuilder("Record has been successfully removed",
                        HTTPResponseCodes.HTTP_OK_RESPONSE);
                }
                return prwBuilderHelper.PRWBuilder("No order exists, null given",
                    HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            catch (NullReferenceException)
            {
                return prwBuilderHelper.PRWBuilder("No record matches given order ID",
                    HTTPResponseCodes.HTTP_NOT_FOUND);
            }
            // Back-end failures 
            catch (Exception ex)
            {
                return prwBuilderHelper.PRWBuilder(ex.ToString(), HTTPResponseCodes.HTTP_SERVER_FAILURE_RESPONSE);
            }
        }

        public async Task<ProviderResponseWrapper> ReallocatedRemovedOrderStock(List<string> reallocatedStockList)
        {
            try
            {
                HttpClient client = new HttpClient();
                string requestURI = "http://localhost:52772/api/Stock/UpdateStockAddAllocation";

                for (int x = 0; x < reallocatedStockList.Count; x++)
                {
                    // How to get rid of this?
                    var content = new StringContent("");
                    HttpResponseMessage response = await _httpClient.PostAsync($"{requestURI}/?body={reallocatedStockList[x]}", content);
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
