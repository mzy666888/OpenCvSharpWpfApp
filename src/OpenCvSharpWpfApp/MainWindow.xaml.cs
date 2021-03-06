﻿using System;
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
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Path = System.IO.Path;
using Window = System.Windows.Window;

namespace OpenCvSharpWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isOpen = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FirstButton_OnClick(object sender, RoutedEventArgs e)
        {
            //Mat src = new Mat("lenna.png", ImreadModes.Grayscale);
            //Mat dst = new Mat();
            //Cv2.Canny(src, dst, 10, 300, 3);

            
            //SrcImage.Source = bitmapToBitmapImage(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(src));
            //if (src.ImWrite("srcLenna.jpg"))
            //{

            //    Uri srcUri = new Uri(Path.Combine(Environment.CurrentDirectory, "srcLenna.jpg"));
            //    SrcImage.Source = new BitmapImage(srcUri);
            //}

            //if (dst.ImWrite("dstLenna.jpg"))
            //{
            //    Uri dstUri = new Uri(Path.Combine(Environment.CurrentDirectory, "dstLenna.jpg"));
            //    DstImage.Source = new BitmapImage(dstUri);
            //}

            //VideoCapture capture = new VideoCapture(0);
            FrameSource frame = Cv2.CreateFrameSource_Camera(0);

            //using var window = new OpenCvSharp.Window("Camera");
           
            Task.Run(() =>
            {
                isOpen = true;
                using var image = new Mat();
                using var dst = new Mat();
                
                while (isOpen)
                {

                    //capture.Read(image);
                    //if (image.Empty()) break;

                    //window.ShowImage(image);
                    //image.ImWrite("camera.jpg");
                    //Uri dstUri = new Uri(Path.Combine(Environment.CurrentDirectory, "camera.jpg"));
                    //DstImage.Source = new BitmapImage(dstUri);

                    //DstImage.Source = bitmapToBitmapImage//(OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image));

                    //Cv2.WaitKey();

                    frame.NextFrame(image);
                    if (image.Empty())
                    {
                        break;
                    }
                    
                    Dispatcher.Invoke(new Action(() =>
                    {
                        var src_gray = new Mat();
                        Cv2.CvtColor(image, src_gray, ColorConversionCodes.RGB2GRAY);
                        Cv2.Blur(src_gray, src_gray, new OpenCvSharp.Size(2,2));
                        Cv2.Canny(src_gray, dst, Threshold1.Value, Threshold2.Value);
                        SrcImage.Source = bitmapToBitmapImage(BitmapConverter.ToBitmap(src_gray));
                        DstImage.Source = bitmapToBitmapImage(BitmapConverter.ToBitmap(dst));
                    }));


                }
            });
        }

        /// <summary>Raises the <see cref="E:System.Windows.Window.Closed" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            isOpen = false;
            base.OnClosed(e);
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
    }
}
