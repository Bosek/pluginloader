using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Installer
{
    partial class InstallPage : UserControl, IPage
    {
        private readonly static ColorAnimation greenAnimation =
            new ColorAnimation(Color.FromRgb(220, 255, 220), new Duration(TimeSpan.FromMilliseconds(300)));
        private readonly static ColorAnimation redAnimation =
            new ColorAnimation(Color.FromRgb(255, 220, 220), new Duration(TimeSpan.FromMilliseconds(300)));
        public InstallPage()
        {
            InitializeComponent();
            if (Instance == null)
                Instance = this;
        }

        public static InstallPage Instance { get; private set; }
        public bool CanGoBack { get; private set; } = false;
        public bool CanGoNext { get; private set; } = false;
        public string NextButtonLabel { get; } = "Next";
        public void ShowPage()
        {
            createNewItem($"Game path: {DetectionPage.Instance.Path}");

            extractPackage();
            createPluginsDir();
            createShortcuts();

            createNewItem("Installation complete.");

            CanGoNext = true;
            MainWindow.Instance.RevalidateButtons();
        }

        private static void catchException(Exception exception)
        {
            File.WriteAllText("error.log", exception.Message);
        }

        private static void failAnimation(ListBoxItem item)
        {
            item.Background.BeginAnimation(SolidColorBrush.ColorProperty, redAnimation);
        }

        private static void successAnimation(ListBoxItem item)
        {
            item.Background.BeginAnimation(SolidColorBrush.ColorProperty, greenAnimation);
        }
        private ListBoxItem createNewItem(string text = "")
        {
            var item = new ListBoxItem
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Content = text
            };
            listBox.Items.Add(item);
            return item;
        }
        private void createPluginsDir()
        {
            if (!Directory.Exists(DetectionPage.DefaultPluginsPath))
            {
                var directoryItem = createNewItem("Creating plugins directory");
                try
                {
                    Directory.CreateDirectory(DetectionPage.DefaultPluginsPath);
                    successAnimation(directoryItem);
                }
                catch (Exception exception)
                {
                    catchException(exception);
                    failAnimation(directoryItem);
                }
            }
        }

        private void createShortcuts()
        {
            if (DetectionPage.Instance.CreateShortcuts)
            {
                var shortcutsItem = createNewItem("Creating desktop shortcuts");
                try
                {
                    Helpers.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "Interstellar Rift with PluginLoader",
                        Path.Combine(DetectionPage.Instance.Path, "Build/", "PLLaucher.exe"),
                        "Interstellar Rift with PluginLoader");

                    Helpers.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "Interstellar Rift Plugins",
                        Path.Combine(DetectionPage.DefaultPluginsPath),
                        "Interstellar Rift Plugins Directory");

                    successAnimation(shortcutsItem);
                }
                catch (Exception exception)
                {
                    catchException(exception);
                    failAnimation(shortcutsItem);
                }
            }
        }

        private void extractPackage()
        {
            var packageItem = createNewItem($"Extracting {MainWindow.Instance.APIData.FileName}");
            try
            {
                MainWindow.Instance.Package.ExtractArchive(
                    Path.Combine(DetectionPage.Instance.Path, "Build/"), true);
                successAnimation(packageItem);
            }
            catch (Exception exception)
            {
                catchException(exception);
                failAnimation(packageItem);
            }
        }
    }
}
