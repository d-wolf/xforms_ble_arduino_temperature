
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ArduinoBLETemperature.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectView : ContentPage
    {
        public ConnectView()
        {
            InitializeComponent();

            CircleScanner.Colors.Add((Color)Application.Current.Resources["LightTeal"]);
            CircleScanner.Colors.Add((Color)Application.Current.Resources["Teal"]);
        }
    }
}