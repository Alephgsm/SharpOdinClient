using SharpOdinClient.structs;
using SharpOdinClient.util;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace SharpOdinClient.Port
{
    public class device
    {

        /// <summary>
        /// port of selected devices;
        /// </summary>
        public SerialPort Port;

        /// <summary>
        /// Set your serialport
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>true if port can be opened</returns>
        public bool RegisterPort(ItypePort portName)
        {
            (Port = new SerialPort(portName.COM)).BaudRate = 115200;
            Port.Parity = Parity.None;
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;
            Port.Handshake = Handshake.RequestToSend;
            Port.DtrEnable = false;
            Port.RtsEnable = false;
            Port.ReadTimeout = 10000;
            Port.WriteTimeout = 10000;

            return Open();
        }


        /// <summary>
        /// write on seriaport
        /// </summary>
        /// <param name="data">buffer u want to write on device</param>
        /// <param name="len">length of buffer</param>
        /// <returns></returns>
        public async Task WritePort(byte[] data, int len)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (utils.Stop) throw new Exception("Stopped By User");
                    Port.Write(data, 0, len);
                }
                finally
                {
                    utils.Stop = false;
                }
            });
        }

        /// <summary>
        /// open Registered Port;
        /// </summary>
        /// <returns>true if can be opened</returns>
        private bool Open()
        {
            Port.Open();
            return true;
        }


        /// <summary>
        /// Read Response
        /// </summary>
        /// <param name="len">length of you needed</param>
        /// <returns>byte array</returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> ReadPort(int len = 0)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (utils.Stop) throw new Exception("Stopped By User");
                    if (len != 0)
                    {
                        var recvBuf = new byte[len];
                        Port.Read(recvBuf, 0, len);
                        return recvBuf;
                    }
                    else
                    {
                        var read = Port.BytesToRead;
                        if (read > 0)
                        {
                            var recvBuf = new byte[read];
                            Port.Read(recvBuf, 0, read);
                            return recvBuf;
                        }
                    }
                    return null;
                }
                finally
                {
                    util.utils.Stop = false;
                }
            });
        }



    }
}
