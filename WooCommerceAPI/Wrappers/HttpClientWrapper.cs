using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OrdersAPI.Interfaces;

namespace OrdersAPI.Wrappers
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper()
        {
            _httpClient = new HttpClient();
        }

        // Replication of GetAsync method
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            return await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        }

        public async Task<HttpResponseMessage> PostAsync(string uri, StringContent content)
        {
            return await _httpClient.PostAsync(uri, content);
        }
    }
}
