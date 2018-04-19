using Plugin.BLE.Abstractions.Contracts;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ArduinoBLETemperature.ViewModels
{
    public class DeviceInfoViewViewModel : BindableBase, INavigationAware
    {
        private IAdapter _btAdapter;
        private IDevice _btDevice;

        private float _temperature;

        public float Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        public DeviceInfoViewViewModel(IAdapter btAdapter)
        {
            _btAdapter = btAdapter;
        }

        public async Task ConnectToDeviceAsync(IDevice bleDevice)
        {
            await _btAdapter.ConnectToDeviceAsync(bleDevice);
            var service = await bleDevice.GetServiceAsync(Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"));
            var characteristic = await service.GetCharacteristicAsync(Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"));

            characteristic.ValueUpdated += (o, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var bytes = args.Characteristic.Value;
                    float temperature = float.Parse(Encoding.Default.GetString(bytes), CultureInfo.InvariantCulture);
                    Temperature = temperature;
                });
            };

            await characteristic.StartUpdatesAsync();
        }

        public void OnNavigatedFrom(NavigationParameters parameters) { }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            _btDevice = parameters["Device"] as IDevice;
            ConnectToDeviceAsync(_btDevice);
        }

        public void OnNavigatingTo(NavigationParameters parameters) { }
    }
}
