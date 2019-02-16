using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageProcessing2019
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    
    public partial class MainWindow : Window
    {
        private Bitmap origImageBMP;
        private Bitmap modImageBMP;
        private bool ShowDiffImage = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileOpen_ClickedOn(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "JPG Files (*.jpg) |*.jpg| JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                origImageBMP = new Bitmap(filename);
                if (origImageBMP.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    MessageBox.Show($"File Format not Supported: {origImageBMP.PixelFormat}");
                    return;
                }

                PaintImage(OrigImage, origImageBMP);
                modImageBMP = (Bitmap) origImageBMP.Clone();
                PaintImage(ModImage,modImageBMP);
            }
        }
        private void PaintImage(System.Windows.Controls.Image imageToPaint, Bitmap bmp)
        {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            stream.Position = 0;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, Convert.ToInt32(stream.Length));
            BitmapImage bmapImage = new BitmapImage();
            bmapImage.BeginInit();
            bmapImage.StreamSource = stream;
            bmapImage.EndInit();
            imageToPaint.Source = bmapImage;
            imageToPaint.Stretch = Stretch.Uniform;
        }

        private void GrayScale_OnClick(object sender, RoutedEventArgs e)
        {
            modImageBMP = ImageProcessing.GrayScale(origImageBMP);
            PaintImage(ModImage,modImageBMP);
            if(ShowDiffImage)
                PaintImage(CompImage, ImageProcessing.SubtractBitMaps(origImageBMP, modImageBMP));
        }
        private void Contrast_OnClick(object sender, RoutedEventArgs e)
        {
            modImageBMP = ImageProcessing.Contrast(origImageBMP);
            PaintImage(ModImage, modImageBMP);
            if(ShowDiffImage)
                PaintImage(CompImage, ImageProcessing.SubtractBitMaps(origImageBMP, modImageBMP));
        }
        private void Convolve_OnClick(object sender, RoutedEventArgs e)
        {
            KernelSelection ks = new KernelSelection();
            if (ks.ShowDialog() == true)
            {
                Bitmap conv = (Bitmap)origImageBMP.Clone();
               
                ImageProcessing.Convolver(conv, ks.kernelselect.weights); 

                PaintImage(ModImage, conv);
                if(ShowDiffImage)
                    PaintImage(CompImage, ImageProcessing.SubtractBitMaps(origImageBMP, modImageBMP));
            }

        }
        private void Gradient_OnClick(object sender, RoutedEventArgs e)
        {
            Bitmap result = ImageProcessing.Gradient(origImageBMP);
            PaintImage(ModImage, result);
            if(ShowDiffImage)
                PaintImage(CompImage, ImageProcessing.SubtractBitMaps(origImageBMP, result));
        }
        private void GaussianGap_OnClick(object sender, RoutedEventArgs e)
        {
            GaussianGapParam gg = new GaussianGapParam();
            if (gg.ShowDialog() == true)
            {
                Bitmap result = ImageProcessing.GaussianGap(origImageBMP,gg.Sigma1,gg.Sigma2);
                PaintImage(ModImage, result);
                if(ShowDiffImage)
                    PaintImage(CompImage, ImageProcessing.SubtractBitMaps(origImageBMP, result));
            }
        }
        private void Brighten_OnClick(object sender, RoutedEventArgs e)
        {
           
            BrigthenValue bright = new BrigthenValue();
            if (bright.ShowDialog() == true)
            {
                int value = bright.BrightenValue;
                modImageBMP = ImageProcessing.Brighten(origImageBMP, value);
                PaintImage(ModImage, modImageBMP);
                if(ShowDiffImage)
                    PaintImage(CompImage, ImageProcessing.SubtractBitMaps(origImageBMP, modImageBMP));
            }
        }
    }
}
