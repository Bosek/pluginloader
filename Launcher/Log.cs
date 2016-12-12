using System;

namespace Launcher
{
    class Log : MarshalByRefObject
    {
        public static event Action<string> OnError;
        public static event Action<string> OnInfo;
        public static event Action<string> OnSuccess;

        public void Error(string text = "")
        {
            OnError?.Invoke(text);
        }

        public void ErrorLine(string text = "")
        {
            Error(text + Environment.NewLine);
        }

        public void Info(string text = "")
        {
            OnInfo?.Invoke(text);
        }
        public void InfoLine(string text = "")
        {
            Info(text + Environment.NewLine);
        }
        public void Success(string text = "")
        {
            OnSuccess?.Invoke(text);
        }
        public void SuccessLine(string text = "")
        {
            Success(text + Environment.NewLine);
        }
    }
}
