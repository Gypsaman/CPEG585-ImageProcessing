using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ImageProcessing2019
{
    class ImageProcessing
    {
        public static Bitmap GrayScale(Bitmap orig)
        {
            Bitmap mod = (Bitmap) orig.Clone();
            Color color;
            for (int x = 0; x < mod.Width; x++)
            {
                for (int y = 0; y < mod.Height; y++)
                {
                    color = mod.GetPixel(x, y);
                    byte gray = (byte) (0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
                    mod.SetPixel(x,y,Color.FromArgb(gray,gray,gray));
                }
            }
            return mod;
        }
        public static Bitmap Contrast(Bitmap orig)
        {
            int[] reds = new int[256];
            int[] greens = new int[256];
            int[] blues = new int[256];
            Bitmap mod = (Bitmap)orig.Clone();
            Color color;
            int counter = 0;
            for (int x = 0; x < mod.Width; x++)
            {
                for (int y = 0; y < mod.Height; y++)
                {
                    color = mod.GetPixel(x, y);
                    reds[color.R] += 1;
                    greens[color.G] += 1;
                    blues[color.B] += 1;
                    counter++;
                }
            }
            reds = HistContrast(reds);
            greens = HistContrast(greens);
            blues = HistContrast(blues);
            for (int x = 0; x < mod.Width; x++)
            {
                for (int y = 0; y < mod.Height; y++)
                {
                    color = mod.GetPixel(x, y);
                    mod.SetPixel(x, y, Color.FromArgb(reds[color.R],greens[color.G],blues[color.B]));
                }
            }
            return mod;
        }
        private static int[] HistContrast(int[] hist)
        {
            int[] newvals = new int[256];
            int totalBytes = 0;
            
            for (int x = 0; x < hist.Length; x++) totalBytes += hist[x];
            int cdfmin = 0;
            int i = 0;
            while (hist[i] == 0) i++;
            cdfmin = hist[i];
            int cum = 0;
            
            for (int x = 0; x < hist.Length; x++)
            {
                if(hist[x]==0) continue;
                cum += hist[x];
                double calc = (cum - cdfmin);
                calc /= (totalBytes - cdfmin);
                newvals[x] = (int) (calc*255);
            }
            return newvals;
        }
        public static Bitmap Rotate(Bitmap orig)
        {
            Bitmap mod = (Bitmap) orig.Clone();

            return mod;
        }

        public static Bitmap Brighten(Bitmap orig,int value)
        {
            Bitmap mod = (Bitmap) orig.Clone();
            Color color;
            for (int x = 0; x < mod.Width; x++)
            {
                for (int y = 0; y < mod.Height; y++)
                {
                    color = mod.GetPixel(x, y);
                    int green = Math.Min(Math.Max(color.G + value, 0), 255);
                    int  blue = Math.Min(Math.Max(color.B + value, 0), 255);
                    int  red = Math.Min(Math.Max(color.R + value,0),255);
                    
                    mod.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return mod;
        }

        public static Bitmap ApplyGaussian(Bitmap orig, int[] Gauss,bool LowPass)
        {
            Bitmap mod = (Bitmap) orig.Clone();
            return mod;
        }

        public static bool Convolve(Bitmap b, double[][] kernel)
        {  // assumes kernel is symmetric or already rotated by 180 degrees
            //  the return format is BGR, NOT RGB.
            Bitmap bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                ImageLockMode.ReadWrite,
                                PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
                               ImageLockMode.ReadWrite,
                               PixelFormat.Format24bppRgb);

            int stride = bmData.Stride; // number of bytes in a row 3*b.Width + even bits
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pc = (byte*)(void*)SrcScan0;

                byte red, green, blue;
                int nOffset = stride - b.Width * 3;
                for (int y = 0; y < b.Height - 2; ++y)
                {
                    for (int x = 0; x < b.Width - 2; ++x)
                    {
                        double[][] bluem = new double[3][];
                        for (int i = 0; i < 3; i++)
                            bluem[i] = new double[3];
                        double[][] greenm = new double[3][];
                        for (int i = 0; i < 3; i++)
                            greenm[i] = new double[3];
                        double[][] redm = new double[3][];
                        for (int i = 0; i < 3; i++)
                            redm[i] = new double[3];
                        bluem[0][0] = pc[0];
                        greenm[0][0] = pc[1];
                        redm[0][0] = pc[2];

                        bluem[0][1] = pc[3];
                        greenm[0][1] = pc[4];
                        redm[0][1] = pc[5];

                        bluem[0][2] = pc[6];
                        greenm[0][2] = pc[7];
                        redm[0][2] = pc[8];

                        bluem[1][0] = pc[0 + stride];
                        greenm[1][0] = pc[1 + stride];
                        redm[1][0] = pc[2 + stride];

                        bluem[1][1] = pc[3 + stride];
                        greenm[1][1] = pc[4 + stride];
                        redm[1][1] = pc[5 + stride];

                        bluem[1][2] = pc[6 + stride];
                        greenm[1][2] = pc[7 + stride];
                        redm[1][2] = pc[8 + stride];

                        bluem[2][0] = pc[0 + stride * 2];
                        greenm[2][0] = pc[1 + stride * 2];
                        redm[2][0] = pc[2 + stride * 2];

                        bluem[2][1] = pc[3 + stride * 2];
                        greenm[2][1] = pc[4 + stride * 2];
                        redm[2][1] = pc[5 + stride * 2];

                        bluem[2][2] = pc[6 + stride * 2];
                        greenm[2][2] = pc[7 + stride * 2];
                        redm[2][2] = pc[8 + stride * 2];


                        double cblue = bluem[0][0] * kernel[0][0] +
                            bluem[0][1] * kernel[0][1] +
                            bluem[0][2] * kernel[0][2] +
                            bluem[1][0] * kernel[1][0] +
                            bluem[1][1] * kernel[1][1] +
                            bluem[1][2] * kernel[1][2] +
                            bluem[2][0] * kernel[2][0] +
                            bluem[2][1] * kernel[2][1] +
                            bluem[2][2] * kernel[2][2];
                        double cgreen = greenm[0][0] * kernel[0][0] +
                           greenm[0][1] * kernel[0][1] +
                           greenm[0][2] * kernel[0][2] +
                           greenm[1][0] * kernel[1][0] +
                           greenm[1][1] * kernel[1][1] +
                           greenm[1][2] * kernel[1][2] +
                           greenm[2][0] * kernel[2][0] +
                           greenm[2][1] * kernel[2][1] +
                           greenm[2][2] * kernel[2][2];

                        double cred = redm[0][0] * kernel[0][0] +
                           redm[0][1] * kernel[0][1] +
                           redm[0][2] * kernel[0][2] +
                           redm[1][0] * kernel[1][0] +
                           redm[1][1] * kernel[1][1] +
                           redm[1][2] * kernel[1][2] +
                           redm[2][0] * kernel[2][0] +
                           redm[2][1] * kernel[2][1] +
                           redm[2][2] * kernel[2][2];

                        if (cblue < 0) cblue = 0;
                        if (cblue > 255) cblue = 255;
                        if (cgreen < 0) cgreen = 0;
                        if (cgreen > 255) cgreen = 255;
                        if (cred < 0) cred = 0;
                        if (cred > 255) cred = 255;

                        p[3 + stride] = (byte)cblue;
                        p[4 + stride] = (byte)cgreen;
                        p[5 + stride] = (byte)cred;

                        p += 3;
                        pc += 3;
                    }
                    p += nOffset;
                    pc += nOffset;
                }

            }
            
            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return true;
        }
        public static bool Convolver(Bitmap b, double[,] kernel)
        {  
            if(kernel.GetLength(0) != kernel.GetLength(1))
                throw new System.ArgumentException("Kernel must be symmetrical", "original");
            Bitmap bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                ImageLockMode.ReadWrite,
                                PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
                               ImageLockMode.ReadWrite,
                               PixelFormat.Format24bppRgb);

            int stride = bmData.Stride; // number of bytes in a row 3*b.Width + even bits
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;
            int colsize = kernel.GetLength(0);
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pc = (byte*)(void*)SrcScan0;

                int nOffset = stride - b.Width * colsize;

                for (int y = 0; y < b.Height - 2; ++y)
                {
                    for (int x = 0; x < b.Width - 2; ++x)
                    {
                        double cblue=0,cgreen=0,cred = 0;

                        for (int row = 0; row < colsize; row++)
                        {
                            for (int col = 0; col < colsize; col++)
                            {
                                cblue += pc[0 + col * colsize + stride * row] * kernel[row, col];
                                cgreen += pc[1 + col * colsize + stride * row] * kernel[row, col];
                                cred += pc[2 + col * colsize + stride * row] * kernel[row, col];
                            }
                        }

                        p[3 + stride] = (byte)Math.Min(Math.Max(cblue, 0), 255);
                        p[4 + stride] = (byte)Math.Min(Math.Max(cgreen, 0), 255);
                        p[5 + stride] = (byte)Math.Min(Math.Max(cred, 0), 255);

                        p += colsize;
                        pc += colsize;
                    }
                    p += nOffset;
                    pc += nOffset;
                }
            }
            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return true;
        }

        public static Bitmap SubtractBitMaps(Bitmap b1, Bitmap b2)
        {
            if(b1.PixelFormat != b2.PixelFormat) throw new System.ArgumentException("Bitmaps must have same PixelFormat", "original");
            return BitMapAddition(b1, b2, -1);
        }
        public static Bitmap AddBitMaps(Bitmap b1, Bitmap b2)
        {
            if (b1.PixelFormat != b2.PixelFormat) throw new System.ArgumentException("Bitmaps must have same PixelFormat", "original");
            return BitMapAddition(b1, b2, 1);
        }
        private static Bitmap BitMapAddition(Bitmap b1, Bitmap b2, int op = 1)
        {
            Bitmap result = new Bitmap(b1.Width,b1.Height);
            if(op != 1 && op != -1) throw new System.ArgumentException("Operand must be -1 or 1", "original");
            
            for (int x = 0; x < b1.Width; x++)
            {
                for (int y=0; y <b1.Height;y++)
                {
                    Color b1color = b1.GetPixel(x, y);
                    Color b2color = b2.GetPixel(x, y);

                    int blue = Math.Abs(b1color.B + op*b2color.B);
                    int green = Math.Abs(b1color.G + op*b2color.G);
                    int red = Math.Abs(b1color.R + op*b2color.R);

                    result.SetPixel(x,y, Color.FromArgb(red, green, blue));
                }
            }
            return result;
        }
        public static Bitmap BitMapMultiply(Bitmap b1, Bitmap b2)
        {
            Bitmap result = new Bitmap(b1.Width, b1.Height);
            

            for (int x = 0; x < b1.Width; x++)
            {
                for (int y = 0; y < b1.Height; y++)
                {
                    Color b1color = b1.GetPixel(x, y);
                    Color b2color = b2.GetPixel(x, y);

                    int blue = (b1color.B  * b2color.B);
                    int green = (b1color.G * b2color.G);
                    int red = (b1color.R  * b2color.R);

                    result.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return result;
        }
        public static Bitmap Gradient(Bitmap origImageBMP)
        {
            Bitmap Dx = (Bitmap)origImageBMP.Clone();
            Bitmap Dy = (Bitmap)origImageBMP.Clone();

            Bitmap result = new Bitmap(Dx.Width, Dx.Height);

            Kernels kernel = new Kernels();
            Kernels.kerneldata kDx = (Kernels.kerneldata)kernel.KernelsAvail["First Derivative x"];
            Kernels.kerneldata kDy = (Kernels.kerneldata)kernel.KernelsAvail["First Derivative y"];
            ImageProcessing.Convolver(Dx, kDx.weights);
            ImageProcessing.Convolver(Dy, kDy.weights);


            for (int x = 0; x < Dx.Width; x++)
            {
                for (int y = 0; y < Dx.Height; y++)
                {
                    Color b1color = Dx.GetPixel(x, y);
                    Color b2color = Dy.GetPixel(x, y);

                    int blue = (int) Math.Ceiling(Math.Sqrt(Math.Pow(b1color.B,2) + Math.Pow(b2color.B,2)));
                    int green = (int)Math.Ceiling(Math.Sqrt(Math.Pow(b1color.G, 2) + Math.Pow(b2color.G, 2)));
                    int red = (int)Math.Ceiling(Math.Sqrt(Math.Pow(b1color.R, 2) + Math.Pow(b2color.R, 2))); ;

                    blue = (int) Math.Min(Math.Max(blue, 0), 255);
                    green = (int)Math.Min(Math.Max(green, 0), 255);
                    red= (int)Math.Min(Math.Max(red, 0), 255);

                    result.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return result;
        }
        public static Bitmap BitMapSquareRoot(Bitmap b1)
        {
            Bitmap result = new Bitmap(b1.Width, b1.Height);


            for (int x = 0; x < b1.Width; x++)
            {
                for (int y = 0; y < b1.Height; y++)
                {
                    Color b1color = b1.GetPixel(x, y);


                    int blue = (int)Math.Sqrt(b1color.B);
                    int green = (int)Math.Sqrt(b1color.G);
                    int red = (int)Math.Sqrt(b1color.R);

                    result.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            return result;
        }

        public static Bitmap GaussianGap(Bitmap Orig, double Sigma1, double Sigma2)
        {
            Bitmap result = (Bitmap) Orig.Clone();
            Kernels.kerneldata sig1 = new Kernels.kerneldata(Kernels.GenerateGaussian, 3, Sigma1);
            Kernels.kerneldata sig2 = new Kernels.kerneldata(Kernels.GenerateGaussian, 3, Sigma2);
            int size = sig1.weights.GetLength(0);
            double[,] ggFilter = new double[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                    ggFilter[row, col] = (sig1.weights[row, col] - sig2.weights[row, col]) * size * size;
            }

            double sum = 0;
            foreach (var num in ggFilter) sum += num;
            Convolver(result, ggFilter);
            return result;
        }

    }
}
