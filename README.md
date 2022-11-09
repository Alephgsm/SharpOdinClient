# SharpOdinClient
SharpOdinClient is a .NET library that allows .NET applications to communicate with samsung android devices in download mode.

A suitable client for flash(image , tar.md5 , lz4), getting info and implementing other features.

It provides a .NET implementation of the odin protocol.
# Platform
Windows .NET 4.5.1

# How does work?
USB communication in SharpOdinClient is serialport.

1. install Official Samsung usb driver
2. Connect your device in download mode 


# Subscribe for events
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
        }```


# Find Automatically samsung devices in download mode

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
        }```

# Read Info from device
```public async Task ReadOdinInfo()
        {
            if(await Odin.FindAndSetDownloadMode())
            {
                //get info from device
                var info = await Odin.DVIF();
                await Odin.PrintInfo();
            }
        }```

in `info` variable we get dictionary of `string` , `string`
The concept of some 'keys'
+ `capa` = Capa Number
+ 'product' = Product Id
+ `model` = Model Number
+ `fwver` = Firmware Version
+ `vendor` = vendor
+ `sales` = Sales Code
+ `ver` = Build Number
+ `did` = did Number
+ `un` = Unique Id
+ `tmu_temp` = Tmu Number
+ `prov` = Provision
