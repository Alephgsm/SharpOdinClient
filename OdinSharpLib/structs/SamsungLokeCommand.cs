using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib.structs
{
    public class SamsungLokeCommand : IEquatable<SamsungLokeCommand>
    {
        protected virtual Type EqualityContract
        {
            get
            {
                return typeof(SamsungLokeCommand);
            }
        }

        public int Cmd { get; set; }

        public int SeqCmd { get; set; }

        public long BinaryType { get; set; }

        public int SizeWritten { get; set; }

        public int Unknown { get; set; }

        public int DeviceId { get; set; }

        public int Identifier { get; set; }

        public int SessionEnd { get; set; }

        public int EfsClear { get; set; }

        public int BootUpdate { get; set; }

        public SamsungLokeCommand(int Cmd = 0, int SeqCmd = 0, long BinaryType = 0L, int SizeWritten = 0, int Unknown = 0, int DeviceId = 0, int Identifier = 0, int SessionEnd = 0, int EfsClear = 0, int BootUpdate = 0)
        {
            this.Cmd = Cmd;
            this.SeqCmd = SeqCmd;
            this.BinaryType = BinaryType;
            this.SizeWritten = SizeWritten;
            this.Unknown = Unknown;
            this.DeviceId = DeviceId;
            this.Identifier = Identifier;
            this.SessionEnd = SessionEnd;
            this.EfsClear = EfsClear;
            this.BootUpdate = BootUpdate;

        }

        public override string ToString()
        {
            StringBuilder stringBuilder;
            (stringBuilder = new StringBuilder()).Append("SamsungLokeCommand");
            stringBuilder.Append(" { ");
            if (PrintMembers(stringBuilder))
            {
                stringBuilder.Append(' ');
            }
            stringBuilder.Append('}');
            return stringBuilder.ToString();
        }

        protected virtual bool PrintMembers(StringBuilder builder)
        {
            builder.Append("Cmd = ");
            builder.Append(Cmd.ToString());
            builder.Append(", SeqCmd = ");
            builder.Append(SeqCmd.ToString());
            builder.Append(", BinaryType = ");
            builder.Append(BinaryType.ToString());
            builder.Append(", SizeWritten = ");
            builder.Append(SizeWritten.ToString());
            builder.Append(", Unknown = ");
            builder.Append(Unknown.ToString());
            builder.Append(", DeviceId = ");
            builder.Append(DeviceId.ToString());
            builder.Append(", Identifier = ");
            builder.Append(Identifier.ToString());
            builder.Append(", SessionEnd = ");
            builder.Append(SessionEnd.ToString());
            builder.Append(", EfsClear = ");
            builder.Append(EfsClear.ToString());
            builder.Append(", BootUpdate = ");
            builder.Append(BootUpdate.ToString());
            return true;
        }

        public static bool operator !=(SamsungLokeCommand left, SamsungLokeCommand right)
        {
            return !(left == right);
        }

        public static bool operator ==(SamsungLokeCommand left, SamsungLokeCommand right)
        {
            if ((object)left != right)
            {
                return left?.Equals(right) ?? false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return (((((((((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Cmd)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(SeqCmd)) * -1521134295 + EqualityComparer<long>.Default.GetHashCode(BinaryType)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(SizeWritten)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Unknown)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(DeviceId)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Identifier)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(SessionEnd)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(EfsClear)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(BootUpdate);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SamsungLokeCommand);
        }

        public virtual bool Equals(SamsungLokeCommand other)
        {
            if ((object)this != other)
            {
                if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(Cmd, other.Cmd) && EqualityComparer<int>.Default.Equals(SeqCmd, other.SeqCmd) && EqualityComparer<long>.Default.Equals(BinaryType, other.BinaryType) && EqualityComparer<int>.Default.Equals(SizeWritten, other.SizeWritten) && EqualityComparer<int>.Default.Equals(Unknown, other.Unknown) && EqualityComparer<int>.Default.Equals(DeviceId, other.DeviceId) && EqualityComparer<int>.Default.Equals(Identifier, other.Identifier) && EqualityComparer<int>.Default.Equals(SessionEnd, other.SessionEnd) && EqualityComparer<int>.Default.Equals(EfsClear, other.EfsClear))
                {
                    return EqualityComparer<int>.Default.Equals(BootUpdate, other.BootUpdate);
                }
                return false;
            }
            return true;
        }

        public virtual SamsungLokeCommand Clone()
        {
            return new SamsungLokeCommand(this);
        }

        protected SamsungLokeCommand(SamsungLokeCommand original)
        {
            Cmd = original.Cmd;
            SeqCmd = original.SeqCmd;
            BinaryType = original.BinaryType;
            SizeWritten = original.SizeWritten;
            Unknown = original.Unknown;
            DeviceId = original.DeviceId;
            Identifier = original.Identifier;
            SessionEnd = original.SessionEnd;
            EfsClear = original.EfsClear;
            BootUpdate = original.BootUpdate;
        }

        public void Deconstruct(out int Cmd, out int SeqCmd, out long BinaryType, out int SizeWritten, out int Unknown, out int DeviceId, out int Identifier, out int SessionEnd, out int EfsClear, out int BootUpdate)
        {
            Cmd = this.Cmd;
            SeqCmd = this.SeqCmd;
            BinaryType = this.BinaryType;
            SizeWritten = this.SizeWritten;
            Unknown = this.Unknown;
            DeviceId = this.DeviceId;
            Identifier = this.Identifier;
            SessionEnd = this.SessionEnd;
            EfsClear = this.EfsClear;
            BootUpdate = this.BootUpdate;
        }
    }
}
