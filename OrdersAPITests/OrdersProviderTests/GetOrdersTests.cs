using Moq;
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
        public void TestGetOrdersReturnsOneItem()
        {
            // Act
            _mockOrderRepository.Setup(x => x.GetOrders()).Returns(CreateOrderDTOList(1));
            // Why is this broke?
            OrdersController orderProvider = new OrdersController(_mockOrderRepository.Object);

            ProviderResponseWrapperCopy outputGetOrdersAsProviderResponseWrapper = orderProvider.GetOrders();


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