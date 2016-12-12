using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Installer
{
    partial class LicensePage : UserControl, IPage
    {
        private string licenseFile = "LICENSE";
        public LicensePage()
        {
            InitializeComponent();

            if (!MainWindow.Instance.Package.Entries.Any(entry => entry.Name == licenseFile))
                licenseNotFound();
            else
            {
                using (var licenceStream = MainWindow.Instance.Package.GetEntry(licenseFile).Open())
                {
                    richTextBox.Selection.Load(licenceStream, DataFormats.Text);
                }
            }
        }

        public bool CanGoBack { get; private set; } = false;
        public bool CanGoNext { get; private set; } = true;
        public string NextButtonLabel { get; } = "Next";
        public void ShowPage() { }

        private void licenseNotFound()
        {
            richTextBox.Selection.Text = $"{licenseFile} file could not be found.";
            CanGoNext = false;
        }
    }
}
