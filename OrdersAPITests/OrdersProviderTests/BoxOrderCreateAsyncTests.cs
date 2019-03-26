using Moq;
using NUnit.Framework;
using StockAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;


/**
namespace OrdersAPITests.OrdersProviderTests
{
    class BoxOrderCreateAsyncTests
    {

        [Test]
        public void BoxorderCreateGetGoodResponse()
        {
            
        }

        private List<StockCopyDTO> CreateStockDTOList(int numOfStock)
        {
            List<StockCopyDTO> list = new List<StockCopyDTO>();
            for (int i = 0; i < numOfStock; i++)
            {
                StockCopyDTO stock = new StockCopyDTO
                {
                    InBoxOne = true
                };
                list.Add(stock);
            }
            return list;
        }
    }
}
*/