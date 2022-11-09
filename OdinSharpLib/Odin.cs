using OdinSharpLib.Pit;
using OdinSharpLib.Port;
using OdinSharpLib.structs;
using OdinSharpLib.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static OdinSharpLib.util.utils;

namespace OdinSharpLib
{
    public class Odin
    {
        public Cmd cmd = new Cmd();
        public device Device = new device();
        public Tar tar = new Tar();
        public PITData PitTool = new PITData();

        public event LogDelegate Log;

        /// <summary>
        /// Finding samsung devices download mode port 
        /// </summary>
        /// <returns>serialport information
        public async Task<ItypePort> FindDownloadModePort() => await PortComm.FindDownloadModePort();
        public async Task<bool> LOKE_Initialize(long totalFileSize) => await cmd.LOKE_Initialize(this.Device, totalFileSize);


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



        public async Task<bool> PDAToNormal()
        {
            try
            {
                SamsungLokeCommand cmd2 = new SamsungLokeCommand(103);
                await cmd.LOKE_SendCMD(this.Device,cmd2);
                cmd2.SeqCmd = 1;
                await cmd.LOKE_SendCMD(this.Device, cmd2);
            }
            catch { }
            var watch = new Stopwatch();
            try
            {
                watch.Start();
                do
                {
                    if (!Device.Port.IsOpen)
                    {
                        return true;
                    }
                } while (watch.ElapsedMilliseconds < 60000);
            }
            finally
            {
                watch.Stop();
            }
            return !Device.Port.IsOpen;
        }

        public async Task<Result> Write_Pit(byte[] pit)
        {
            var Result = new Result { error = "Failed RePartition" };
            try
            {
                SamsungLokeCommand command = new SamsungLokeCommand(101);
                if (await cmd.LOKE_SendCMD(Device, command))
                {
                    command = new SamsungLokeCommand(101, 2, pit.Length);
                    if (await cmd.LOKE_SendCMD(Device, command))
                    {
                        await Device.WritePort(pit, pit.Length);
                        cmd.BufferRead = await Device.ReadPort(8);
                        if (cmd.BufferRead[0] != byte.MaxValue)
                        {
                            command = new SamsungLokeCommand(101, 3, pit.Length);
                            await cmd.LOKE_SendCMD(Device, command);
                            Result.status = true;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Result.error = ex.Message;
            }

            return Result;
        }

        public async Task<Result> Write_Pit(string File)
        {
            var Result = new Result { error = "Failed RePartition" };
            try
            {
                byte[] pit = new byte[] { };
                var Extension = Path.GetExtension(File).ToLower();
                if (Extension == ".tar" || Extension == ".md5")
                {

                    var TarInfo = tar.TarInformation(File);
                    var pitname = TarInfo.ToList().Find(item => item.Filename.ToLower().EndsWith(".pit"));
                    if (pitname != null)
                    {
                        pit = await tar.ExtractFileFromTar(File, pitname.Filename);
                    }
                    else
                    {
                        Result.error = "Cannot Find Pit In Tar";
                        return Result;
                    }
                }
                else if (Extension == ".pit")
                {
                    pit = System.IO.File.ReadAllBytes(File);
                }
                else
                {
                    Result.error = "Pit Is Invalid";
                    return Result;
                }

                SamsungLokeCommand command = new SamsungLokeCommand(101);
                if (await cmd.LOKE_SendCMD(Device, command))
                {
                    command = new SamsungLokeCommand(101, 2, pit.Length);
                    if (await cmd.LOKE_SendCMD(Device, command))
                    {
                        await Device.WritePort(pit,  pit.Length);
                        cmd.BufferRead = await Device.ReadPort(8);
                        if (cmd.BufferRead[0] != byte.MaxValue)
                        {
                            command = new SamsungLokeCommand(101, 3, pit.Length);
                            await cmd.LOKE_SendCMD(Device, command);
                            Result.status = true;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Result.error = ex.Message;
            }

            return Result;
        }

        public async Task<ReadPitResult> Read_Pit()
        {
            var Result = new ReadPitResult();
            try
            {
                using (var PitStream = new MemoryStream())
                {
                    byte[] array = new byte[1025];
                    byte[] array2 = new byte[4097];
                    string currentDirectory = Environment.CurrentDirectory;
                    var cmd = new SamsungLokeCommand(0x65, 1);
                    if (await this.cmd.LOKE_SendCMD(Device,cmd))
                    {
                        // 4 to 8
                        var pitSize = new byte[] { this.cmd.BufferRead[7], this.cmd.BufferRead[6], this.cmd.BufferRead[5], this.cmd.BufferRead[4] };
                        long num = (long)Convert.ToInt32(BitConverter.ToString(pitSize).Replace("-",""), 16);
                        int num2 = (int)(unchecked((double)num / 500.0 + 1.0));
                        int num3 = 0;
                        int num4 = num2 - 1;
                        for (int i = 0; i <= num4; i++)
                        {
                            int num5;
                            if (num - unchecked((long)num3) >= 500L)
                            {
                                num5 = 500;
                            }
                            else
                            {
                                num5 = (int)(num - unchecked((long)num3));
                            }
                            int num6 = 0;
                            do
                            {
                                array[num6] = 0;
                                num6++;
                            }
                            while (num6 <= 1023);
                            num6 = 0;
                            do
                            {
                                array2[num6] = 0;
                                num6++;
                            }
                            while (num6 <= 4096);
                            array[0] = 101;
                            array[1] = 0;
                            array[2] = 0;
                            array[3] = 0;
                            array[4] = 2;
                            array[5] = 0;
                            array[6] = 0;
                            array[7] = 0;
                            array[8] = (byte)(i % 256);
                            array[9] = (byte)(i / 256.0);
                            array[10] = (byte)(i / 65536.0);
                            array[11] = (byte)(i / 16777216.0);
                            num3 += num5;
                            await this.Device.WritePort(array,  1024);
                            int num7 = this.Device.Port.Read(array2, 0, num5);
                            PitStream.Write(array2, 0, num5);
                        }
                    }
                    byte[] sData = PitStream.ToArray();
                    Result.Result = true;
                    Result.data = sData;
                    if (PitTool.UNPACK_PIT(sData))
                    {
                        Result.Pit = PitTool.xPIT_Entry.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Result.error = e.Message;
            }
            return Result;
        }

      
    }
}
