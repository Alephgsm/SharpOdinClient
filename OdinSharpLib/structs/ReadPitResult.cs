
using OdinSharpLib.Pit;
using System.Collections.Generic;

namespace OdinSharpLib.structs
{
    public struct ReadPitResult
    {
        public bool Result;
        public byte[] data;
        public string error;
        public List<TPIT_Entry> Pit;
    }
}
