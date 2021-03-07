using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

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

        public async Task ConnectToMyDevice()
        {
            InitConnectionAttempt(null, null);
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
                                adapter.DeviceDisconnected += (s, a) =>
                                {
                                    connectedDevice = null;
                                    OnDeviceDisconnect(s, a);
                                };
                                adapter.DeviceConnected += (s, a) =>
                                {
                                    OnDeviceConnect(s, a);
                                };
                                adapter.DeviceConnectionLost += (s, a) =>
                                {
                                    connectedDevice = null;
                                    OnDeviceDisconnect(s, a);
                                };
                                var timeoutSeconds = 5;
                                var cancellationTokenSource = new CancellationTokenSource(delay: TimeSpan.FromSeconds(timeoutSeconds));
                                await adapter.ConnectToDeviceAsync(connectedDevice, new Plugin.BLE.Abstractions.ConnectParameters(autoConnect: true, forceBleTransport: true),cancellationToken: cancellationTokenSource.Token);
                                break;
                            }
                        }
                    }

                }
                else if (connectedDevice.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
                {
                    adapter.DeviceConnected += (s, a) =>
                    {
                        OnDeviceConnect(s, a);
                    };
                    adapter.DeviceDisconnected += (s, a) =>
                    {
                        connectedDevice = null;
                        OnDeviceDisconnect(s, a);
                    };
                    adapter.DeviceConnectionLost += (s, a) =>
                    {
                        connectedDevice = null;
                        OnDeviceDisconnect(s, a);
                    };
                    var timeoutSeconds = 5;
                    var cancellationTokenSource = new CancellationTokenSource(delay: TimeSpan.FromSeconds(timeoutSeconds));
                    await adapter.ConnectToDeviceAsync(connectedDevice, new Plugin.BLE.Abstractions.ConnectParameters(autoConnect: true, forceBleTransport: true), cancellationToken: cancellationTokenSource.Token);

                }
            }
            catch (Exception ex)
            {
                var e = ex.Message;
            }
            FinishConnectionAttempt(null, null);
        }

        public async Task Write(string data)
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


        public event EventHandler OnDeviceConnect;
        public event EventHandler OnDeviceDisconnect;
        public event EventHandler InitConnectionAttempt;
        public event EventHandler FinishConnectionAttempt;
    }
}

