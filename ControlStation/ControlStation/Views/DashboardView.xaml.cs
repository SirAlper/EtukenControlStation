using ControlStation.ViewModels;
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

            this.DataContextChanged += (s, e) =>
            {
                if (DataContext is DashboardViewModel vm)
                {
                    // ViewModel'deki elçiye görevini veriyoruz: 
                    // "ViewModel haber verince, haritadaki UpdateMapPosition fonksiyonunu çalıştır"
                    vm.RequestMapUpdate = (data) =>
                    {
                        UpdateMapPosition(
                            data.IhaEnlem,
                            data.IhaBoylam,
                            data.IhaYonelme,
                            data.IhaIrtifa,
                            data.IhaHiz
                        );
                    };
                }
            };
        }
        private async void InitializeMapAsync()
        {
            // WebView2 Motorunu Başlat
            await MapWebView.EnsureCoreWebView2Async(null);

            // Çıktı dizinindeki map.html dosyasının tam yolunu bul ve yükle
            string htmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "map.html"); MapWebView.CoreWebView2.Navigate(htmlPath);
        }

        public async void UpdateMapPosition(double lat, double lng, double yaw, double alt, double speed)
        {
            if (MapWebView != null && MapWebView.CoreWebView2 != null)
            {
                // Sayıları JS'in anlayacağı formatta (noktalı) yollamak için InvariantCulture kullanıyoruz!
                string script = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "updateUAV({0}, {1}, {2}, {3}, {4})", lat, lng, yaw, alt, speed);

                await MapWebView.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        // ViewModel veya Telemetri Servisi üzerinden bu metodu çağırabilirsin
        
    }
}


    

