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
using System.Windows.Shapes;
using System.Linq;

namespace ImageProcessing2019
{
    /// <summary>
    /// Interaction logic for KernelSelection.xaml
    /// </summary>
    public partial class KernelSelection : Window
    {
        
        public Kernels.kerneldata kernelselect;

        private List<string> Kernels;
        private Kernels kernel;
        public KernelSelection()
        {
            InitializeComponent();
            GetDefinedKernels();
        }

        private void GetDefinedKernels()
        {
            Kernels = new List<string>();
            kernel = new Kernels();
            foreach(string k in kernel.KernelsAvail.Keys)
                Kernels.Add(k);
            cmbKernels.ItemsSource = Kernels.OrderBy(x=>x);


        }

        private void cmbKernels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string SelItem = cmbKernels.SelectedItem.ToString();
            kernelselect = (Kernels.kerneldata)kernel.KernelsAvail[SelItem];
            if (kernelselect == null)
            {
                MessageBox.Show("Invalid Selection");
                return;
            }
            SetupKernel();

        }

        private void SetupKernel()
        {
            double sum = 0;
            foreach (double num in kernelselect.weights)
                sum += num;
            KernelSum.Text = sum.ToString();
            if (kernelselect.IsLowPass == true)
            {
                ckIsLowPass.IsChecked = true;
                ckIsLowPass.Visibility = Visibility.Visible;
                HPConvert.Visibility = Visibility.Visible;
            }
            else
            {
                ckIsLowPass.Visibility = Visibility.Hidden;
                HPConvert.Visibility = Visibility.Hidden;
            }

            while (KernelMatrix.Children.Count > 0)
                KernelMatrix.Children.Remove(KernelMatrix.Children[0]);
            while (KernelMatrix.RowDefinitions.Count > 0)
                KernelMatrix.RowDefinitions.Remove(KernelMatrix.RowDefinitions[0]);
            while (KernelMatrix.ColumnDefinitions.Count > 0)
                KernelMatrix.ColumnDefinitions.Remove(KernelMatrix.ColumnDefinitions[0]);

            for (int mat = 0; mat < kernelselect.weights.GetLength(0); mat++)
            {
                ColumnDefinition col = new ColumnDefinition();
                KernelMatrix.ColumnDefinitions.Add(col);
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(15);
                KernelMatrix.RowDefinitions.Add(row);
            }
            for (int row = 0; row < kernelselect.weights.GetLength(0); row++)
            {
                for (int col = 0; col < kernelselect.weights.GetLength(1); col++)
                {
                    TextBox tb = new TextBox();
                    tb.Text = kernelselect.weights[row, col].ToString();
                    Grid.SetColumn(tb, col);
                    Grid.SetRow(tb, row);
                    KernelMatrix.Children.Add(tb);
                }
            }


        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox tb in KernelMatrix.Children.OfType<TextBox>())
            {
                kernelselect.weights[Grid.GetRow(tb), Grid.GetColumn(tb)] = double.Parse(tb.Text);
            }
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }

        private void HPConvert_Click(object sender, RoutedEventArgs e)
        {
            kernelselect = kernelselect.ConvertToHP();
            SetupKernel();
        }
    }
}
