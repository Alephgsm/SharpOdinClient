using OdinSharpLib.Port;
using OdinSharpLib.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib
{
    public class Cmd
    {
        public byte[] BufferRead = new byte[8];

        private long GetVariant(byte[] responseBuff)
        {
            return (BitConverter.ToInt32(responseBuff, 4) & 0xFFFF0000L) >> 16;
        }

        private byte[] GetCmdBuff(SamsungLokeCommand loke)
        {
            byte[] array = new byte[1024];
            Array.Copy(BitConverter.GetBytes(loke.Cmd), 0, array, 0, 4);
            Array.Copy(BitConverter.GetBytes(loke.SeqCmd), 0, array, 4, 4);
            if (loke.Cmd == 100)
            {
                Array.Copy(BitConverter.GetBytes(loke.BinaryType), 0, array, 8, 8);
            }
            else
            {
                Array.Copy(BitConverter.GetBytes((int)loke.BinaryType), 0, array, 8, 4);
                Array.Copy(BitConverter.GetBytes(loke.SizeWritten), 0, array, 12, 4);
            }
            Array.Copy(BitConverter.GetBytes(loke.Unknown), 0, array, 16, 4);
            Array.Copy(BitConverter.GetBytes(loke.DeviceId), 0, array, 20, 4);
            Array.Copy(BitConverter.GetBytes(loke.Identifier), 0, array, 24, 4);
            Array.Copy(BitConverter.GetBytes(loke.SessionEnd), 0, array, 28, 4);
            Array.Copy(BitConverter.GetBytes(loke.EfsClear), 0, array, 32, 4);
            Array.Copy(BitConverter.GetBytes(loke.BootUpdate), 0, array, 36, 4);
            return array;
        }
        public async Task<bool> LOKE_SendCMD(device device,SamsungLokeCommand Cmd, bool readresp = true)
        {
            byte[] cmdBuff = GetCmdBuff(Cmd);
            await device.WritePort(cmdBuff, cmdBuff.Length);
            Array.Clear(BufferRead, 0, 8);
            if (!readresp)
            {
                return true;
            }
            var num = device.Port.Read(BufferRead, 0, 8);
            if (num == 8 && this.BufferRead[0] != byte.MaxValue)
            {
                return true;
            }
            throw new Exception("Invalid LOKE response: 0xFF");
        }

        public async Task<bool> LOKE_Initialize(device device, long totalFileSize)
        {
            SamsungLokeCommand command = new SamsungLokeCommand(0x64, 0, 5L);
            if (await LOKE_SendCMD(device, command))
            {
                long variant = GetVariant(BufferRead);
                if (variant == 5L)
                {
                    command = new SamsungLokeCommand(0x64, 12);
                    await LOKE_SendCMD( device, command, false);
                }

                if (totalFileSize != 0)
                {
                    if (variant == 2L)
                    {
                        command = new SamsungLokeCommand(0x64, 2);
                        await LOKE_SendCMD(device,command);
                    }
                    if (variant == 3L || variant == 4L || variant == 5L)
                    {
                        command = new SamsungLokeCommand(0x64, 5, 1048576L);
                        await LOKE_SendCMD(device, command);
                    }

                    command = new SamsungLokeCommand(0x64, 2, totalFileSize);
                    await LOKE_SendCMD(device, command);
                    if (variant == 4L)
                    {
                        int i = 0;
                        while (i < 3)
                        {
                            command = new SamsungLokeCommand(0x69, i);
                            await LOKE_SendCMD(device, command);
                            int num = i + 1;
                            i = num;
                        }
                    }

                }

            }
            return true;
        }

    }
}
