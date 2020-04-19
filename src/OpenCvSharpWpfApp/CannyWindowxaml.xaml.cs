using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenCvSharp;
using Window = System.Windows.Window;

namespace OpenCvSharpWpfApp
{
    /// <summary>
    /// CannyWindowxaml.xaml 的交互逻辑
    /// </summary>
    public partial class CannyWindowxaml : Window
    {
        public CannyWindowxaml()
        {
            InitializeComponent();
        }

        private Mat FindContours(Mat srcImage)
        {
            Mat src_gray = new Mat();
            Cv2.CvtColor(srcImage, src_gray, ColorConversionCodes.RGB2GRAY);
            Cv2.Blur(src_gray, src_gray, new OpenCvSharp.Size(2, 2));

            Mat cannyImage = new Mat();
            Cv2.Canny(src_gray, cannyImage, 100, 200);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchly;
            Cv2.FindContours(cannyImage, out contours, out hierarchly, RetrievalModes.Tree,
                ContourApproximationModes.ApproxSimple, new OpenCvSharp.Point(0, 0));

            Mat dstImage = Mat.Zeros(cannyImage.Size(), srcImage.Type());

            Random rnd = new Random();

            for (int i = 0; i < contours.Length; i++)
            {
                Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                Cv2.DrawContours(dstImage, contours, i, color, 2, LineTypes.Link8, hierarchly);
            }

            return dstImage;
        }

        private void CannyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            
            Mat srcImage = Cv2.ImRead(System.IO.Path.Combine(Environment.CurrentDirectory, "Images\\OpenCvSharp.png"));

            Mat dstImage = FindContours(srcImage);
            Cv2.ImShow("原始图", srcImage);
            Cv2.ImShow("轮廓图", dstImage);
        }
    }
}
