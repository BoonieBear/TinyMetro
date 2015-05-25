using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using TinyMetroWpfLibrary.LogUtil;
using TinyMetroWpfLibrary.Utility;

namespace TinyMetroWpfLibrary.Controls
{
    public class AsyncThumbnail : Control
    {
        private static ILogService _logger = LogService.GetLogger(typeof(AsyncThumbnail));
        public BitmapSource BindingSource
        {
            get { return (BitmapSource)GetValue(BindingSourceProperty); }
            set { SetValue(BindingSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindingSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingSourceProperty =
            DependencyProperty.Register("BindingSource", typeof(BitmapSource), typeof(AsyncThumbnail), new PropertyMetadata(null));

        
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(AsyncThumbnail), new PropertyMetadata(null, SourceChanged));

        private static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AsyncThumbnail thumbnail = d as AsyncThumbnail;
            if (thumbnail!= null)
            {
                thumbnail.OnSourceChanged(e.NewValue as string);
            }
        }

        private static Dictionary<string, BitmapSource> _thumbnailCache = new Dictionary<string, BitmapSource>();
        public void OnSourceChanged(string imagePath)
        {
            if (_thumbnailCache.ContainsKey(imagePath))
            {
                BindingSource = _thumbnailCache[imagePath];
            }
            else
            {
                ThreadPool.QueueUserWorkItem((obj) =>
                {
                    OnThreadCallBack(imagePath, GetThumbnailImage(imagePath));
                });
            }
        }

        private BitmapSource GetThumbnailImage(string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                System.Diagnostics.Debug.Assert(false);
                _logger.Warn(string.Format("GetThumbnailImage: imagePath {0} dose not exist", imagePath == null ? "null" : imagePath));
                return null;
            }

            if (!IOHelper.CheckImageValidation(imagePath))
            {
                _logger.Warn(string.Format("GetThumbnailImage: call CheckImageValidation return false, on image:{0}", imagePath));
                return null;
            }

            BitmapImage bmpImage = new BitmapImage();
            try
            {
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    bmpImage.BeginInit();
                    bmpImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                    bmpImage.DecodePixelWidth = 250;
                    bmpImage.DecodePixelHeight = 120;
                    bmpImage.StreamSource = fs;
                    bmpImage.EndInit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("GetThumbnailImage {0} Fail", imagePath), ex);
            }
            finally
            {
                bmpImage.Freeze();
            }
            return bmpImage;
        }

        private void OnThreadCallBack(string imagePath, BitmapSource imageSource)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(DispatcherPriority.Send, (Action)delegate
                {
                    OnThreadCallBack(imagePath, imageSource);
                });
                return;
            }

            BindingSource = imageSource;
            if (!_thumbnailCache.ContainsKey(imagePath) && imageSource != null)
            {
                _thumbnailCache.Add(imagePath, imageSource);
            }
        }
    
    }
}
