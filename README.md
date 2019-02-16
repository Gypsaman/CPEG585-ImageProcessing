# CPEG585-ImageProcessing
Image Processing Assignments 1 and 2
See Imaging Processing.pdf for screen shots of the program

To create the multiple kernels requested in assignment 1 the kerneldata class was created.
```c#
        public class kerneldata
        {
            private int _size;
            private double _param;

            public double[,] weights;
            public bool IsLowPass = true;
            public delegate double[,] CreateDelegate(int size = 3, double param = 0.0);

            \\Function to generate kernel
            public kerneldata(CreateDelegate delfunc,int size=3, double param=0.0)
            {
                CreateDelegate del = delfunc;
                weights = delfunc(size, param);
                _size = size;
                _param = param;
            }
            
            \\Convert to High Pass Filter
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
```
