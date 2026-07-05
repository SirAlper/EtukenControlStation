using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlStation.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;

namespace ControlStation.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ConnectionsService _tcpService;
        private RfService _rfService; 

       
        [ObservableProperty]
        private bool _isTcpTabActive = true;

        [ObservableProperty]
        private bool _isRfTabActive = false;

        
        [ObservableProperty]
        private string _targetIpAddress = "127.0.0.1";
        [ObservableProperty]
        private int _targetPort = 8000;

        
        [ObservableProperty]
        private ObservableCollection<string> _availablePorts = new();
        [ObservableProperty]
        private string _selectedPort;
        [ObservableProperty]
        private int _rfBaudRate = 115200;


        [ObservableProperty]
        private string _statusMessage = "Sistem Hazır. Bağlantı Bekleniyor...";
        [ObservableProperty]
        private bool _isConnected = false;

        public SettingsViewModel(ConnectionsService tcpService, RfService rfService)
        {
            _tcpService = tcpService;
            _rfService = rfService;
            RefreshPorts(); // Sayfa açılınca portları bul
        }

     
        [RelayCommand]
        private void RefreshPorts()
        {
            
            AvailablePorts.Clear();
            foreach (var port in SerialPort.GetPortNames())
            {            
                AvailablePorts.Add(port);
            }
            if (AvailablePorts.Count > 0) SelectedPort = AvailablePorts[0];
        }

       
        [RelayCommand]
        private async Task ConnectTcpAsync()
        {
            try
            {
                StatusMessage = $"TCP Bağlanılıyor... ({TargetIpAddress}:{TargetPort})";
                await _tcpService.ConnectAsync(TargetIpAddress, TargetPort);

                IsConnected = true;
                StatusMessage = "TCP BAĞLANTISI KURULDU!";
            }
            catch (Exception ex)
            {
                IsConnected = false;
                StatusMessage = $"TCP HATASI: {ex.Message}";
            }
        }

        
        [RelayCommand]
        private void ConnectRf()
        {
            if (string.IsNullOrEmpty(SelectedPort))
            {
                StatusMessage = "Lütfen bir COM portu seçin!";
                return;
            }

            try
            {
                StatusMessage = $"RF Bağlanılıyor... ({SelectedPort} @ {RfBaudRate})";

                
                _rfService.Connect(SelectedPort, RfBaudRate);

                // Hata event'ini burada dinleyebilirsin
                _rfService.ErrorReceived += (s, err) =>
                {
                    App.Current.Dispatcher.Invoke(() => StatusMessage = $"RF HATASI: {err}");
                };

                IsConnected = true;
                StatusMessage = "RF BAĞLANTISI KURULDU!";
            }

            catch (Exception ex)
            {
                IsConnected = false;
                StatusMessage = $"RF BAĞLANTI HATASI: {ex.Message}";
            }
        }

        [RelayCommand]
        private void Disconnect()
        {
            if (IsTcpTabActive) _tcpService.Disconnect();
            if (IsRfTabActive && _rfService != null) _rfService.Disconnect();

            IsConnected = false;
            StatusMessage = "Bağlantı Kesildi.";
        }
    }
}