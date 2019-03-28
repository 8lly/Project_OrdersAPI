using Moq;
using NUnit.Framework;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using OrdersAPI.Interfaces;
using OrdersAPI.Wrapper;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;
using OrdersAPI.Models;

namespace OrdersAPITests.OrdersProviderTests
{
    class BoxOrderCreateAsyncTests
    {
        private Mock<IHttpClientWrapper> _mockHttpClient;
        private Mock<IOrdersRepository> _mockOrdersRepository;

        [SetUp]
        public void Setup()
        {
            _mockHttpClient = new Mock<IHttpClientWrapper>();
            _mockOrdersRepository = new Mock<IOrdersRepository>();
        }

        // Finally works!!!!!!
        [Test]
        public async Task TestGetCompletedListOfItemsForOrder()
        {

            // Arrange
            string orderDTOjson = JsonConvert.SerializeObject(CreateOrderDTO());
            HttpContent content = new StringContent(orderDTOjson);

            _mockHttpClient.Setup(x => x.GetAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new HttpResponseMessage { Content = content, StatusCode = HttpStatusCode.OK}));

            _mockOrdersRepository.Setup(x => x.GetOrder(It.IsAny<string>())).Returns(CreateOrderDTO());

            // Act 
            OrdersProvider ordersProvider = new OrdersProvider(_mockOrdersRepository.Object, _mockHttpClient.Object);
            ProviderResponseWrapper outputGetCompletedListOfItemsForOrderAsPRW = await
                ordersProvider.BoxOrderCreateAsync(orderDTOjson);

            // Asserts
            Assert.AreEqual(outputGetCompletedListOfItemsForOrderAsPRW.ResponseMessage, orderDTOjson);
            Assert.AreEqual(HTTPResponseCodes.HTTP_OK_RESPONSE, outputGetCompletedListOfItemsForOrderAsPRW.ResponseHTMLType);
        }

        [Test]
        public async Task TestCompletedOrderInvalidOrderID()
        {
            // Arrange
            string orderDTOjson = "60242a787f13737283ef0bce";

            // Act
            OrdersProvider ordersProvider = new OrdersProvider(null, null);
            ProviderResponseWrapper outputGetNullResponseAsPRW = await
                ordersProvider.BoxOrderCreateAsync(orderDTOjson);

            // Asserts
            Assert.AreEqual("No Order ID document could be found.", outputGetNullResponseAsPRW.ResponseMessage);
            Assert.AreEqual(HTTPResponseCodes.HTTP_NOT_FOUND , outputGetNullResponseAsPRW.ResponseHTMLType);
        }

        [Test]
        public async Task TestCompleteOrderNullOrderID()
        {
            // Arrange
            string orderDTOjson = null;

            // Act
            OrdersProvider ordersProvider = new OrdersProvider(null, null);
            ProviderResponseWrapper outputGetNullResponseAsPRW = await
                ordersProvider.BoxOrderCreateAsync(orderDTOjson);

            // Asserts
            Assert.AreEqual("Please enter a Order ID.", outputGetNullResponseAsPRW.ResponseMessage);
            Assert.AreEqual(HTTPResponseCodes.HTTP_BAD_REQUEST, outputGetNullResponseAsPRW.ResponseHTMLType);
        }

        private OrderDTO CreateOrderDTO()
        {
            OrderDTO order = new OrderDTO
            {
                Id = "Test"
            };
            return order;
        }
    }
}