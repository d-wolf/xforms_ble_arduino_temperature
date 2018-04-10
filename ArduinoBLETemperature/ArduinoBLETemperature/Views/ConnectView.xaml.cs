using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArduinoBLETemperature.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectView : ContentPage
    {
        private bool _isScanning = false;
        private IAdapter _btAdapter;

        public ConnectView()
        {
            InitializeComponent();

            CircleScanner.Colors.Add((Color)Application.Current.Resources["Orange"]);
            CircleScanner.Colors.Add((Color)Application.Current.Resources["Yellow"]);

            _btAdapter = CrossBluetoothLE.Current.Adapter;
            _btAdapter.DeviceDiscovered += (s, a) =>
            {
                if (!string.IsNullOrWhiteSpace(a.Device.Name) && a.Device.Name.Contains("HC-08"))
                    Navigation.PushAsync(new DeviceInfoView(a.Device));
            };
        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            _isScanning = !_isScanning;
            CircleScanner.IsActive = _isScanning;

            if (_isScanning)
            {
                StatusLabel.Text = "Scanning...";
                await _btAdapter.StartScanningForDevicesAsync();
            }
            else
            {
                StatusLabel.Text = "Tap to scan";
                await _btAdapter.StopScanningForDevicesAsync();
            }
        }
    }
}