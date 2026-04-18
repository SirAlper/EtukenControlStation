using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ControlStation.Models
{
    public partial class TelemetryCard : ObservableObject
    {
        [ObservableProperty] private string _title;
        [ObservableProperty] private string _value;
        [ObservableProperty] private string _unit;
        [ObservableProperty] private string _colorHex;
    }
}