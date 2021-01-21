using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using _01_Neon.App_Data;

namespace _01_Neon
{
    public partial class MainPage : ContentPage
    {
        bleNeon ble = null;
        public MainPage()
        {
            InitializeComponent();
            ble = new bleNeon("6E400001-B5A3-F393-E0A9-E50E24DCCA9E", "6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
        }

        //await Task.Delay(300);
        private void btnEncendidoNormal_Clicked(object sender, System.EventArgs e)
        {
            ble.Write("002");
        }

        private void btnEncendidoApagar_Clicked(object sender, System.EventArgs e)
        {
            ble.Write("003");
        }
        private void btnEncendidoChido_Clicked(object sender, System.EventArgs e)
        {
            ble.Write("004");
        }

        private void btnSegurosPoner_Clicked(object sender, System.EventArgs e)
        {
            ble.Write("005");
        }

        private void btnSegurosQuitar_Clicked(object sender, System.EventArgs e)
        {
            ble.Write("006");
        }

        private void btnConectar_Clicked(object sender, System.EventArgs e)
        {
            ble.ConnectToMyDevice();
        }

        private void btnCajuela_Clicked(object sender, System.EventArgs e)
        {
            ble.Write("007");
        }
    }
}
