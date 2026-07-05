using ControlStation.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ControlStation.Services
{
    
    class HttpService
    {
        private string ServerUrl { get; set; } = "http://127.0.0.25:5000/";


        public event EventHandler<string> DataRecieved;

        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("http://127.0.0.25:5000/")
        };

        
        public HttpService()
        {
            
            

        }

        static async Task<HttpStatusCode> LoginAsync(HttpClient httpClient, string kadi, string sifre)
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
                "api/giris",
                new Login(kadi: kadi, sifre: sifre));

            return response.StatusCode;   
            
        }

        static async Task PostTelemetryAsync(HttpClient httpClient, StringContent jsonContent)
        {
            using HttpResponseMessage response = await httpClient.PostAsync(
                "api/telemetri_gonder",
                jsonContent);
        }
        
        using HttpResponseMessage

        var jsonResponse = await response
    }

    class Login(string kadi, string sifre)
    {
        string kadi { get; set; } = kadi;
        string sifre { get; set; } = sifre;
    }

}
