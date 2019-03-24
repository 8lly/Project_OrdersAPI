using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OrdersAPI.Models;
using StockAPI.Models;
using System.Collections.Generic;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;

namespace OrdersAPITests.OrdersProviderTests
{
    public class Tests
    {
        Mock<IOrdersRepository> _mockOrderRepository;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrdersRepository>();
        }

        // Test: Get one valid order
        [Test]
        public void TestGetOrdersReturnsOneOrder()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetOrders()).Returns(CreateOrderDTOList(1));
            // Is this meant to be orderprovider or orderscontroller
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object);
            ProviderResponseWrapperCopy outputGetOrdersAsProviderResponseWrapper = orderProvider.GetOrders();
            List<OrderDTO> outputOrderList = JsonConvert.DeserializeObject<List<OrderDTO>>(outputGetOrdersAsProviderResponseWrapper.ResponseMessage);

            // Assert
            Assert.AreEqual(1, outputOrderList.Count);
            Assert.AreEqual("Test0", outputOrderList[0].Id);
        }

        // Test: Get one thousand valid orders
        [Test]
        public void TestGetOrdersReturnsOneThousandOrders()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetOrders()).Returns(CreateOrderDTOList(1000));
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object);
            ProviderResponseWrapperCopy outputGetOrdersAsProviderResponseWrapper = orderProvider.GetOrders();
            List<OrderDTO> outputOrderList = JsonConvert.DeserializeObject<List<OrderDTO>>(outputGetOrdersAsProviderResponseWrapper.ResponseMessage);

            // Assert
            Assert.AreEqual(1000, outputOrderList.Count);
            for (int i = 0; i < outputOrderList.Count; i++)
            {
                Assert.AreEqual("Test" + i.ToString(), outputOrderList[i].Id);
            }
        }

        // Test: Get null as response from 
        [Test]
        public void TestGetOrdersReturnsNullOrders()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetOrders()).Returns(CreateOrderDTOList(0));
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object);
            ProviderResponseWrapperCopy outputGetOrdersAsProviderResponseWrapper = orderProvider.GetOrders();

            // Assert
            Assert.AreEqual("No orders have been saved!", outputGetOrdersAsProviderResponseWrapper.ResponseMessage);
        }

        private List<OrderDTO> CreateOrderDTOList(int numOfOrder)
        {
            List<OrderDTO> list = new List<OrderDTO>();
            for (int i = 0; i < numOfOrder; i++)
            {
                OrderDTO order = new OrderDTO
                {
                    Id = "Test" + i.ToString()
                };
            list.Add(order);
            }
            return list;
        }
    }
}