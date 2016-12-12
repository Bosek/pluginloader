using Newtonsoft.Json;
using System;
using System.Net;

namespace Installer
{
    class WebWorker
    {
        public static string APIURL { get; } = "https://runkit.io/bosek/pluginloader-installer-api/branches/master";

        public static void DownloadFile(string url, string filename,
            Action startCallback = null,
            Action<int> progressCallback = null,
            Action completeCallback = null,
            Action<Exception> errorCallback = null)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += (sender, args) =>
                {
                    progressCallback?.Invoke(args.ProgressPercentage);
                };
                webClient.DownloadFileCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                        errorCallback?.Invoke(args.Error);
                    else
                        completeCallback?.Invoke();
                };

                webClient.DownloadFileAsync(new Uri(url), filename);
                startCallback?.Invoke();
            }
        }

        public static void GetAPIData(int version,
            Action startCallback = null,
            Action<APIData> completeCallback = null,
            Action<Exception> errorCallback = null)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadStringCompleted += (sender, args) =>
                {
                    if (args.Error != null)
                        errorCallback?.Invoke(args.Error);
                    else
                        completeCallback?.Invoke(JsonConvert.DeserializeObject<APIData>(args.Result));
                };

                webClient.DownloadStringAsync(new Uri($"{APIURL}?api={version}"));
                startCallback?.Invoke();
            }
        }
    }
}
