using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using AForge.Video.DirectShow;

namespace CameraVideoCapture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private bool isOpen = false;
        private Mat image;
        public MainWindow()
        {
            InitializeComponent();
            var f = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo i in f)
            {
                CameraFilter.Items.Add(i.Name);
            }

            if (CameraFilter.Items.Count > 0)
            {
                CameraFilter.SelectedIndex = 0;
            }
        }

        private void OpenBtn_OnClick(object sender, RoutedEventArgs e)
        {
            VideoCapture capture = new VideoCapture(CameraFilter.SelectedIndex);
            
            if (null == image)
            {
                image = new Mat();
            }

            Task.Run(() => {
                isOpen = true;
                while (isOpen)
                {
                    capture.Read(image);
                    if (image.Empty()) break;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        CameraImage.Source = bitmapToBitmapImage(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image));
                    } ));
                    
                    Cv2.WaitKey();
                }
            });
        }
        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (isOpen)
            {
                isOpen = false;
            }
        }

        private void ScreenshotsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (null != image)
            {
                Mat current = new Mat();
                image.CopyTo(current);
                
                var imName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
                current.ImWrite(imName);
                ScreenshotImage.Source = bitmapToBitmapImage(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(current));
            }
        }

        private BitmapImage bitmapToBitmapImage(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage reslut = new BitmapImage();
                reslut.BeginInit();
                reslut.CacheOption = BitmapCacheOption.OnLoad;
                reslut.StreamSource = stream;
                reslut.EndInit();
                reslut.Freeze();
                return reslut;
            }
        }

        /// <summary>Raises the <see cref="E:System.Windows.Window.Closed" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (isOpen)
            {
                isOpen = false;
            }
            image.Dispose();
            base.OnClosed(e);
        }

    }
}
