using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib.Pit
{
	public class TPIT_Entry
	{
		public long MbinaryType
		{
			get
			{
				return this.binaryType;
			}
			set
			{
				this.binaryType = value;
			}
		}

		public long MdeviceType
		{
			get
			{
				return this.deviceType;
			}
			set
			{
				this.deviceType = value;
			}
		}

		public long Midentifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = value;
			}
		}

		public long Mattributes
		{
			get
			{
				return this.attributes;
			}
			set
			{
				this.attributes = value;
			}
		}

		public long MupdateAttributes
		{
			get
			{
				return this.updateAttributes;
			}
			set
			{
				this.updateAttributes = value;
			}
		}

		public long MblockSizeOrOffset
		{
			get
			{
				return this.blockSizeOrOffset;
			}
			set
			{
				this.blockSizeOrOffset = value;
			}
		}

		public long MblockCount
		{
			get
			{
				return this.blockCount;
			}
			set
			{
				this.blockCount = value;
			}
		}

		public long MfileOffset
		{
			get
			{
				return this.fileOffset;
			}
			set
			{
				this.fileOffset = value;
			}
		}

		public long MfileSize
		{
			get
			{
				return this.fileSize;
			}
			set
			{
				this.fileSize = value;
			}
		}

		public string MpartitionName
		{
			get
			{
				return this.partitionName;
			}
			set
			{
				this.partitionName = value;
			}
		}

		public string MflashFilename
		{
			get
			{
				return this.flashFilename;
			}
			set
			{
				this.flashFilename = value;
			}
		}

		public string MfotaFilename
		{
			get
			{
				return this.fotaFilename;
			}
			set
			{
				this.fotaFilename = value;
			}
		}


		private long binaryType;

		private long deviceType;

		private long identifier;

		private long attributes;

		private long updateAttributes;

		private long blockSizeOrOffset;

		private long blockCount;

		private long fileOffset;

		private long fileSize;

		private string partitionName;

		private string flashFilename;

		private string fotaFilename;
	}
}
