using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharpOdinClient.util.utils;

namespace SharpOdinClient.util
{
    public class cListFileData
    {
        public int Pos { get; set; }
        public string Filename { get; set; }
        public string Filetype { get; set; }
        public long Filesize { get; set; }
        public ulong FilePacketStart { get; set; }
        public ulong FilePacketEnd { get; set; }
        public long FilePosStart { get; set; }
        public long FilePosEnd { get; set; }
        public ulong FilePosCurrent { get; set; }
    }


    public class Tar
    {

        public IList<cListFileData> TarInformationFromArray(byte[] TarArray)
        {
            try
            {

                IList<cListFileData> ArrayListFileTAR = new List<cListFileData>();
                if (TarArray.Length > 0)
                {
                    using (var reader = new BinaryReader(new MemoryStream(TarArray)))
                    {
                        var Xreader = reader;
                        ulong j = 0UL;
                        while (j < (decimal)reader.BaseStream.Length)
                        {
                            var Buf = new byte[256];
                            int OctPower = 10;
                            string iFilename = "";
                            long iFileSize = 0L;
                            ulong iFilePacketStart = 0UL;
                            ulong iFilePacketEnd = 0UL;
                            long iFilePosStart = 0L;
                            long iFilePosEnd = 0L;
                            Xreader.BaseStream.Flush();
                            Xreader.BaseStream.Position = (long)j;
                            Xreader.Read(Buf, 0, 256);
                            if (Buf[0] == 0)
                            {
                                break;
                            }

                            for (int index = 0; index <= 100 - 1; index++)
                            {
                                if (Buf[index] > 0)
                                {
                                    iFilename = iFilename + char.ConvertFromUtf32(Buf[index]);
                                }
                            }

                            if (iFilename.Substring(iFilename.Length - 1) == "/")
                            {
                                iFileSize = 0L;
                                iFilePacketStart = j;
                                iFilePacketEnd = (ulong)Math.Round(j + 512m);
                                iFilePosStart = (long)Math.Round(j + 512m);
                                iFilePosEnd = iFilePosStart + iFileSize;
                                ArrayListFileTAR.Add(new cListFileData()
                                {
                                    Pos = 0,
                                    Filename = iFilename.Trim(),
                                    Filetype = "Directory",
                                    Filesize = iFileSize,
                                    FilePacketStart = iFilePacketStart,
                                    FilePacketEnd = iFilePacketEnd,
                                    FilePosStart = iFilePosStart,
                                    FilePosEnd = iFilePosEnd,
                                    FilePosCurrent = 0
                                });
                                j = (ulong)Math.Round(j + 512m);
                            }
                            else
                            {
                                for (int index = 124; index <= 124 + 12 - 1; index++)
                                {
                                    if (Buf[index] > 0)
                                    {
                                        ulong p = (ulong)Math.Round(Math.Pow(8d, OctPower));
                                        int val = (int)Math.Round(char.GetNumericValue((char)Buf[index]));
                                        if (val <= -1)
                                        {
                                            val = 1;
                                        }

                                        iFileSize = (long)Math.Round(iFileSize + val * (decimal)p);
                                    }

                                    OctPower = OctPower - 1;
                                }

                                iFilePacketStart = j;
                                iFilePosStart = (long)Math.Round(j + 512m);
                                iFilePosEnd = (long)Math.Round(iFilePosStart + iFileSize - 1m);
                                long d512 = (long)Math.Round(Math.Round(iFileSize / 512d));
                                long m512 = (long)Math.Round(iFileSize % 512m);
                                iFilePacketEnd = (ulong)Math.Round(j + 512m + iFileSize);
                                if (m512 > 0L)
                                {
                                    iFilePacketEnd = (ulong)Math.Round(iFilePacketEnd + (decimal)(512L - m512));
                                }

                                ArrayListFileTAR.Add(new cListFileData()
                                {
                                    Pos = 0,
                                    Filename = iFilename.Trim(),
                                    Filetype = "",
                                    Filesize = iFileSize,
                                    FilePacketStart = iFilePacketStart,
                                    FilePacketEnd = iFilePacketEnd,
                                    FilePosStart = iFilePosStart,
                                    FilePosEnd = iFilePosEnd,
                                    FilePosCurrent = 0
                                });
                                j = iFilePacketEnd;
                            }
                        }
                    }
                }

                return ArrayListFileTAR;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IList<cListFileData> TarInformation(string PathFile)
        {
            try
            {

                IList<cListFileData> ArrayListFileTAR = new List<cListFileData>();
                if (File.Exists(PathFile))
                {
                    using (var reader = new BinaryReader(File.Open(PathFile, FileMode.Open)))
                    {
                        var Xreader = reader;
                        ulong j = 0UL;
                        while (j < (decimal)reader.BaseStream.Length)
                        {
                            var Buf = new byte[256];
                            int OctPower = 10;
                            string iFilename = "";
                            long iFileSize = 0L;
                            ulong iFilePacketStart = 0UL;
                            ulong iFilePacketEnd = 0UL;
                            long iFilePosStart = 0L;
                            long iFilePosEnd = 0L;
                            Xreader.BaseStream.Flush();
                            Xreader.BaseStream.Position = (long)j;
                            Xreader.Read(Buf, 0, 256);
                            if (Buf[0] == 0)
                            {
                                break;
                            }

                            for (int index = 0; index <= 100 - 1; index++)
                            {
                                if (Buf[index] > 0)
                                {
                                    iFilename = iFilename + char.ConvertFromUtf32(Buf[index]);
                                }
                            }

                            if (iFilename.Substring(iFilename.Length - 1) == "/")
                            {
                                iFileSize = 0L;
                                iFilePacketStart = j;
                                iFilePacketEnd = (ulong)Math.Round(j + 512m);
                                iFilePosStart = (long)Math.Round(j + 512m);
                                iFilePosEnd = iFilePosStart + iFileSize;
                                ArrayListFileTAR.Add(new cListFileData()
                                {
                                    Pos = 0,
                                    Filename = iFilename.Trim(),
                                    Filetype = "Directory",
                                    Filesize = iFileSize,
                                    FilePacketStart = iFilePacketStart,
                                    FilePacketEnd = iFilePacketEnd,
                                    FilePosStart = iFilePosStart,
                                    FilePosEnd = iFilePosEnd,
                                    FilePosCurrent = 0
                                });
                                j = (ulong)Math.Round(j + 512m);
                            }
                            else
                            {
                                for (int index = 124; index <= 124 + 12 - 1; index++)
                                {
                                    if (Buf[index] > 0)
                                    {
                                        ulong p = (ulong)Math.Round(Math.Pow(8d, OctPower));
                                        int val = (int)Math.Round(char.GetNumericValue((char)Buf[index]));
                                        if (val <= -1)
                                        {
                                            val = 1;
                                        }

                                        iFileSize = (long)Math.Round(iFileSize + val * (decimal)p);
                                    }

                                    OctPower = OctPower - 1;
                                }

                                iFilePacketStart = j;
                                iFilePosStart = (long)Math.Round(j + 512m);
                                iFilePosEnd = (long)Math.Round(iFilePosStart + iFileSize - 1m);
                                long d512 = (long)Math.Round(Math.Round(iFileSize / 512d));
                                long m512 = (long)Math.Round(iFileSize % 512m);
                                iFilePacketEnd = (ulong)Math.Round(j + 512m + iFileSize);
                                if (m512 > 0L)
                                {
                                    iFilePacketEnd = (ulong)Math.Round(iFilePacketEnd + (decimal)(512L - m512));
                                }

                                ArrayListFileTAR.Add(new cListFileData()
                                {
                                    Pos = 0,
                                    Filename = iFilename.Trim(),
                                    Filetype = "",
                                    Filesize = iFileSize,
                                    FilePacketStart = iFilePacketStart,
                                    FilePacketEnd = iFilePacketEnd,
                                    FilePosStart = iFilePosStart,
                                    FilePosEnd = iFilePosEnd,
                                    FilePosCurrent = 0
                                });
                                j = iFilePacketEnd;
                            }
                        }
                    }
                }

                return ArrayListFileTAR;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public event ProgressChangeDelegate ProgressChanged;
        public async Task<byte[]> ExtractFileFromTar(string tar, string filename)
        {
            var info = TarInformation(tar);
            return await Task.Run(() =>
            {
                var ArrayByte = new List<byte>();
                if (info.Count > 0)
                {
                    foreach (var item in info)
                    {
                        if (item.Filename == filename)
                        {
                            using (var reader = new BinaryReader(File.Open(tar, FileMode.Open)))
                            {
                                var LenghtValue = 1024 * 1024;
                                var Buffer = new byte[LenghtValue];
                                var SizeReceived = 0L;
                                var pos = item.FilePosStart;

                                do
                                {
                                    reader.BaseStream.Flush();
                                    reader.BaseStream.Position = pos;
                                    var endValue = item.Filesize - SizeReceived;
                                    if (endValue < LenghtValue)
                                    {
                                        LenghtValue = (int)endValue;
                                        Buffer = new byte[LenghtValue];
                                    }
                                    reader.Read(Buffer, 0, LenghtValue);
                                    ArrayByte.AddRange(Buffer);
                                    SizeReceived += LenghtValue;
                                    pos += LenghtValue;
                                    ProgressChanged?.Invoke(item.Filesize, SizeReceived);

                                } while (SizeReceived < item.Filesize);
                            }
                        }
                    }
                }
                return ArrayByte.ToArray();
            });
        }

    }
}
