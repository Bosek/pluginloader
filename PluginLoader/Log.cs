using System;

namespace PluginLoader
{
    static class Log
    {
        public static event Action<string> OnError;
        public static event Action<Exception> OnException;
        public static event Action<string> OnInfo;
        public static event Action<string> OnWarning;
        public static void Error(string message)
        {
            OnError?.Invoke(message);
        }
        public static void Exception(Exception exception)
        {
            OnException?.Invoke(exception);
        }
        public static void Info(string message)
        {
            OnInfo?.Invoke(message);
        }

        public static void Warning(string message)
        {
            OnWarning?.Invoke(message);
        }
    }
}
