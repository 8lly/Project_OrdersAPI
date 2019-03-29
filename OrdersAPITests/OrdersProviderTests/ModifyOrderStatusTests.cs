using Moq;
using NUnit.Framework;
using OrdersAPI.Wrapper;
using System;
using System.Collections.Generic;
using System.Text;
using WooCommerceAPI.BLL;
using WooCommerceAPI.DAL;

namespace OrdersAPITests.OrdersProviderTests
{

    // Note: Status type is not checked here as it is locked by the front-end to specific values via dropdown. 

    class ModifyOrderStatusTests
    {

        Mock<IOrdersRepository> _mockOrderRepository;

        [SetUp]
        public void Setup()
        {
            _mockOrderRepository = new Mock<IOrdersRepository>();
        }

        [Test]
        public void ModifyOrderStatusSuccess()
        {
            // Arrange 

            string validOrderID = "5c979251a7d6851258d7b574";
            string statusType = "Completed";

            // Act 

            _mockOrderRepository.Setup(x => x.ModifyOrderStatus(It.IsAny<string>(), It.IsAny<string>())).Returns("Stock status has been adjusted.");
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputModifyOrderStatusAsPRW = orderProvider.ModifyOrderStatus(validOrderID, statusType);

            // Assert

            Assert.AreEqual(outputModifyOrderStatusAsPRW.ResponseMessage, "Stock status has been adjusted.");
            Assert.AreEqual(outputModifyOrderStatusAsPRW.ResponseHTMLType, HTTPResponseCodes.HTTP_OK_RESPONSE);
        }

        [Test]
        public void ModifyOrderStatusInvalidOrderID()
        {
            // Arrange 

            string invalidOrderID = "Invalid Test ID";
            string statusType = "Completed";

            // Act 

            _mockOrderRepository.Setup(x => x.ModifyOrderStatus(It.IsAny<string>(), It.IsAny<string>())).Throws(new FormatException());
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputModifyOrderStatusAsPRW = orderProvider.ModifyOrderStatus(invalidOrderID, statusType);

            // Assert

            Assert.AreEqual("Order ID does not exist", outputModifyOrderStatusAsPRW.ResponseMessage);
            Assert.AreEqual(HTTPResponseCodes.HTTP_BAD_REQUEST, outputModifyOrderStatusAsPRW.ResponseHTMLType);
        }

        [Test]
        public void ModifyOrderStatusNullOrderId()
        {
            // Arrange 

            string validOrderID = "";
            string statusType = "";

            // Act 

            _mockOrderRepository.Setup(x => x.ModifyOrderStatus(It.IsAny<string>(), It.IsAny<string>())).Throws(new NullReferenceException());
            OrdersProvider orderProvider = new OrdersProvider(_mockOrderRepository.Object, null);
            ProviderResponseWrapper outputModifyOrderStatusAsPRW = orderProvider.ModifyOrderStatus(validOrderID, statusType);

            // Assert

            Assert.AreEqual(outputModifyOrderStatusAsPRW.ResponseMessage, "No Order ID was given, please enter an Order ID");
            Assert.AreEqual(HTTPResponseCodes.HTTP_BAD_REQUEST, outputModifyOrderStatusAsPRW.ResponseHTMLType);
        }
    }
}
