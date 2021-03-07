using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using _01_Neon.App_Data;
using System;
using Plugin.LocalNotifications;
using System.Collections.Generic;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace _01_Neon
{
    public partial class MainPage : ContentPage
    {
        bleNeon ble = null;
        Dictionary<string, int> estatusAccesorios = null;
        public MainPage()
        {
            InitializeComponent();
            ble = new bleNeon("6E400001-B5A3-F393-E0A9-E50E24DCCA9E", "6E400002-B5A3-F393-E0A9-E50E24DCCA9E");
            ble.OnDeviceConnect += onConnected;
            ble.OnDeviceDisconnect += onDisconnected;
            ble.InitConnectionAttempt += InitConnectionAttempt;
            ble.FinishConnectionAttempt += FinishConnectionAttempt;
            estatusAccesorios = new Dictionary<string, int>
            {
                { "Faros", 0 },
                { "Altas", 0 },
                { "Accesorios", 0 },
                { "QuartoD", 0 },
                { "QuartoI", 0 }
            };
            updateDeviceStatus(0);
            checkBLStatus(CrossBluetoothLE.Current.State);
            CrossBluetoothLE.Current.StateChanged += (s, e) => { checkBLStatus(e.NewState); };
        }

        private void updateDeviceStatus(int status)
        {
            if(status == 0)
            {
                statusDispositivo.TextColor = new Color(217,0,0);
                statusDispositivo.Text = "Desconectado";
            }
            if (status == 1)
            {
                statusDispositivo.TextColor = new Color(9, 122, 14);
                statusDispositivo.Text = "Conectado";
            }
        }

        public void checkBLStatus(BluetoothState bluetoothState)
        {
            if(bluetoothState == BluetoothState.TurningOff || bluetoothState == BluetoothState.Off)
            {
                pnlActivarBL.IsVisible = true;
                pnlGeneral.IsVisible = false;
                pnlCarga.IsVisible = false;
                lblStatusBL.Text = "Bluetooth DESACTIVADO";
            }
            else if (bluetoothState == BluetoothState.Unauthorized || bluetoothState == BluetoothState.Unavailable || bluetoothState == BluetoothState.Unknown)
            {
                pnlActivarBL.IsVisible = true;
                pnlGeneral.IsVisible = false;
                pnlCarga.IsVisible = false;
                lblStatusBL.Text = "La aplicación no tiene permisos para acceder al bluetooth o el bluetooth no está disponible";
            }
            else if (bluetoothState == BluetoothState.On)
            {
                pnlActivarBL.IsVisible = false;
                pnlGeneral.IsVisible = true;
                pnlCarga.IsVisible = false;
            }
        }

        public void onConnected(object sender, EventArgs e)
        {
            updateDeviceStatus(1);
        }

        public void onDisconnected(object sender, EventArgs e)
        {
            CrossLocalNotifications.Current.Show("Neoncito", "No te preocupes, he puesto los seguros 😉");
            updateDeviceStatus(0);
        }

        public void InitConnectionAttempt(object sender, EventArgs e)
        {
            pnlCarga.IsVisible = true;
            pnlGeneral.IsVisible = false;
        }
        public void FinishConnectionAttempt(object sender, EventArgs e)
        {
            pnlCarga.IsVisible = false;
            pnlGeneral.IsVisible = true;
        }


        private async void btnEncender_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            await ble.Write("002");
        }

        private async void btnApagar_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            await ble.Write("003");
        }

        private async void btnAccesorios_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            estatusAccesorios["Accesorios"] ^= 1;
            await ble.Write("13" + estatusAccesorios["Accesorios"].ToString());
        }

        private async void btnAbrirSeguros_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            await ble.Write("006");
        }

        private async void btnCerrarSeguros_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            await ble.Write("005");
        }

        private async void btnAbrirCajuela_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            await ble.Write("007");
        }

        private async void btnFaros_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            estatusAccesorios["Faros"] ^= 1;
            await ble.Write("06" + estatusAccesorios["Faros"].ToString());
        }

        private async void btnAltas_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            estatusAccesorios["Altas"] ^= 1;
            await ble.Write("05" + estatusAccesorios["Altas"].ToString());
        }

        private async void btnQuartoI_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            estatusAccesorios["QuartoI"] ^= 1;
            await ble.Write("08" + estatusAccesorios["QuartoI"].ToString());
        }

        private async void btnQuartoD_Clicked(object sender, EventArgs e)
        {
            await ble.ConnectToMyDevice();
            estatusAccesorios["QuartoD"] ^= 1;
            await ble.Write("07" + estatusAccesorios["QuartoD"].ToString());
        }
    }
}
