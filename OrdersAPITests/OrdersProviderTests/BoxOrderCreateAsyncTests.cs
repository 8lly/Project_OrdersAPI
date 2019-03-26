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