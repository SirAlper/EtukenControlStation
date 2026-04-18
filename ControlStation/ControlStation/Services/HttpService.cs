using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ControlStation.Services
{
    
    class HttpService
    {
        private string ServerUrl { get; set; } = "http://127.0.0.25:5000/";

        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://127.0.0.25:5000/")
        };
        public HttpService()
        {
           

        }
        
    }

}
