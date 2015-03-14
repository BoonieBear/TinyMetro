namespace TinyMetroWpfLibrary.Utility
{
    public class WebBrowsingSourceManager
    {
        private static IWebBrowsingSource _source;
        public static void RegisterSource(IWebBrowsingSource source)
        {
            _source = source;
        }

        public static IWebBrowsingSource Source
        {
            get
            {
                return _source;
            }
        }
    }
}
