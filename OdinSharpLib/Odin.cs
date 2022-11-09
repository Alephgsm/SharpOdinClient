using OdinSharpLib.Port;
using OdinSharpLib.structs;
using OdinSharpLib.util;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib
{
    public class Odin
    {
        public device Device = new device();
        public async Task<ItypePort> FindDownloadModePort() => await PortComm.FindDownloadModePort();

        public async Task<bool> FindAndSetDownloadMode()
        {
            var dev = await FindDownloadModePort();
            if (!string.IsNullOrEmpty(dev.COM))
            {
                return Device.RegisterPort(dev);
            }
            return false;
        }

        public void StopOperation()
        {
            utils.Stop = true;
        }
    }
}
