using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
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
            double[,] result = GenerateBinomial(size, param);
            int center = (int)Math.Floor((double)size / 2);
            for (int row = 0; row < size; row++)
                result[center, row] = 0;
            return result;
        }
        public static double[,] GenerateDy(int size = 3, double param = 0)
        {
            double[,] result = new double[size,size];
            double[,] temp = GenerateBinomial(size, param);
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                    result[row, col] = temp[col, row];
            }
            int center = (int)Math.Floor((double)size / 2);
            for (int col = 0; col < size; col++)
                result[col,center] = 0;
            return result;
        }
        public static double[,] GenerateLaPlace(int size = 3, double sigma = 0)
        {
            if (size > 5) throw new System.ArgumentException("Kernels only implemented for size 3 & 5", "original");
            if (size % 2 != 1) throw new System.ArgumentException("Kernels are odd size (3,5,7..)", "original");
            double[,] return3 = new double[3, 3] { { 0,1,0},{ 1,-4,-1},{0,1,0} };
            double[,] return5 = new double[5, 5]
                {{-4, -1, 0, -1, -4}, {-1, 2, 3, 2, -1}, {0, 3, 4, 3, 0}, {-1, 2, 3, 2, -1}, {-4, -1, 0, -1, -4}};
            if (size == 3)
                return return3;
            else
            {
                return return5;
            }
            ;
            //double[,] result = new double[size, size];
            //double sigmasq = sigma * sigma;
            //int range = (int)Math.Floor((double)size / 2);

            //for (int row = 0; row < size; row++)
            //{
            //    for (int col = 0; col < size; col++)
            //    {
            //        int x = -range + col;
            //        int y = range - row;
            //        result[row, col] = -(1 / (Math.PI * sigmasq*sigmasq))*(1-(row*row+col*col)/(2*sigmasq)) * Math.Exp(-(Math.Pow(x, 2) + Math.Pow(y, 2)) / (2 * sigmasq));
            //    }
            //}

            //double sum = 0;
            //foreach (double num in result) sum += num;
            //for (int row = 0; row < size; row++)
            //for (int col = 0; col < size; col++)
            //    result[row, col] /= sum;


            return result;
        }
        public static double[,] GenerateBinomial(int size = 3, double param = 0)
        {
            double[,] result = new double[size, size];

            double[] binomial = GetBinomialRow(size);
            double binomialsum = 0;
            foreach (var num in binomial) binomialsum += num;
            binomialsum *= binomialsum;
            for(int row=0;row<size;row++)
            for (int col = 0; col < size; col++)
                result[row, col] = (binomial[row] * binomial[col])/binomialsum;
            return result;
        }
        private static double[] GetBinomialRow(int size)
        {
            double[][] binomial = new double[size][];
            for (int row = 0; row < size; row++)
            {
                binomial[row] = new double[row+1];
                for(int col=0; col <=row; col++)
                    if (col == 0 || col == row)
                        binomial[row][col] = 1.0;
                    else
                        binomial[row][col] = binomial[row - 1][col - 1] + binomial[row - 1][col];

            }

            return binomial[size-1];
        }

        public void test()
        {
            double[,] test = GenerateLaPlace(9, 1.4);
            double laplsum = 0;
            foreach (var num in test) laplsum += num;
            test[0, 0] = 1;
        }
    }

}
