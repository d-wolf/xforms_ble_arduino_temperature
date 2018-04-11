using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
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

            CircleScanner.Colors.Add((Color)Application.Current.Resources["LightTeal"]);
            CircleScanner.Colors.Add((Color)Application.Current.Resources["Teal"]);

            _btAdapter = CrossBluetoothLE.Current.Adapter;
            _btAdapter.DeviceDiscovered += OnDeviceDiscovered;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            StatusLabel.Text = "Tap to scan";
            CircleScanner.IsActive = false;
            CircleScanner.Radius = 0;

            foreach (var device in _btAdapter.ConnectedDevices)
                _btAdapter.DisconnectDeviceAsync(device);


        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Device.Name) && e.Device.Name.Contains("HC-08"))
            {
                StatusLabel.Text = "Connecting...";
                Navigation.PushAsync(new DeviceInfoView(e.Device));
            }
        }

        private async void OnTapGestureRecognizerTapped(object sender, EventArgs args)
        {
            _isScanning = !_isScanning;
            CircleScanner.IsActive = _isScanning;

            if (_isScanning)
            {
                StatusLabel.Text = "Searching...";
                await _btAdapter.StartScanningForDevicesAsync();
            }
            else
            {
                await _btAdapter.StopScanningForDevicesAsync();
                StatusLabel.Text = "Tap to scan";
            }
        }
    }
}