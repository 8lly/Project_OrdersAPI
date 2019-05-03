using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using OrdersAPI.Wrapper;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;

namespace OrdersAPITests.OrdersProviderTests
{
    class GetLateOrdersTests
    {
        Mock<IOrdersRepository> _mockOrderRepository;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrdersRepository>();
        }

        // Test: Get one late order
        [Test]
        public void TestGetOrdersReturnsOneLateOrder()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetLateOrders()).Returns(CreateOrderDTOList(1));
            // Is this meant to be orderprovider or orderscontroller
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputGetOrdersAsProviderResponseWrapper = orderProvider.GetLateOrders();
            List<OrderDTO> outputOrderList = JsonConvert.DeserializeObject<List<OrderDTO>>(outputGetOrdersAsProviderResponseWrapper.ResponseMessage);

            // Assert
            Assert.AreEqual(1, outputOrderList.Count);
            Assert.AreEqual("Test0", outputOrderList[0].Id);
        }

        // Test: Get one thousand late orders
        [Test]
        public void TestGetOrdersReturnsOneThousandLateOrders()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetLateOrders()).Returns(CreateOrderDTOList(1000));
            // Is this meant to be orderprovider or orderscontroller
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputGetOrdersAsProviderResponseWrapper = orderProvider.GetLateOrders();
            List<OrderDTO> outputOrderList = JsonConvert.DeserializeObject<List<OrderDTO>>(outputGetOrdersAsProviderResponseWrapper.ResponseMessage);

            // Assert
            Assert.AreEqual(1000, outputOrderList.Count);
            for (int i = 0; i < outputOrderList.Count; i++){
                Assert.AreEqual("Test" + i.ToString(), outputOrderList[i].Id);
            }
        }

        // Test: Get null as response
        [Test]
        public void TestGetOrdersReturnsNullOrders()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetLateOrders()).Returns(CreateOrderDTOList(0));
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputGetOrdersAsProviderResponseWrapper = orderProvider.GetLateOrders();

            // Assert
            Assert.AreEqual("No late orders", outputGetOrdersAsProviderResponseWrapper.ResponseMessage);
        }

        private List<OrderDTO> CreateOrderDTOList(int numOfOrder)
        {
            List<OrderDTO> list = new List<OrderDTO>();
            for (int i = 0; i < numOfOrder; i++)
            {
                OrderDTO order = new OrderDTO
                {
                    Id = "Test" + i.ToString(),
                    Order_Created = DateTime.Now
                };
                list.Add(order);
            }
            return list;
        }

    }
}
