using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlStation.Views
{
    /// <summary>
    /// DashboardView.xaml etkileşim mantığı
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            InitializeMapAsync();
        }
        private async void InitializeMapAsync()
        {
            // WebView2 Motorunu Başlat
            await MapWebView.EnsureCoreWebView2Async(null);

            // Çıktı dizinindeki map.html dosyasının tam yolunu bul ve yükle
            string htmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html"); MapWebView.CoreWebView2.Navigate(htmlPath);
        }

        // ViewModel veya Telemetri Servisi üzerinden bu metodu çağırabilirsin
        public async void SendOwnTelemetryToMap(double lat, double lng, double yaw, double alt, double speed)
        {
            if (MapWebView != null && MapWebView.CoreWebView2 != null)
            {
                // JavaScript'teki 'updateOwnUAV' fonksiyonunu C# içinden tetikliyoruz!
                string script = $"updateOwnUAV({lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{yaw.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{alt.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{speed.ToString(System.Globalization.CultureInfo.InvariantCulture)});";

                await MapWebView.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        public async void SendEnemyTelemetryToMap(int enemyId, double lat, double lng, double yaw, double alt, double speed)
        {
            if (MapWebView != null && MapWebView.CoreWebView2 != null)
            {
                // Düşman uçağının verilerini haritaya gönderiyoruz
                string script = $"updateEnemyUAV({enemyId}, " +
                                $"{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{yaw.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{alt.ToString(System.Globalization.CultureInfo.InvariantCulture)}, " +
                                $"{speed.ToString(System.Globalization.CultureInfo.InvariantCulture)});";

                await MapWebView.CoreWebView2.ExecuteScriptAsync(script);
            }
        }
    }
}


    

