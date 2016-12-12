namespace Installer
{
    public interface IPage
    {
        bool CanGoBack { get; }
        bool CanGoNext { get; }

        string NextButtonLabel { get; }

        void ShowPage();
    }
}
