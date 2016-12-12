using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Installer
{
    partial class DetectionPage : UserControl, IPage
    {
        public DetectionPage()
        {
            InitializeComponent();
            if (Instance == null)
                Instance = this;

            pathBox.Text = DefaultPath;
            pluginsPathBox.Text = DefaultPluginsPath;

            var pathFromRegistry = tryRegistryPath();
            if (!string.IsNullOrEmpty(pathFromRegistry))
                pathBox.Text = pathFromRegistry;
        }

        public static string DefaultPath { get; } =
            System.IO.Path.Combine(
                Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%"),
                @"Steam\steamapps\common\Interstellar Rift\Uninstall");

        public static string DefaultPluginsPath { get; } =
            System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%appdata%"), @"InterstellarRift\plugins");

        public static DetectionPage Instance { get; private set; }

        public static string RegistryPath { get; } =
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 363360";
        public static string RegistryValueName { get; } = "InstallLocation";
        public static string[] RequiredFiles { get; } =
        {
            @"Build\IR.exe"
        };

        public bool CanGoBack { get; } = true;
        public bool CanGoNext { get; private set; } = false;

        public bool CreateShortcuts { get; private set; }
        public string NextButtonLabel { get; } = "Next";
        public string Path { get; private set; }
        public void ShowPage()
        {
            checkFiles();
        }

        private static string tryRegistryPath()
        {
            var registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);

            var registrySteamAppKey = registry.OpenSubKey(RegistryPath);

            if (registrySteamAppKey == null)
                return null;

            return (string)registrySteamAppKey.GetValue(RegistryValueName);
        }
        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = pathBox.Text
            };

            var handle = new WindowInteropHelper(MainWindow.Instance).Handle;
            folderBrowser.ShowDialog(new OldWindow(handle));
            pathBox.Text = folderBrowser.SelectedPath;
            folderBrowser.Dispose();

            checkFiles();
        }

        private bool checkFiles()
        {
            if (!RequiredFiles.All(file => System.IO.File.Exists(System.IO.Path.Combine(pathBox.Text, file))))
            {
                pathBox.Background = new SolidColorBrush(Color.FromRgb(255,220,220));
                CanGoNext = false;
            }
            else
            {
                pathBox.Background = new SolidColorBrush(Color.FromRgb(220, 255, 220));
                CreateShortcuts = shortcutsCheckBox.IsChecked.GetValueOrDefault(false);
                Path = pathBox.Text;
                CanGoNext = true;
            }

            MainWindow.Instance.RevalidateButtons();
            return CanGoNext;
        }
    }
}
