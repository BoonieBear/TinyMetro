using System;
using System.Windows.Media;

namespace TinyMetroWpfLibrary.Controls.MediaManager
{
    public class MediaManager
    {
        private MediaManager()
        {
 
        }

        private static MediaManager _instance;
        public static MediaManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MediaManager();
                }
                return _instance;
            }
        }

        private MediaPlayer _backgroundMediaMgr = null;
        public MediaPlayer BackgroundMediaMgr
        {
            get
            {
                if (_backgroundMediaMgr == null)
                {
                    _backgroundMediaMgr = new MediaPlayer();
                    _backgroundMediaMgr.Open(new Uri(@"Audios\Windows Background.wav", UriKind.Relative));
                }
                return _backgroundMediaMgr;
            }
        }

        public void PlayBackgroundMedia()
        {
            BackgroundMediaMgr.Stop();
            BackgroundMediaMgr.Play();
        }
    }
}
