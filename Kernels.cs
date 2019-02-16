using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.VideoStab;

namespace ImageProcessing2019
{
    public class Kernels
    {
        
        public class kerneldata
        {
            private int _size;
            private double _param;

            public double[,] weights;
            public bool IsLowPass = true;
            public delegate double[,] CreateDelegate(int size = 3, double param = 0.0);

            public kerneldata(CreateDelegate delfunc,int size=3, double param=0.0)
            {
                CreateDelegate del = delfunc;
                weights = delfunc(size, param);
                _size = size;
                _param = param;
            }

            public kerneldata ConvertToHP()
            {
                kerneldata id = new kerneldata(GenerateIdentity,_size,_param){IsLowPass = false};
                kerneldata hp = new kerneldata(GenerateIdentity,_size,_param){IsLowPass = false};
                for(int row=0;row < _size;row++)
                for (int col = 0; col < _size; col++)
                    hp.weights[row, col] = (id.weights[row, col] - weights[row, col])*_size*_size;
                return hp;
            }
        }
        
        public kerneldata identity = new kerneldata(GenerateIdentity){IsLowPass = false};
        public kerneldata Gaussian = new kerneldata(GenerateGaussian,3,1){IsLowPass = true};
        public kerneldata Average = new kerneldata(GenerateAverage, 3, 1){IsLowPass = true};
        public kerneldata Sharpen = new kerneldata(GenerateSharpen, 3,0.5){IsLowPass = true};
        public kerneldata FirstDx = new kerneldata(GenerateDx, 3, 0){IsLowPass = false};
        public kerneldata FirstDy = new kerneldata(GenerateDy, 3, 0) { IsLowPass = false };
        public kerneldata LaPlace = new kerneldata(GenerateLaPlace, 3, 0) { IsLowPass = false };
        public kerneldata BiNomial = new kerneldata(GenerateBinomial, 3, 0) { IsLowPass = true };

        public Hashtable KernelsAvail = new Hashtable();

        public Kernels()
        {
            KernelsAvail["identity"] = identity;
            KernelsAvail["Gaussian"] = Gaussian;
            KernelsAvail["Average"] = Average;
            KernelsAvail["Sharpen"] = Sharpen;
            KernelsAvail["First Derivative x"] = FirstDx;
            KernelsAvail["First Derivative y"] = FirstDy;
            KernelsAvail["LaPlace"] = LaPlace;
            KernelsAvail["Bi Nomial"] = BiNomial;
            ;
        }
        public static double[,] GenerateAverage(int size = 3, double param = 0)
        {
            double[,] result = new double[size, size];
            int center = (int)Math.Floor((double)size / 2);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                    result[row, col] = (1.0 / (size*size));
            }

            return result;
        }
        public static double[,] GenerateIdentity(int size=3, double param=0)
        {
            double[,] result = new double[size,size];
            int center = (int)Math.Floor((double)size / 2);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                    result[row, col] = (col == center && row == center) ? 1 : 0;
            }

            return result;
        }
        public static double[,] GenerateGaussian(int size, double sigma = 1)
        {
            if (size % 2 != 1) throw new System.ArgumentException("Kernels are odd size (3,5,7..)", "original");

            double[,] result = new double[size, size];
            double sigmasq = sigma * sigma;
            int range = (int)Math.Floor((double)size / 2);
            
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    int x = -range + col;
                    int y = range - row;
                    result[row, col] = 1 / (2 * Math.PI * sigmasq) * Math.Exp(-(Math.Pow(x,2) + Math.Pow(y,2)) / (2 * sigmasq));
                }
            }

            double sum = 0;
            foreach (double num in result) sum += num;
            for (int row = 0; row < size; row++)
            for (int col = 0; col < size; col++)
                result[row, col] /= sum;
            

            return result;
        }
        public static double[,] GenerateSharpen(int size, double f = 0.5)
        {
            double[,] result = new double[size, size];
            int center = (int)Math.Floor((double)size / 2);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                    result[row, col] = (row==center && col==center) ? (size*size-f) / (size*size) : -f / (size * size);
            }

            return result;
        }

        public static double[,] GenerateDx(int size = 3, double param = 0)
        {
            double[,] result = new double[,]
            {   {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };
            int center = (int)Math.Floor((double)size / 2);
            return result;
        }
        public static double[,] GenerateDy(int size = 3, double param = 0)
        {
            double[,] result = new double[,]
            {   {1.0,2.0,1.0},
                {0.0,0.0,0.0},
                {-1.0,-2.0,-1.0}
            };
            int center = (int)Math.Floor((double)size / 2);
            return result;
        }
        public static double[,] GenerateLaPlace(int size = 3, double param = 0)
        {
            double[,] result = new double[,]
            {
                {0.0, -1.0, 0.0},
                {-1.0, 4.0, -1.0},
                {0.0, -1.0, 0.0}
            };
            int center = (int)Math.Floor((double)size / 2);
            return result;
        }
        public static double[,] GenerateBinomial(int size = 3, double param = 0)
        {
            double[,] result = new double[,]
            {
                {-1.0, -2.0, -1.0},
                {-2.0, 12.0, -2.0},
                {-1.0, -2.0, -1.0}
            };
            int center = (int)Math.Floor((double)size / 2);
            return result;
        }
    }

    //public class KernelsOld
    //{

    //    private double[,] AverageLPF = new double[3, 3]
    //        {
    //        {1.0 / 9, 1.0 / 9, 1.0 / 9},
    //        {1.0 / 9, 1.0 / 9, 1.0 / 9},
    //        {1.0 / 9, 1.0 / 9, 1.0 / 9}
    //        };

    //    double[,] AverageLPF5x5 = new double[5, 5]
    //    {   {1.0/25,1.0/25,1.0/25,1.0/25,1.0/25},
    //        {1.0/25,1.0/25,1.0/25,1.0/25,1.0/25},
    //        {1.0/25,1.0/25,1.0/25,1.0/25,1.0/25},
    //        {1.0/25,1.0/25,1.0/25,1.0/25,1.0/25},
    //        {1.0/25,1.0/25,1.0/25,1.0/25,1.0/25}
    //    };
    //    double[,] Sharpenf50 = new double[3, 3]
    //    {   {-0.5/9,-0.5/9,-0.5/9},
    //        {-0.5/9,(9-0.5)/9,-0.5/9},
    //        {-0.5/9,-0.5/9,-0.5/9}
    //    };
    //    double[,] AverageHPF = new double[3, 3]
    //    {   {-1.0,-1.0,-1.0},
    //        {-1.0,8.0,-1.0},
    //        {-1.0,-1.0,-1.0}
    //    };

    //    // G(x,y) = 1/2*Pi*sigma^2 * e^(-(x^2+y^2)/2*sigma^2
    //    private double[,] Gaussian = new double[3, 3]
    //    {   {0, 0, 0},
    //        {0, 0, 0},
    //        {0, 0, 0}
    //    };

    //    private double[,] FirstDerivativeX = new double[3, 3]
    //    {   {-1, 0, 1},
    //        {-2, 0, 2},
    //        {-1, 0, 1}
    //    };
    //    double[,] FirstDerivativeY = new double[3, 3]
    //    {   {1.0,2.0,1.0},
    //        {0.0,0.0,0.0},
    //        {-1.0,-2.0,-1.0}
    //    };
    //    double[,] Laplace = new double[3, 3]
    //    {   {0.0,-1.0,0.0},
    //        {-1.0,4.0,-1.0},
    //        {0.0,-1.0,0.0}
    //    };
    //    double[,] BinomialLPF = new double[3, 3]
    //    {   {1.0/16,2.0/16,1.0/16},
    //        {2.0/16,4.0/16,2.0/16},
    //        {1.0/16,2.0/16,1.0/16}
    //    };
    //    double[,] BinomialHPF = new double[3, 3]
    //    {   {-1.0,-2.0,-1.0},
    //        {-2.0,12.0,-2.0},
    //        {-1.0,-2.0,-1.0}
    //    };
    //    double[,] DifferenceofGaussian = new double[3, 3]
    //    {   {-1.0,-2.0,-1.0},
    //        {0.0,1,0.0},
    //        {1.0,2.0,1.0}
    //    };

    //    public Hashtable KernelsAvail = new Hashtable();

    //    public Kernels()
    //    {
    //        KernelsAvail["identity"] = identity;
    //        KernelsAvail["AverageLPF"] = AverageLPF;
    //        KernelsAvail["AverageLPF5x5"] = AverageLPF5x5;
    //        KernelsAvail["AverageHPF"] = AverageHPF;
    //        KernelsAvail["Sharpen f=0.5"] = Sharpenf50;
    //        KernelsAvail["Gaussian"] = GenerateGaussian(3, 1);
    //        KernelsAvail["FirstDerivativeX"] = FirstDerivativeX;
    //        KernelsAvail["FirstDerivativeY"] = FirstDerivativeY;
    //        KernelsAvail["LaPlace"] = Laplace;
    //        KernelsAvail["BinomialLPD"] = BinomialLPF;
    //        KernelsAvail["BinomialHPF"] = BinomialHPF;
    //        KernelsAvail["Difference of Gaussian"] = DifferenceofGaussian;
    //    }

    //    public double[,] GenerateGaussian(int size, double sigma = 1)
    //    {
    //        if (size % 2 != 1) throw new System.ArgumentException("Kernels are odd size (3,5,7..)", "original");

    //        double[,] result = new double[size, size];
    //        double sigmasq = sigma * sigma;
    //        int range = (int)Math.Floor((double)size / 2);

    //        for (int row = 0; row < size; row++)
    //        {
    //            for (int col = 0; col < size; col++)
    //            {
    //                int x = -range + col;
    //                int y = range - row;
    //                result[row, col] = 1 / (2 * Math.PI * sigmasq) * Math.Exp(-(Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigmasq));
    //            }
    //        }

    //        return result;
    //    }
    //}
}
