using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlStation.Models;
using ControlStation.Services;
using LibVLCSharp.Shared;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ControlStation.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly RfService _rfService;

        private LibVLC _libVLC;

        [ObservableProperty]
        private MediaPlayer _videoPlayer;

        [ObservableProperty]
        private bool _isVideoRunning = false;

        [ObservableProperty]
        private string _connectionStatus = "Bağlantı Bekleniyor...";

        [ObservableProperty]
        private ObservableCollection<TelemetryCard> _telemetryCards;

        


        public ObservableCollection<string> FlightModes { get; } = new ObservableCollection<string>
        {
            "MANUAL",
            "STABILIZE",
            "LOITER",
            "AUTO",
            "RTL"
        };

        public DashboardViewModel(RfService rfService)
        {
            _rfService = rfService;

            
            TelemetryCards = new ObservableCollection<TelemetryCard>
            {
                new TelemetryCard { Title = "İRTİFA", Value = "0.0", Unit = "m", ColorHex = "#00FF66" },
                new TelemetryCard { Title = "HIZ", Value = "0.0", Unit = "m/s", ColorHex = "#00E5FF" },
                new TelemetryCard { Title = "BATARYA", Value = "0", Unit = "%", ColorHex = "#FFCC00" },
                new TelemetryCard { Title = "PITCH", Value = "0.0", Unit = "°", ColorHex = "#FFFFFF" },
                new TelemetryCard { Title = "ROLL", Value = "0.0", Unit = "°", ColorHex = "#FFFFFF" },
                new TelemetryCard { Title = "YAW", Value = "0.0", Unit = "°", ColorHex = "#FFFFFF" },
                new TelemetryCard { Title = "UÇUŞ MODU", Value = "BEKLİYOR", Unit = "", ColorHex = "#FF4444" }
            };

            
            _rfService.DataRecieved += OnRfDataReceived;

            _libVLC = new LibVLC();
            VideoPlayer = new MediaPlayer(_libVLC);



        }

        private void OnRfDataReceived(object sender, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    ConnectionStatus = "Veri Akışı Aktif";
                    System.Diagnostics.Debug.WriteLine($"Dashboard Veri Yakaladı: {message}");

                    
                    var data = JsonSerializer.Deserialize<IhaTelemetri>(message);

                    if (data != null)
                    {
                      
                        TelemetryCards.First(c => c.Title == "İRTİFA").Value = data.IhaIrtifa.ToString("F1");
                        TelemetryCards.First(c => c.Title == "HIZ").Value = data.IhaHiz.ToString("F1");
                        TelemetryCards.First(c => c.Title == "BATARYA").Value = data.IhaBatarya.ToString();
                        TelemetryCards.First(c => c.Title == "PITCH").Value = data.IhaDikilme.ToString("F1");
                        TelemetryCards.First(c => c.Title == "ROLL").Value = data.IhaYatis.ToString("F1");
                        TelemetryCards.First(c => c.Title == "YAW").Value = data.IhaYonelme.ToString("F1");

                       
                        var modeCard = TelemetryCards.First(c => c.Title == "UÇUŞ MODU");
                        modeCard.Value = data.IhaOtonom == 1 ? "OTONOM" : "MANUEL";
                        modeCard.ColorHex = data.IhaOtonom == 1 ? "#00E5FF" : "#FF4444";
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"JSON Çevirme Hatası: {ex.Message}");
                }
            });
        }

       

        [RelayCommand]
        private void ChangeMode(string mode)
        {
           
            _rfService.SendMessage(mode);
            System.Diagnostics.Debug.WriteLine($"Uçağa Mod Komutu Gönderildi: {mode}");
        }

        [RelayCommand]
        private void ToggleVideo()
        {
            Debug.WriteLine("Buton calisti");
            if (IsVideoRunning)
            {
                VideoPlayer.Stop();
                IsVideoRunning = false;
            }
            else
            {
                // İşlenmiş İHA videosunu UDP 5000 portundan sıfır gecikmeyle (low-latency) al!
                var media = new Media(_libVLC, "udp://@127.0.0.1:5000", FromType.FromLocation);

                // Gecikmeyi düşürmek için efsanevi VLC parametreleri:
                media.AddOption(":network-caching=300");
                media.AddOption(":clock-jitter=0");
                media.AddOption(":clock-synchro=0");
                media.AddOption(":avcodec-hw=any");


                VideoPlayer.Play(media);
                IsVideoRunning = true;
            }
        }


    }
}