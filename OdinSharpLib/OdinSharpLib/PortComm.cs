using OdinSharpLib.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib
{
  
  
    public class PortComm
    {
        private static List<ItypePort> ListOfComPort = new List<ItypePort>();
       
        private static async Task UpdatePorts()
        {
            await Task.Run(() =>
            {
                ListOfComPort = new List<ItypePort>();
                try
                {
                    using (var PortSearcher = new ManagementObjectSearcher(@"root\CIMV2", @"Select * FROM Win32_PnPEntity WHERE Name Like '%(COM[0-9]%'"))
                    {
                        foreach (var queryObj in PortSearcher.Get())
                        {
                            string Caption = Convert.ToString(queryObj["Caption"]);
                            string deviceId = queryObj["DeviceID"].ToString();
                            int vidIndex = deviceId.IndexOf("VID_");
                            int pidIndex = deviceId.IndexOf("PID_");
                            int ComIndex = Caption.IndexOf("(") + 1;
                            string guidValue = queryObj["ClassGuid"].ToString();
                            var com = Caption.Substring(Caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
                            if (!string.IsNullOrEmpty(com))
                            {
                                var port = new ItypePort()
                                {
                                    GUID = guidValue,
                                    COM = com.Trim(),
                                    Type = PortType.COM_LPT,
                                    Name = Caption.Substring(0, ComIndex - 1)
                                };
                                if (vidIndex != -1 && pidIndex != -1)
                                {
                                    string startingAtVid = deviceId.Substring(vidIndex + 4);
                                    string VendorID = startingAtVid.Substring(0, 4);
                                    string startingAtPid = deviceId.Substring(pidIndex + 4);
                                    string ProductID = startingAtPid.Substring(0, 4);
                                    port.VID = VendorID;
                                    port.PID = ProductID;
                                }
                                ListOfComPort.Add(port);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }

                try
                {
                    using (var ModemSearcher = new ManagementObjectSearcher(@"root\CIMV2", "Select * FROM Win32_POTSModem "))
                    {
                        foreach (var queryObj in ModemSearcher.Get())
                        {
                            if (queryObj["Status"].ToString() == "OK")
                            {
                                string AttachedTo = queryObj["AttachedTo"].ToString();
                                string Caption = queryObj["Caption"].ToString();
                                string DeviceID = queryObj["DeviceID"].ToString();

                                int vidIndex = DeviceID.IndexOf("VID_");
                                int pidIndex = DeviceID.IndexOf("PID_");
                                var cmtype = new ItypePort()
                                {
                                    COM = AttachedTo,
                                    Name = Caption,
                                    Type = PortType.Modem,
                                };
                                if (vidIndex != -1 && pidIndex != -1)
                                {
                                    string startingAtVid = DeviceID.Substring(vidIndex + 4);
                                    string VendorID = startingAtVid.Substring(0, 4);
                                    string startingAtPid = DeviceID.Substring(pidIndex + 4);
                                    string ProductID = startingAtPid.Substring(0, 4);
                                    cmtype.VID = VendorID;
                                    cmtype.PID = ProductID;
                                }
                                ListOfComPort.Add(cmtype);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            });
        }


        /// <summary>
        /// finding serial port with vid , pid
        /// </summary>
        /// <param name="VIDPid">First value is PID and Last Is VID</param>
        /// <returns></returns>
        public static async Task<ItypePort> FindPortsWithVidPid(List<Tuple<string, string>> VIDPid)
        {
            try
            {
                await UpdatePorts();
                return ListOfComPort.Find(func =>
                VIDPid.Find(func2 => func.PID == func2.Item1.Trim().ToUpper() && func.VID == func2.Item2.Trim().ToUpper()) != null);
            }
            catch
            {
            }
            return new ItypePort();
        }
    }
}
