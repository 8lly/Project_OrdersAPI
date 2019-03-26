using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


// Used for unit testing to mock responses from opposing API's
namespace OrdersAPI.Interfaces
{
    public interface IHttpClientWrapper
    {
        // GetASync
        Task<HttpResponseMessage> GetAsync(string url);
        // PostASync
        Task<HttpResponseMessage> PostAsync(string url, StringContent content);
    }
}
