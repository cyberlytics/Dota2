using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RateLimiter;
using ComposableAsync;

namespace Backend.Domain.Services
{
    public class OpenDotaCallerService : IOpenDotaCallerService
    {
        private HttpClient httpClient;

        public OpenDotaCallerService()
        {
            //Ratelimiter auf 60 Operationen pro Minute
            var handler = TimeLimiter
                                .GetFromMaxCountByInterval(60, TimeSpan.FromMinutes(1))
                                .AsDelegatingHandler();
            httpClient = new HttpClient(handler);
        }

        public async Task<string> GetValue(string url)
        {
            HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
            return await responseMessage.Content.ReadAsStringAsync();
        }

        public async Task<HttpResponseMessage> PostValue(string url, StringContent stringContent)
        {
            return await httpClient.PostAsync(url, stringContent);
        }
    }
}