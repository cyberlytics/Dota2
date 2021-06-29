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

        /// <summary>
        /// HTTP Get-Anfrage
        /// </summary>
        /// <param name="url">Anfrage-URL</param>
        /// <returns>Content der Antwort in Stringform</returns>
        public async Task<string> GetValue(string url)
        {
            HttpResponseMessage responseMessage = await httpClient.GetAsync(url);
            return await responseMessage.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// HTTP Post-Anfrage
        /// </summary>
        /// <param name="url">Anfrage-URL</param>
        /// /// <param name="stringContent">Inhalt der Post-Anfrage in Stringform</param>
        public async Task<HttpResponseMessage> PostValue(string url, StringContent stringContent)
        {
            return await httpClient.PostAsync(url, stringContent);
        }
    }
}