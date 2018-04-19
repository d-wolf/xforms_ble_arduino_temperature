using ArduinoBLETemperature.ViewModels;
using ArduinoBLETemperature.Views;
using Plugin.BLE;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using System;

using Xamarin.Forms;

namespace ArduinoBLETemperature
{
    public partial class App : PrismApplication
    {
		public App (IPlatformInitializer initializer = null) : base(initializer)
		{
		}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(CrossBluetoothLE.Current.Adapter);
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<ConnectView, ConnectViewViewModel>();
            containerRegistry.RegisterForNavigation<DeviceInfoView, DeviceInfoViewViewModel>();
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync(new Uri("http://www.ArduinoBLETemperature/NavigationPage/ConnectView", UriKind.Absolute));
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
    }
}
