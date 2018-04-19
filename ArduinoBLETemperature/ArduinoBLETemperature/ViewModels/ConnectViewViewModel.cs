using ArduinoBLETemperature.Views;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Windows.Input;

namespace ArduinoBLETemperature.ViewModels
{
    public class ConnectViewViewModel : BindableBase, INavigationAware
    {
        private INavigationService _navigationService;
        private IAdapter _btAdapter;
        private IDevice _btDevice;

        public ICommand TapGestureRecognizerTappedCommand { get; set; }

        private bool _isScanning;

        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value); }
        }

        private string _statusText;

        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public ConnectViewViewModel(INavigationService navigationService, IAdapter btAdapter)
        {
            _navigationService = navigationService;
            _btAdapter = btAdapter;
            _btAdapter.DeviceDiscovered += OnDeviceDiscovered;
            TapGestureRecognizerTappedCommand = new DelegateCommand(OnTapGestureRecognizerTapped);
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Device.Name) && e.Device.Name.Contains("HC-08"))
            {
                _btDevice = e.Device;
                StatusText = "Connecting...";
                NavigationParameters p = new NavigationParameters
                {
                    { "Device", _btDevice }
                };
                
                _navigationService.NavigateAsync(new Uri(nameof(DeviceInfoView), UriKind.Relative), p);
            }
        }

        private async void OnTapGestureRecognizerTapped()
        {
            IsScanning = !IsScanning;
           
            if (_isScanning)
            {
                StatusText = "Searching...";
                await _btAdapter.StartScanningForDevicesAsync();
            }
            else
            {
                await _btAdapter.StopScanningForDevicesAsync();
                StatusText = "Tap to scan";
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (_btDevice != null) _btAdapter.DisconnectDeviceAsync(_btDevice);
            StatusText = "Tap to scan.";
            IsScanning = false;
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }
    }
}
