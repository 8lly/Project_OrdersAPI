using Moq;
using NUnit.Framework;
using OrdersAPI.Wrapper;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;

namespace OrdersAPITests.OrdersProviderTests
{
    class RemoveCompletedOrdersTests
    {
        Mock<IOrdersRepository> _mockOrderRepository;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrdersRepository>();
        }

        [Test]
        public void TestRemoveAllCompletedOrders()
        {
            // Act
            _mockOrderRepository.Setup(x => x.RemoveCompletedOrders()).Returns("Completed Orders Cleared");
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputRemoveAllCompletedOrdersAsProviderResponseWrapper = orderProvider.RemoveCompletedOrders();

            // Assert
            Assert.AreEqual("Completed Orders Cleared", outputRemoveAllCompletedOrdersAsProviderResponseWrapper.ResponseMessage);
        }
    }
}
