using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Installer
{
    partial class FinalPage : UserControl, IPage
    {
        public FinalPage()
        {
            InitializeComponent();
        }

        public bool CanGoBack { get; } = false;
        public bool CanGoNext { get; } = true;
        public string NextButtonLabel { get; } = "Finish";
        public void ShowPage()
        {
            var apiData = MainWindow.Instance.APIData;
            apiData.Contributors
                .OrderByDescending(contributor => contributor.Contributions).ToList()
                .ForEach(contributor =>
            {
                contributors.Text += $"{contributor.Name}{Environment.NewLine}";
            });
            apiData.SpecialThanks.ToList().ForEach(specialThank =>
            {
                specialThanks.Text += $"{specialThank}{Environment.NewLine}";
            });
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
