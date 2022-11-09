# SharpOdinClient
SharpOdinClient is a .NET library that allows .NET applications to communicate with samsung android devices in download mode.

A suitable client for flash(image , tar.md5 , lz4), getting info and implementing other features.

It provides a .NET implementation of the odin protocol.
# Requirements:
+ .NET Framework 4.5.1
+ Official Samsung usb driver


# How does work?
USB communication in SharpOdinClient is serialport.

1. install Official Samsung usb driver
2. Connect your device in download mode 

## Namespaces
first add namespaces of SharpOdinClient on your project
```using SharpOdinClient;
using SharpOdinClient.structs;
using SharpOdinClient.util;
```

## Subscribe for events
```        private Odin Odin = new Odin();
        public MainWindow()
        {
            InitializeComponent();

            Odin.Log += Odin_Log;
            Odin.ProgressChanged += Odin_ProgressChanged;
        }

        private void Odin_ProgressChanged(string filename, long max, long value, long WritenSize)
        {
        }

        private void Odin_Log(string Text, SharpOdinClient.util.utils.MsgType Color)
        {
        }
```

## Find Automatically samsung devices in download mode

```  public async Task FindOdin()
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
```

## Read Info from device
```public async Task ReadOdinInfo()
        {
            if(await Odin.FindAndSetDownloadMode())
            {
                //get info from device
                var info = await Odin.DVIF();
                await Odin.PrintInfo();
            }
        }
```
in `info` variable we get dictionary of `string` , `string`
The concept of some 'keys'
+ `capa` = Capa Number
+ `product` = Product Id
+ `model` = Model Number
+ `fwver` = Firmware Version
+ `vendor` = vendor
+ `sales` = Sales Code
+ `ver` = Build Number
+ `did` = did Number
+ `un` = Unique Id
+ `tmu_temp` = Tmu Number
+ `prov` = Provision


## Read Pit from device
```public async Task ReadPit()
        {
            if(await Odin.FindAndSetDownloadMode())
            {
                await Odin.PrintInfo();
                if (await Odin.IsOdin())
                {
                    if(await Odin.LOKE_Initialize(0))
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
```

for doing any action in download mode , need first to check `IsOdin` and Run `LOKE_Initialize` argument, if you do not want to write anything on device set `LOKE_Initialize` `totalfilesize` parameter to zero(0) 

`buffer` = is byte array of pit from device , you can write this buffer on file for saving pit
`entry` = is list of partition information of your device

## Write Pit On Device
```
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
```

+ `pit` parameter = if you want to write pit from tar or tar.md5(Like CSC) file on device you can set your tar type file path , also you can set your pit single file with .pit format file

## Flash List Of tar.md5 package on device
```/// <summary>
        /// Add List Of Your tar package (bl,ap,cp,csc , or more)
        /// </summary>
        /// <param name="ListOfTarFile">add tar type files path in this list</param>
        /// <returns></returns>
        public async Task<bool> FlashFirmware(List<string> ListOfTarFile)
        {
            var FlashFile = new List<FileFlash>();
            foreach(var i in ListOfTarFile)
            {
                var item = Odin.tar.TarInformation(i);
                if(item.Count > 0)
                {
                    foreach (var Tiem in item)
                    {
                        if (!Exist(Tiem , FlashFile))
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

            if(FlashFile.Count > 0)
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
                            if(findPit != null)
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
```

for flashing tar,tar.md5 contains files(lz4 , image, bin and more ...) we need to create list of `FileFlash` from you tar package information.

`Enable` property in `FileFlash` is `bool` if you set this propery to false, SharpOdinClient does not Flash on the phone.

in `FlashFirmware` function , SharpOdinClient can write lz4 from contains of your tar package

## Flash Single File
You can Flash your single file like boot.img  or more files on partitions
```
        /// <summary>
        /// Flash Single File lz4 , image
        /// </summary>
        /// <param name="FilePath">path of your file</param>
        /// <param name="PartitionFileName">like boot.img , sboot.bin or more ...</param>
        /// <returns></returns>
        public async Task<bool> FlashSingleFile(string FilePath , string PartitionFileName)
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
```

# donate
theter TRC20
```TXZ1KviFtRzEiumVD8UCH1W7etJ2vM9VsQ```
