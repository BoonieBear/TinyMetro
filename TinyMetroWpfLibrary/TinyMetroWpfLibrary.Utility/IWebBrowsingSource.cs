namespace TinyMetroWpfLibrary.Utility
{
    public delegate void LoadCompleteHandler();
    public interface IWebBrowsingSource
    {
        event LoadCompleteHandler LoadComplete;
        void RegisterLoadCompleteHandler(LoadCompleteHandler handler);
        void Navigate(string url);
        void Stop();
        void Reinitialize();

        void TestCompleted();
    }
}
