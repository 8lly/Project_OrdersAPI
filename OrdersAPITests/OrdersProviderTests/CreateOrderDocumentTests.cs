using Moq;
using NUnit.Framework;
using OrdersAPI.Models;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using OrdersAPI.Interfaces;
using OrdersAPI.Wrapper;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;

namespace OrdersAPITests.OrdersProviderTests
{
    class CreateOrderDocumentTests
    {
        Mock<IOrdersRepository> _mockOrderRepository;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrdersRepository>();
        }
        
        [Test]
        public void TestCreateValidOrder()
        {
            // Arrange
            string validCustomerFirst = "test";
            Order newValidTestOrder = CreateTestOrder(validCustomerFirst);

            // Act
            _mockOrderRepository.Setup(x => x.CreateOrderDocument(It.IsAny<OrderDTO>())).Returns("New item Inserted");
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputCreateOrderDocumentsAsProviderResponseWrapper = orderProvider.CreateOrderDocument(newValidTestOrder);

            // Assert
            Assert.AreEqual("New item Inserted", outputCreateOrderDocumentsAsProviderResponseWrapper.ResponseMessage);
        }

        [Test]
        public void TestCreateInvalidOrder()
        {
            // Arrange
            string nullCustomerFirst = null;
            Order newInvalidTestOrder = CreateTestOrder(nullCustomerFirst);

            // Act
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputCreateOrderDocumentsAsProviderResponseWrapper = orderProvider.CreateOrderDocument(newInvalidTestOrder);

            // Assert
            Assert.AreEqual("Some fields are completed incorrect. Please re-enter values again.", outputCreateOrderDocumentsAsProviderResponseWrapper.ResponseMessage);
        }

        [Test]
        public void TestCreateNullOrder()
        {
            // Arrange 
            Order newNullTestOrder = null;

            // Act
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputCreateOrderDocumentsAsProviderResponseWrapper = orderProvider.CreateOrderDocument(newNullTestOrder);

            // Assert
            Assert.AreEqual("The form has not been fully complete, please send a completed form.", outputCreateOrderDocumentsAsProviderResponseWrapper.ResponseMessage);
        }

        private Order CreateTestOrder(string customerFirstName)
        {
            Order newTestOrder = new Order();
            {
                newTestOrder.Customer_First = customerFirstName;
                newTestOrder.Customer_Last = "test";
                newTestOrder.Order_Number = "000000";
                newTestOrder.SKU = "SKU000001";
                newTestOrder.Status = "Awaiting Action";
            };
            return newTestOrder;
        }
    }
}
