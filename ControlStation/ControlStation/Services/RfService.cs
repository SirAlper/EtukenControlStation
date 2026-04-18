using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ControlStation.Services
{
    public class RfService
    {
        private SerialPort _serialPort;
        private Thread _readThread;
        private bool _keepReading;

        public event EventHandler<string> DataRecieved;

        public event EventHandler<string> ErrorReceived;

        public RfService()
        {
        }

       
        public void Connect(string portName, int baudRate = 115200)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                try
                {
                    
                    _serialPort = new SerialPort(portName, baudRate);
                    _serialPort.Parity = Parity.None;
                    _serialPort.DataBits = 8;
                    _serialPort.StopBits = StopBits.One;
                    _serialPort.Handshake = Handshake.None;
                    _serialPort.ReadTimeout = 500;
                    _serialPort.WriteTimeout = 500;

                    _serialPort.Open();
                    _keepReading = true;

                    _readThread = new Thread(ReadPort) { IsBackground = true };
                    _readThread.Start();
                }
                catch (Exception ex)
                {
                    ErrorReceived?.Invoke(this, $"Bağlantı açılamadı: {ex.Message}");
                }
            }
        }
        public void Disconnect()
        {
            _keepReading = false;

            if (_readThread != null && _readThread.IsAlive)
            {
                _readThread.Join(500);
            }
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
        private void ReadPort()
        {
            while (_keepReading)
            {
                try
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        string message = _serialPort.ReadLine();

                        DataRecieved?.Invoke(this, message);
                    }
                }
                catch (TimeoutException)
                {

                }
                catch (Exception ex)
                {
                    if (_keepReading)
                    {
                        ErrorReceived?.Invoke(this, $"Okuma hatasi: {ex.Message}");
                    }
                }
            }
        }
        public void SendMessage(string message)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _serialPort.WriteLine(message); 
                }
                catch(Exception ex)
                {
                    ErrorReceived?.Invoke(this, $"Mesaj gönderilemedi: {ex.Message}");
                }
            }
        }
    }
}
