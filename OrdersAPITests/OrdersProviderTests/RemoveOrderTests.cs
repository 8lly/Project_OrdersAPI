using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OrdersAPI.Interfaces;
using OrdersAPI.Models;
using OrdersAPI.Wrapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;

// TODO: TO MOCK THE RESPONSE OF THIS 

namespace OrdersAPITests.OrdersProviderTests
{
    class RemoveOrderTests
    {
        private Mock<IHttpClientWrapper> _mockHttpClient;
        private Mock<IOrdersRepository> _mockOrdersRepository;

        [SetUp]
        public void Setup()
        {
            _mockHttpClient = new Mock<IHttpClientWrapper>();
            _mockOrdersRepository = new Mock<IOrdersRepository>();
        }

        // TEST: Remove Valid OrderID
        [Test]
        public async Task TestRemoveOrderValidID()
        {
            // Arrange
            string orderDTOjson = JsonConvert.SerializeObject(CreateBoxItems());
            HttpContent content = new StringContent(orderDTOjson);
            string validOrderID = "5c8818b2f81e697d3c82d90e";

            _mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage { Content = content, StatusCode = HttpStatusCode.OK }));

            _mockOrdersRepository.Setup(x => x.RemoveOrder(It.IsAny<string>())).Returns(CreateOrder());

            // Act
            OrdersProvider ordersProvider = new OrdersProvider(_mockOrdersRepository.Object, _mockHttpClient.Object);
            ProviderResponseWrapper outputGetRemovedItemMessageAsPRW = await
                ordersProvider.RemoveOrder(validOrderID);

            // Asserts
            Assert.AreEqual("Record has been successfully removed", outputGetRemovedItemMessageAsPRW.ResponseMessage);
            Assert.AreEqual(HTTPResponseCodes.HTTP_OK_RESPONSE, outputGetRemovedItemMessageAsPRW.ResponseHTMLType);
        }

        // TEST: Remove Invalid OrderID
        [Test]
        public async Task TestRemoveOrderInvalidID()
        {
            // Arrange
            string invalidOrderID = "invalid";

            // Act
            OrdersProvider ordersProvider = new OrdersProvider(_mockOrdersRepository.Object, _mockHttpClient.Object);
            ProviderResponseWrapper outputGetRemovedItemMessageAsPRW = await
                ordersProvider.RemoveOrder(invalidOrderID);

            // Asserts
            Assert.AreEqual("No record matches given Order ID", outputGetRemovedItemMessageAsPRW.ResponseMessage);
            Assert.AreEqual(HTTPResponseCodes.HTTP_NOT_FOUND, outputGetRemovedItemMessageAsPRW.ResponseHTMLType);
        }

        // TEST: Null Order ID
        [Test] 
        public async Task TestRemoveOrderNullID()
        {
            // Arrange
            string nullOrderID = "";

            // Act
            OrdersProvider ordersProvider = new OrdersProvider(_mockOrdersRepository.Object, _mockHttpClient.Object);
            ProviderResponseWrapper outputGetRemovedItemMessageAsPRW = await
                ordersProvider.RemoveOrder(nullOrderID);

            // Asserts
            Assert.AreEqual("No Order Exists", outputGetRemovedItemMessageAsPRW.ResponseMessage);
            Assert.AreEqual(HTTPResponseCodes.HTTP_NOT_FOUND, outputGetRemovedItemMessageAsPRW.ResponseHTMLType);
        }

        private OrderDTO CreateOrder()
        {
            OrderDTO order = new OrderDTO
            {
                Id = "Test",
                ItemOneName = "1",
                ItemTwoName = "2",
                ItemThreeName = "3",
                ItemFourName = "4",
                ItemFiveName = "5",
                ItemSixName = "6",
            };
            return order;
        }

        private List<OrderDTO> CreateBoxItems()
        {
            // Arrange
            List<OrderDTO> order = new List<OrderDTO>();


            while (order.Count != 6)
            {
                    OrderDTO item = new OrderDTO
                    {
                        Id = "Test",
                        ItemOneName = "1",
                        ItemTwoName = "2",
                        ItemThreeName = "3",
                        ItemFourName = "4",
                        ItemFiveName = "5",
                        ItemSixName = "6",
                    };
                    order.Add(item);
            }

            return order;
        }
    }
}
