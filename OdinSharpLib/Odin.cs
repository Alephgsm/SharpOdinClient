using OdinSharpLib.Port;
using OdinSharpLib.structs;
using OdinSharpLib.util;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OdinSharpLib.util.utils;

namespace OdinSharpLib
{
    public class Odin
    {

        /// <summary>
        /// connected device
        /// </summary>
        public device Device = new device();

        /// <summary>
        /// delegate log
        /// </summary>
        public event LogDelegate Log;

        /// <summary>
        /// Finding samsung devices download mode port 
        /// </summary>
        /// <returns>serialport information
        public async Task<ItypePort> FindDownloadModePort() => await PortComm.FindDownloadModePort();


        /// <summary>
        /// Find And set samsung devices in odin mode
        /// </summary>
        /// <returns>true if can find and registered</returns>
        public async Task<bool> FindAndSetDownloadMode()
        {
            var dev = await FindDownloadModePort();
            if (!string.IsNullOrEmpty(dev.COM))
            {
                return Device.RegisterPort(dev);
            }
            return false;
        }

        /// <summary>
        /// set download mode device manually
        /// </summary>
        /// <param name="device">serialport of your device</param>
        /// <returns>true if can be opened</returns>
        public bool SetDownloadMode(ItypePort device)
        {
            return Device.RegisterPort(device);
        }

        /// <summary>
        /// Get Device info
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> DVIF()
        {
            var Info = new Dictionary<string, string>();
            try
            {
                var DV =new byte[] { 0x44,0x56,0x49,0x46 };

                await Device.WritePort(DV, DV.Length);
                await Task.Delay(400);
                var Read = Device.Port.ReadExisting();
                if (!string.IsNullOrEmpty(Read))
                {
                    var array = Regex.Split(Read, ";");
                    foreach (var item in array)
                    {
                        var KeyValue = Regex.Split(item.Replace("#", null).Replace("@", null), "=");
                        if (string.IsNullOrEmpty(KeyValue[0]) || string.IsNullOrEmpty(KeyValue[1]))
                        {
                            continue;
                        }
                        Info.Add(KeyValue[0], KeyValue[1]);
                    }
                }
            }
            catch
            {
            }
            return Info;
        }

        /// <summary>
        /// GetDeviceInfo and print information
        /// </summary>
        /// <returns></returns>
        public async Task PrintInfo()
        {
            var info = await DVIF();
            foreach (var item in info)
            {
                switch (item.Key.ToLower())
                {
                    case "capa":
                        {
                            Log?.Invoke("Capa Number: ", MsgType.Message);
                            Log?.Invoke(item.Value , MsgType.Result);
                            break;
                        }
                    case "product":
                        {
                            Log?.Invoke("Product Id: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "model":
                        {
                            Log?.Invoke("Model Number: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "fwver":
                        {
                            Log?.Invoke("Firmware Version: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "vendor":
                        {
                            Log?.Invoke("vendor: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "sales":
                        {
                            Log?.Invoke("Sales Code: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "ver":
                        {
                            Log?.Invoke("Build Number: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "did":
                        {
                            Log?.Invoke("did Number: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "un":
                        {
                            Log?.Invoke("Unique Id: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "tmu_temp":
                        {
                            Log?.Invoke("Tmu Number: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                    case "prov":
                        {
                            Log?.Invoke("Provision: ", MsgType.Message);
                            Log?.Invoke(item.Value, MsgType.Result);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Check device is Odin mode
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsOdin()
        {
            byte[] array = new byte[] { 0x4F,0x44, 0x49, 0x4E };
            await Device.WritePort(array, 4);
            await Task.Delay(400);
            string text = Device.Port.ReadExisting();
            return text.Contains("LOKE");
        }

        /// <summary>
        /// stop all operation
        /// </summary>
        public void StopOperations()
        {
            utils.Stop = true;
        }
    }
}
