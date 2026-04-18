using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlStation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // Dependency Injection ile alt VM'leri alıyoruz
        private readonly DashboardViewModel _dashboardVM;
        private readonly SettingsViewModel _settingsVM; // Settings eklendi

        // Şu anki aktif sayfa
        [ObservableProperty]
        private object _currentView;

        // 2. DI Container, bu ViewModel'leri buraya otomatik getirecek
        public MainViewModel(DashboardViewModel dashboardVM, SettingsViewModel settingsVM)
        {
            _dashboardVM = dashboardVM;
            _settingsVM = settingsVM; // Atama yapıldı

            CurrentView = _dashboardVM; // Başlangıç sayfası
        }

        // Navigasyon Komutu
        [RelayCommand]
        private void Navigate(string viewName)
        {
            // 3. Çoklu sayfalar için switch kullanmak her zaman daha temizdir
            switch (viewName)
            {
                case "Dashboard":
                    CurrentView = _dashboardVM;
                    break;
                case "Settings":
                    CurrentView = _settingsVM; // Settings yönlendirmesi eklendi
                    break;
                    // İleride buraya: case "Mission": CurrentView = _missionVM; break; gelecek
            }
        }
    }
}
