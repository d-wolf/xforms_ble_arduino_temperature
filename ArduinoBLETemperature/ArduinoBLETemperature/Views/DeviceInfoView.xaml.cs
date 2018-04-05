using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArduinoBLETemperature.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceInfoView : ContentPage
    {
        public DeviceInfoView(IDevice bleDevice)
        {
            InitializeComponent();
            ConnectToDeviceAsync(bleDevice);
        }

        public async Task ConnectToDeviceAsync(IDevice bleDevice)
        {
            var adapter = CrossBluetoothLE.Current.Adapter;
            await adapter.ConnectToDeviceAsync(bleDevice);
            var service = await bleDevice.GetServiceAsync(Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"));
            var characteristic = await service.GetCharacteristicAsync(Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"));

            characteristic.ValueUpdated += (o, args) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var bytes = args.Characteristic.Value;
                    float temperature = float.Parse(Encoding.Default.GetString(bytes), CultureInfo.InvariantCulture);
                    Thermometer.Temperature = temperature;
                });

            };

            await characteristic.StartUpdatesAsync();
        }
    }
}