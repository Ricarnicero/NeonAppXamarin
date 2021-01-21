using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace _01_Neon.App_Data
{
    class bleNeon
    {
        private IDevice connectedDevice;
        private Guid ServiceUID;
        private Guid CharacteristcUID;
        public bleNeon(string ServiceUID, string CharacteristcUID)
        {
            connectedDevice = null;
            this.ServiceUID = new Guid(ServiceUID);
            this.CharacteristcUID = new Guid(CharacteristcUID);
        }

        public async void ConnectToMyDevice()
        {
            try
            {
                var adapter = CrossBluetoothLE.Current.Adapter;
                if (connectedDevice == null)
                {

                    var devices = adapter.GetSystemConnectedOrPairedDevices();
                    foreach (IDevice device in devices)
                    {
                        if (device.Name == "ESP32test")
                        {
                            connectedDevice = device;
                            if (connectedDevice.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
                            {
                                await adapter.ConnectToDeviceAsync(connectedDevice, new Plugin.BLE.Abstractions.ConnectParameters(autoConnect: true, forceBleTransport: true));
                                adapter.DeviceDisconnected += (s, a) =>
                                {
                                    connectedDevice = null;
                                };
                                break;
                            }

                        }
                    }
                }
                else if (connectedDevice.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
                {
                    await adapter.ConnectToDeviceAsync(connectedDevice, new Plugin.BLE.Abstractions.ConnectParameters(autoConnect: true, forceBleTransport: true));
                    adapter.DeviceDisconnected += (s, a) =>
                    {
                        connectedDevice = null;
                    };
                }
            }
            catch (Exception ex)
            {
                var e = ex.Message;
            }
        }

        public async void Write(string data)
        {
            try
            {
                var service = await connectedDevice.GetServiceAsync(ServiceUID);
                var characteristic = await service.GetCharacteristicAsync(CharacteristcUID);
                if (characteristic != null)
                {
                    if (characteristic.CanWrite)
                    {
                        byte[] rv = new byte[1 + data.Length];
                        rv[0] = 200;

                        var dataBytes = Encoding.ASCII.GetBytes(data);

                        dataBytes.CopyTo(rv, 1);

                        await characteristic.WriteAsync(rv);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("-----------------------------------------------------");
                Debug.WriteLine(ex.Message);
            }
        }

        public async void Write(byte[] data)
        {
            try
            {
                var service = await connectedDevice.GetServiceAsync(ServiceUID);
                var characteristic = await service.GetCharacteristicAsync(CharacteristcUID);
                if (characteristic != null)
                {
                    if (characteristic.CanWrite)
                    {
                        await characteristic.WriteAsync(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("-----------------------------------------------------");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}

