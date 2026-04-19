using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ControlStation.Services
{

    public class ConnectionsService
    {
        private TcpClient _client;
        private NetworkStream _stream;

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
            _stream = _client.GetStream();
        }

        public async Task<bool> SendCommandAsync(string message)
        {
            // Bağlantı yoksa veya koptuysa boşa işlem yapma
            if (_client == null || !_client.Connected || _stream == null)
                return false;

            try
            {
                // Göndereceğimiz metni Byte dizisine çeviriyoruz
                byte[] data = Encoding.UTF8.GetBytes(message);

                // Veriyi ağa yaz
                await _stream.WriteAsync(data, 0, data.Length);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Veri Gönderilemedi: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            _client?.Close();
            _client = null;
        }

        public bool IsConnected => _client?.Connected ?? false;
    }
}




