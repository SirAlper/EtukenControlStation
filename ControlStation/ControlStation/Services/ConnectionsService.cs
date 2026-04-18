using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ControlStation.Services
{

    public class ConnectionsService
    {
        private TcpClient _client;

        // DI Container burayı kullanacak
        public ConnectionsService()
        {
        }

        // Arayüzden IP ve Port gelecek, biz de bağlanacağız
        public async Task ConnectAsync(string ipAddress, int port)
        {
            // Eski bağlantı varsa temizle
            if (_client != null && _client.Connected)
            {
                _client.Close();
            }

            _client = new TcpClient();
            var ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            // Senin asenkron bağlanma kodun
            await _client.ConnectAsync(ipEndPoint);
        }

        public void Disconnect()
        {
            _client?.Close();
            _client = null;
        }

        public bool IsConnected => _client?.Connected ?? false;
    }
}




