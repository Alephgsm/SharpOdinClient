using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdinSharpLib.util
{
    public static class utils
    {
        public enum MsgType
        {
            Message,
            Result
        }
        public static bool Stop;
        public delegate void LogDelegate(string Text,  MsgType Color);
    }
}
