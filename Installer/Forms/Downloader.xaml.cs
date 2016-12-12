using System;
using System.IO;
using System.Net;
using System.Windows;

namespace Installer
{
    partial class Downloader : Window
    {
        public Downloader()
        {
            InitializeComponent();

            WebWorker.GetAPIData(1, () =>
            {
                DownloaderStatus.Text = "Obtaining latest release info...";
                showWorkingBar();
            },
            (data) =>
            {   
                hideProgressBars();

                var fileInfo = new FileInfo(data.FileName);
                if (fileInfo.Exists && fileInfo.Length == data.FileSize)
                    openMainWindow(data);
                else
                {
                    WebWorker.DownloadFile(data.DownloadURL, data.FileName,
                        () =>
                        {
                            DownloaderStatus.Text = $"File: {data.FileName}";
                            showProgress();
                        },
                        (progress) =>
                        {
                            Progress.Value = progress;
                            var fileSize = data.FileSize / 1024;
                            ProgressLabel.Text = $"{Math.Round(fileSize * progress * 0.01, 0)} / {fileSize} kB";
                        },
                        () =>
                        {
                            openMainWindow(data);
                        },
                        catchError("Unable to download latest release file"));
                }
            },
            catchError("Unable to obtain latest release info"));
        }

        private Action<Exception> catchError(string message)
        {
            return (Exception exception) =>
            {
                if (exception.GetType() == typeof(WebException))
                {
                    var webException = (WebException)exception;
                    if (webException.Response.GetType() == typeof(HttpWebResponse))
                    {
                        var response = (HttpWebResponse)webException.Response;
                        if (response.StatusCode == (HttpStatusCode)404)
                            message += "\nIt seems like there is no release yet";
                        else
                            message += "\nConsider re-downloading this installer";
                    }
                }
                DownloaderStatus.Text = message;
                hideProgressBars();

                File.WriteAllText("error.log", exception.Message);
            };
        }

        private void hideProgressBars()
        {
            Progress.Visibility = Visibility.Hidden;
            WorkingBar.Visibility = Visibility.Hidden;
        }

        private void openMainWindow(APIData apiData)
        {
            new MainWindow(apiData).Show();
            Close();
        }
        private void showProgress()
        {
            Progress.Visibility = Visibility.Visible;
            WorkingBar.Visibility = Visibility.Hidden;
        }

        private void showWorkingBar()
        {
            Progress.Visibility = Visibility.Hidden;
            WorkingBar.Visibility = Visibility.Visible;
        }
    }
}
