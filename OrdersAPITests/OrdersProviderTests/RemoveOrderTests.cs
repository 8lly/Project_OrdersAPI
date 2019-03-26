using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using WooCommerceAPI.DAL;

// TODO: TO MOCK THE RESPONSE OF THIS 

namespace OrdersAPITests.OrdersProviderTests
{
    class RemoveOrderTests
    {
        Mock<IOrdersRepository> _mockOrderRepository;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrdersRepository>();
        }

        // TEST: Remove Valid OrderID
        [Test]
        public void TestRemoveOrderValidID()
        {
            // Arrange
            string validOrderID = "5c979252a7d6851258d7b575";

            // Act
           // _mockOrderRepository.Setup(x => x.RemoveOrder(It.IsAny<string>())).Returns("");
        }
    }
}
