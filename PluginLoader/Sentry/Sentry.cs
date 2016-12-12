using SharpRaven;
using SharpRaven.Data;
using System;

namespace PluginLoader
{
    static class Sentry
    {
        private static RavenClient client;

        public static void Error(string message)
        {
            if (client == null)
                setupClient();

            var sentryEvent = new SentryEvent(message)
            {
                Level = ErrorLevel.Error
            };
            client?.CaptureAsync(sentryEvent);

            Log.Error(message);
        }

        public static void Exception(Exception exception)
        {
            if (client == null)
                setupClient();

            var sentryEvent = new SentryEvent(exception)
            {
                Level = ErrorLevel.Error
            };
            client?.CaptureAsync(sentryEvent);

            Log.Exception(exception);
        }

        public static void Info(string message)
        {
            if (client == null)
                setupClient();

            var sentryEvent = new SentryEvent(message)
            {
                Level = ErrorLevel.Info
            };
            client?.CaptureAsync(sentryEvent);

            Log.Info(message);
        }

        public static void Warning(string message)
        {
            if (client == null)
                setupClient();

            var sentryEvent = new SentryEvent(message)
            {
                Level = ErrorLevel.Warning
            };
            client?.CaptureAsync(sentryEvent);

            Log.Warning(message);
        }

        private static void setupClient()
        {
            // Comment method body in order to disable Sentry. All calls will be directed to Log
            client = new RavenClient(SentrySecret.URL);
            setupRaven();
        }
        private static void setupRaven()
        {
            client.Release = Versions.PLVersion.ToString();
        }
    }
}
