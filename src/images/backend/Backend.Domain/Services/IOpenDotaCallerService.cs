using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.Services
{
    public interface IOpenDotaCallerService
    {
        Task<string> GetValue(string url);
        Task<HttpResponseMessage> PostValue(string url, StringContent stringContent);
    }
}