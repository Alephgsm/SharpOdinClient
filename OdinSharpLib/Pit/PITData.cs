using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib.Pit
{
	public enum TPitEntry
	{
		kDataSize = 132,
		kPartitionNameMaxLength = 32,
		kFlashFilenameMaxLength = 32,
		kFotaFilenameMaxLength = 32
	}
	public class PITData
	{
		public long UnpackInteger(byte[] data, long offset, bool ISWORDS_BIGENDIAN)
		{
			long result;
			if (ISWORDS_BIGENDIAN)
			{
				result = ((long)Math.Round(Conversion.Val(unchecked((byte)(data[checked((int)offset)] << (24 & 7))))) | (long)Math.Round(Conversion.Val(data[(int)(offset + 1L)])) << 16 | (long)Math.Round(Conversion.Val(data[(int)(offset + 2L)])) << 8 | (long)Math.Round(Conversion.Val(data[(int)(offset + 3L)])));
			}
			else
			{
				result = ((long)Math.Round(Conversion.Val(data[(int)offset])) | (long)Math.Round(Conversion.Val(data[(int)(offset + 1L)])) << 8 | (long)Math.Round(Conversion.Val(data[(int)(offset + 2L)])) << 16 | (long)Math.Round(Conversion.Val(data[(int)(offset + 3L)])) << 24);
			}
			return result;
		}

		public long UnpackShort(byte[] data, long offset, bool ISWORDS_BIGENDIAN)
		{
			checked
			{
				long result;
				if (ISWORDS_BIGENDIAN)
				{
					result = ((long)Math.Round(Conversion.Val(unchecked((byte)(data[checked((int)offset)] << (8 & 7))))) | (long)Math.Round(Conversion.Val(data[(int)(offset + 1L)])));
				}
				else
				{
					result = ((long)Math.Round(Conversion.Val(data[(int)offset])) | (long)Math.Round(Conversion.Val(data[(int)(offset + 1L)])) << 8);
				}
				return result;
			}
		}

		public string GetPartationname(byte[] data, int offset)
		{
			string text = "";
			int num = 0;
			checked
			{
				do
				{
					bool flag = Conversion.Val(data[offset + num]) == 0.0;
					if (flag)
					{
						text = (text ?? "");
					}
					else
					{
						text += Strings.Chr((int)data[offset + num]);
					}
					num++;
				}
				while (num <= 31);
				return text;
			}
		}

		public bool UNPACK_PIT(byte[] sData)
		{
			string text = "";
			bool flag = this.UnpackInteger(sData, 0L, false) == 305436790L;
			this.entryCount = this.UnpackInteger(sData, 4L, false);
			checked
			{
				Array.Resize<TPIT_Entry>(ref this.xPIT_Entry, (int)this.entryCount);
				long num = 8L;
				do
				{
					text += Strings.Chr((int)sData[(int)num]);
					num += 1L;
				}
				while (num <= 25L);

				this.unknown1 = this.UnpackInteger(sData, 8L, false);
				this.unknown2 = this.UnpackInteger(sData, 12L, false);
				this.unknown3 = this.UnpackShort(sData, 16L, false);
				this.unknown4 = this.UnpackShort(sData, 18L, false);
				this.unknown5 = this.UnpackShort(sData, 20L, false);
				this.unknown6 = this.UnpackShort(sData, 22L, false);
				this.unknown7 = this.UnpackShort(sData, 24L, false);
				this.unknown8 = (int)this.UnpackShort(sData, 26L, false);
				long num2 = this.entryCount - 1L;
				for (num = 0L; num <= num2; num += 1L)
				{
					long num3 = (long)Math.Round(unchecked(Conversion.Val(PITData.TFileIdentifier.kHeaderDataSize) + (double)num * Conversion.Val(TPitEntry.kDataSize)));
					TPIT_Entry tpit_Entry = new TPIT_Entry();
					tpit_Entry.MbinaryType = this.UnpackInteger(sData, num3, false);
					tpit_Entry.MdeviceType = this.UnpackInteger(sData, num3 + 4L, false);
					tpit_Entry.Midentifier = this.UnpackInteger(sData, num3 + 8L, false);
					tpit_Entry.Mattributes = this.UnpackInteger(sData, num3 + 12L, false);
					tpit_Entry.MupdateAttributes = this.UnpackInteger(sData, num3 + 16L, false);
					tpit_Entry.MblockSizeOrOffset = this.UnpackInteger(sData, num3 + 20L, false);
					tpit_Entry.MblockCount = this.UnpackInteger(sData, num3 + 24L, false);
					tpit_Entry.MfileOffset = this.UnpackInteger(sData, num3 + 28L, false);
					tpit_Entry.MfileSize = this.UnpackInteger(sData, num3 + 32L, false);
					tpit_Entry.MpartitionName = this.GetPartationname(sData, (int)(num3 + 36L));
					tpit_Entry.MflashFilename = this.GetPartationname(sData, (int)(num3 + 36L + 32L));
					tpit_Entry.MfotaFilename = this.GetPartationname(sData, (int)(num3 + 36L + 64L));
					this.xPIT_Entry[(int)num] = tpit_Entry;
				}

				int num4 = 0;
				Array.Resize<TPIT_Entry>(ref this.PIT_EntryOrdered, (int)this.entryCount);
				int num5 = 0;
				do
				{
					long num6 = this.entryCount - 1L;
					for (num = 0L; num <= num6; num += 1L)
					{
						bool flag2 = this.xPIT_Entry[(int)num].Midentifier == unchecked((long)num5);
						if (flag2)
						{
							this.PIT_EntryOrdered[num4] = this.xPIT_Entry[(int)num];
							num4++;
							text = PaddLog(this.xPIT_Entry[(int)num].MpartitionName, 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MbinaryType), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MdeviceType), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].Midentifier), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].Mattributes), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MupdateAttributes), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MblockSizeOrOffset), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MblockCount), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MfileOffset), 16);
							text += PaddLog(this.IntThex(this.xPIT_Entry[(int)num].MfileSize), 16);
							text += PaddLog(this.xPIT_Entry[(int)num].MflashFilename, 16);

						}
					}
					num5++;
				}
				while (num5 <= 255);
				return true;
			}
		}
		public string PaddLog(string s, int Lngth)
		{
			string text = "";
			checked
			{
				for (int i = 1; i <= Lngth; i++)
				{
					bool flag = i > s.Length;
					if (flag)
					{
						text += " ";
					}
					else
					{
						text += Strings.Mid(s, i, 1);
					}
				}
				return text;
			}
		}
		public string IntThex(long v)
		{
			string str = "00000000" + Conversion.Hex(v);
			return Strings.Right(str, 8);
		}

		public PITData()
		{
			this.entryCount = 0L;
			this.unknown1 = 0L;
			this.unknown2 = 0L;
			this.unknown3 = 0L;
			this.unknown4 = 0L;
			this.unknown5 = 0L;
			this.unknown6 = 0L;
			this.unknown7 = 0L;
			this.unknown8 = 0;
		}

		public TPIT_Entry[] xPIT_Entry;

		public TPIT_Entry[] PIT_EntryOrdered;
		private long entryCount;

		private long unknown1;

		private long unknown2;

		private long unknown3;

		private long unknown4;

		private long unknown5;

		private long unknown6;

		private long unknown7;

		private int unknown8;

		public enum TFileIdentifier
		{
			kFileIdentifier = 305436790,
			kHeaderDataSize = 28
		}
	}
}
