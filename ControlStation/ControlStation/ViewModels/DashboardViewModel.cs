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

        private readonly ConnectionsService _tcpService;

        private LibVLC _libVLC;

        [ObservableProperty]
        private MediaPlayer _videoPlayer;

        [ObservableProperty]
        private bool _isVideoRunning = false;

        [ObservableProperty]
        private string _connectionStatus = "Bağlantı Bekleniyor...";

        [ObservableProperty]
        private ObservableCollection<TelemetryCard> _telemetryCards;

        [ObservableProperty]
        private double pitchAngle;

        [ObservableProperty]
        private double rollAngle;

        [ObservableProperty]
        private double yawAngle;

        [ObservableProperty]
        private string _deadzoneLat = "0.0";

        [ObservableProperty]
        private string _deadzoneLong = "0.0";

        [ObservableProperty]
        private string _deadzoneRadius = "100";

        [ObservableProperty]
        private string _altitude = "0";

        [ObservableProperty]
        private string _waypoints = "-35.36460,149.16520,40,-35.36280,149.16790,50,-35.36090,149.17020,60,-35.35950,149.16780,50," +
            "-35.35890,149.16460,40,-35.36080,149.16310,50,-35.36300,149.16280,60,-35.36500,149.16510,50";

        private string _sonKayitYolu;


        public ObservableCollection<string> FlightLogs { get; } = new ObservableCollection<string>();

        public Action<IhaTelemetri> RequestMapUpdate { get; set; }
        public Action<List<RakipIha>> RequestRakiplerUpdate { get; set; }


        public ObservableCollection<string> FlightModes { get; } = new ObservableCollection<string>
        {
            "MANUAL",
            "STABILIZE",
            "LOITER",
            "AUTO",
            "RTL"
        };

        public DashboardViewModel(RfService rfService, ConnectionsService tcpService)
        {
            _rfService = rfService;
            _tcpService = tcpService;


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

        public void AddLog(string message)
        {

            string time = DateTime.Now.ToString("HH:mm:ss");
            string formattedLog = $"[{time}] {message}";


            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {

                FlightLogs.Insert(0, formattedLog);


                if (FlightLogs.Count > 50)
                {
                    FlightLogs.RemoveAt(FlightLogs.Count - 1);
                }
            });
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

                        PitchAngle = data.IhaDikilme;
                        RollAngle = data.IhaYatis;
                        YawAngle = data.IhaYonelme;

                        RequestMapUpdate?.Invoke(data);
                    }

                    if (data.Rakipler != null && data.Rakipler.Count > 0)
                    {
                        RequestRakiplerUpdate?.Invoke(data.Rakipler);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"JSON Çevirme Hatası: {ex.Message}");
                }
            });
        }

        [RelayCommand]
        private async Task SetDeadzone()
        {
            // Arayüzden gelen verilerle komutu birleştirip gönderir
            string fullCommand = $"deadzone set {DeadzoneLat} {DeadzoneLong} {DeadzoneRadius}";
            await _tcpService.SendCommandAsync(fullCommand);
        }

        [RelayCommand]
        private async Task Takeoff()
        {
            // Arayüzden gelen verilerle komutu birleştirip gönderir
            string fullCommand = $"takeoff {Altitude}";
            await _tcpService.SendCommandAsync(fullCommand);
        }

        [RelayCommand]
        private async Task ListAdd()
        {
            string fullCommand = $"list add {Waypoints}";
            await _tcpService.SendCommandAsync(fullCommand);
        }




        [RelayCommand]
        private void ChangeMode(string mode)
        {

            _rfService.SendMessage(mode);
            System.Diagnostics.Debug.WriteLine($"Uçağa Mod Komutu Gönderildi: {mode}");
        }

        [RelayCommand]
        private async Task ChangeModeTCP(string mode)
        {



            await _tcpService.SendCommandAsync(mode);
            AddLog($"BAŞARILI: {mode.ToUpper()} uçağa ulaştı."); // LOG EKLEDİK
            MessageBox.Show($"Uçağa şu komut gönderildi: {mode}");
        }

        [RelayCommand]
        private async Task ToggleVideo() // DİKKAT: async eklendi!
        {
            Debug.WriteLine("Buton calisti");
            if (IsVideoRunning)
            {
                // 1. Oynatıcıyı ve Kaydı Durdur
                VideoPlayer.Stop();
                IsVideoRunning = false;
                ConnectionStatus = "VİDEO VE KAYIT DURDURULDU.";

                // 2. BÜYÜ BURADA: KAYDEDİLEN DOSYAYI OTOMATİK AÇ!
                if (!string.IsNullOrEmpty(_sonKayitYolu) && System.IO.File.Exists(_sonKayitYolu))
                {
                    try
                    {
                        Debug.WriteLine($"🎬 Kayıt tamamlandı, VLC'nin dosyayı serbest bırakması bekleniyor...");

                        // VLC'nin dosya kilidini (File Lock) kaldırması için 1 saniye bekle
                        await Task.Delay(1000);

                        Debug.WriteLine($"🎬 Video açılıyor: {_sonKayitYolu}");

                        // UseShellExecute=true, dosyayı sanki sen çift tıklamışsın gibi Windows'ta varsayılan programla açar.
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = _sonKayitYolu,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"⚠️ Video otomatik açılamadı: {ex.Message}");
                    }
                }
            }
            else
            {
                // Yer istasyonu bilgisayarı 5000 portundan gelen tüm UDP paketlerini dinler
                var media = new Media(_libVLC, "udp://@:5000", FromType.FromLocation);

                // Gecikmeyi düşürmek için efsanevi VLC parametreleri:
                media.AddOption(":network-caching=150");
                media.AddOption(":clock-jitter=0");
                media.AddOption(":clock-synchro=0");
                media.AddOption(":avcodec-hw=any"); // GPU hızlandırma
                media.AddOption(":live-caching=0");

                // Dosya ismini oluştur ve Sınıf Seviyesindeki değişkene ata
                string dosyaAdi = $"IHA_Ucus_{DateTime.Now:yyyyMMdd_HHmmss}.ts";
                _sonKayitYolu = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dosyaAdi);

                // VLC bazı durumlarda Windows yollarındaki '\' işaretini sevmez, '/' yaparız:
                string vlcIcinGoreceliYol = _sonKayitYolu.Replace("\\", "/");

                // VLC'ye emri veriyoruz: Görüntüyü ÇOĞALT! Birini ekrana (display) ver, diğerini dosyaya (file) yaz!
                media.AddOption($":sout=#duplicate{{dst=display,dst=std{{access=file,mux=ts,dst='{vlcIcinGoreceliYol}'}}}}");

                VideoPlayer.Play(media);
                IsVideoRunning = true;

                ConnectionStatus = $"CANLI YAYIN VE KAYIT BAŞLADI!\nDosya: {dosyaAdi}";
                System.Diagnostics.Debug.WriteLine($"🎥 Video kaydediliyor: {_sonKayitYolu}");
            }
        }
    }
}