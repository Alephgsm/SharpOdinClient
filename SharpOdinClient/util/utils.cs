

namespace SharpOdinClient.util
{
    public static class utils
    {
        public enum MsgType
        {
            Message,
            Result
        }
        public static bool Stop;
        public delegate void LogDelegate(string Text,  MsgType Color ,bool IsError = false);
        public delegate void ProgressChangeDelegate(long max, long value);
        public delegate void ProgressChangedDelegate(string filename, long max, long value, long WritenSize);

        public static string Right(this string value, int length)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            return value.Length <= length ? value : value.Substring(value.Length - length);
        }
    }
}
