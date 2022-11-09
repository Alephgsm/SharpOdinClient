using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpOdinClient;
using SharpOdinClient.structs;
using SharpOdinClient.util;

namespace SharpOdinClient.test
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
        private Odin Odin = new Odin();
        public Program()
        {
            Odin.Log += Odin_Log;
            Odin.ProgressChanged += Odin_ProgressChanged;
        }

        private void Odin_ProgressChanged(string filename, long max, long value, long WritenSize)
        {
        }

        private void Odin_Log(string Text, SharpOdinClient.util.utils.MsgType Color)
        {
        }
        public async Task FindOdin()
        {
            //Find Auto odin device
            var device = await Odin.FindDownloadModePort();
            //device name
            Console.WriteLine(device.Name);

            // COM Port Of device 
            Console.WriteLine(device.COM);

            // VID and PID Of Device
            Console.WriteLine(device.VID);
            Console.WriteLine(device.PID);
        }


        public async Task ReadOdinInfo()
        {
            if (await Odin.FindAndSetDownloadMode())
            {
                //get info from device
                var info = await Odin.DVIF();
                await Odin.PrintInfo();
            }
        }

        public async Task ReadPit()
        {
            if (await Odin.FindAndSetDownloadMode())
            {
                await Odin.PrintInfo();
                if (await Odin.IsOdin())
                {
                    if (await Odin.LOKE_Initialize(0))
                    {
                        var Pit = await Odin.Read_Pit();
                        if (Pit.Result)
                        {
                            var buffer = Pit.data;
                            var entry = Pit.Pit;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// write pit file on your device
        /// </summary>
        /// <param name="pit">in this parameter, you can set tar.md5 contains have pit file(Like csc package of firmware)
        /// or pit file with .pit format
        /// </param>
        /// <returns>true if success</returns>
        public async Task<bool> Write_Pit(string pit)
        {
            if (await Odin.FindAndSetDownloadMode())
            {
                await Odin.PrintInfo();
                if (await Odin.IsOdin())
                {
                    if (await Odin.LOKE_Initialize(0))
                    {
                        var Pit = await Odin.Write_Pit(pit);
                        return Pit.status;
                    }
                }
            }
            return false;
        }


        public bool Exist(cListFileData File, List<FileFlash> FlashFile)
        {
            foreach (var item in FlashFile)
            {
                if (item.FileName == File.Filename)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add List Of Your tar package (bl,ap,cp,csc , or more)
        /// </summary>
        /// <param name="ListOfTarFile">add tar type files path in this list</param>
        /// <returns></returns>
        public async Task<bool> FlashFirmware(List<string> ListOfTarFile)
        {
            var FlashFile = new List<FileFlash>();
            foreach (var i in ListOfTarFile)
            {
                var item = Odin.tar.TarInformation(i);
                if (item.Count > 0)
                {
                    foreach (var Tiem in item)
                    {
                        if (!Exist(Tiem, FlashFile))
                        {
                            var Extension = System.IO.Path.GetExtension(Tiem.Filename);
                            var file = new FileFlash
                            {
                                Enable = true,
                                FileName = Tiem.Filename,
                                FilePath = i
                            };

                            if (Extension == ".pit")
                            {
                                //File Contains have pit
                            }
                            else if (Extension == ".lz4")
                            {
                                file.RawSize = Odin.CalculateLz4SizeFromTar(i, Tiem.Filename);
                            }
                            else
                            {
                                file.RawSize = Tiem.Filesize;
                            }
                            FlashFile.Add(file);
                        }
                    }
                }

            }

            if (FlashFile.Count > 0)
            {
                var Size = 0L;
                foreach (var item in FlashFile)
                {
                    Size += item.RawSize;
                }
                if (await Odin.FindAndSetDownloadMode())
                {
                    await Odin.PrintInfo();
                    if (await Odin.IsOdin())
                    {
                        if (await Odin.LOKE_Initialize(Size))
                        {
                            var findPit = FlashFile.Find(x => x.FileName.ToLower().EndsWith(".pit"));
                            if (findPit != null)
                            {
                                var res = MessageBox.Show("Pit Finded on your tar package , you want to repartition?", "", MessageBoxButton.YesNo);
                                if (res == MessageBoxResult.Yes)
                                {
                                    var Pit = await Odin.Write_Pit(findPit.FilePath);

                                }
                            }

                            var ReadPit = await Odin.Read_Pit();
                            if (ReadPit.Result)
                            {
                                var EfsClearInt = 0;
                                var BootUpdateInt = 1;
                                if (await Odin.FlashFirmware(FlashFile, ReadPit.Pit, EfsClearInt, BootUpdateInt, true))
                                {
                                    if (await Odin.PDAToNormal())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return false;
        }

        /// <summary>
        /// Flash Single File lz4 , image
        /// </summary>
        /// <param name="FilePath">path of your file</param>
        /// <param name="PartitionFileName">like boot.img , sboot.bin or more ...</param>
        /// <returns></returns>
        public async Task<bool> FlashSingleFile(string FilePath, string PartitionFileName)
        {
            var FlashFile = new FileFlash
            {
                Enable = true,
                FileName = PartitionFileName,
                FilePath = FilePath,
                RawSize = new FileInfo(FilePath).Length
            };

            if (await Odin.FindAndSetDownloadMode())
            {
                await Odin.PrintInfo();
                if (await Odin.IsOdin())
                {
                    if (await Odin.LOKE_Initialize(FlashFile.RawSize))
                    {
                        var ReadPit = await Odin.Read_Pit();
                        if (ReadPit.Result)
                        {
                            var EfsClearInt = 0;
                            var BootUpdateInt = 0;
                            if (await Odin.FlashSingleFile(FlashFile, ReadPit.Pit, EfsClearInt, BootUpdateInt, true))
                            {
                                if (await Odin.PDAToNormal())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }


            return false;
        }

    }
}
