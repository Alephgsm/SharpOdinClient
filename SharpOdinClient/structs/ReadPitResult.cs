
using SharpOdinClient.Pit;
using System.Collections.Generic;

namespace SharpOdinClient.structs
{
    public struct ReadPitResult
    {
        public bool Result;
        public byte[] data;
        public string error;
        public List<TPIT_Entry> Pit;
    }
}
