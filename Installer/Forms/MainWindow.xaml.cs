using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Installer
{
    partial class MainWindow : Window
    {
        private IPage actualPage;
        private Stream packageStream;
        public MainWindow(APIData apiData)
        {
            InitializeComponent();
            if (Instance == null)
                Instance = this;
            APIData = apiData;

            packageStream = File.OpenRead(APIData.FileName);
            Package = new ZipArchive(packageStream);

            Pages.Add(new LicensePage());
            Pages.Add(new DetectionPage());
            Pages.Add(new InstallPage());
            Pages.Add(new FinalPage());

            actualPage = Pages.First();
            loadPage((Control)actualPage);
        }

        public static MainWindow Instance { get; private set; }

        public APIData APIData { get; private set; }
        public ZipArchive Package { get; private set; }
        public List<IPage> Pages { get; } = new List<IPage>();
        public void RevalidateButtons()
        {
            backButton.Visibility = hasPreviousPage(actualPage) ? Visibility.Visible : Visibility.Hidden;
            backButton.IsEnabled = actualPage.CanGoBack;

            nextButton.IsEnabled = actualPage.CanGoNext;
            nextButton.Content = actualPage.NextButtonLabel;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            loadPreviousPage();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private int getPageIndex(IPage page)
        {
            return Pages.FindIndex(p => p == page);
        }

        private bool hasNextPage(IPage page)
        {
            var index = getPageIndex(page);
            if (index < Pages.Count - 1)
                return true;
            return false;
        }

        private bool hasPreviousPage(IPage page)
        {
            var index = getPageIndex(page);
            if (0 < index)
                return true;
            return false;
        }

        private void loadNextPage()
        {
            if (actualPage.CanGoNext)
            {
                if (hasNextPage(actualPage))
                    loadPage((Control)Pages[getPageIndex(actualPage) + 1]);
                else
                    Close();
            }
        }
        private void loadPage(Control page)
        {
            Viewer.Content = page;
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;
            actualPage = (IPage)page;

            RevalidateButtons();
            ((IPage)page).ShowPage();
        }

        private void loadPreviousPage()
        {
            if (hasPreviousPage(actualPage) && actualPage.CanGoBack)
                loadPage((Control)Pages[getPageIndex(actualPage) - 1]);
        }
        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            loadNextPage();
        }
    }
}
