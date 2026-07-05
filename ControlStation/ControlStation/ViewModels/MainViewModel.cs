using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlStation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        
        private readonly DashboardViewModel _dashboardVM;
        private readonly SettingsViewModel _settingsVM; 

        // Şu anki aktif sayfa
        [ObservableProperty]
        private object _currentView;

       
        public MainViewModel(DashboardViewModel dashboardVM, SettingsViewModel settingsVM)
        {
            _dashboardVM = dashboardVM;
            _settingsVM = settingsVM; 

            CurrentView = _dashboardVM; 
        }

        // Navigasyon Komutu
        [RelayCommand]
        private void Navigate(string viewName)
        {
            
            switch (viewName)
            {
                case "Dashboard":
                    CurrentView = _dashboardVM;
                    break;
                case "Settings":
                    CurrentView = _settingsVM; 
                    break;
                    
            }
        }
    }
}
