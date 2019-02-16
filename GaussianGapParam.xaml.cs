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

namespace ImageProcessing2019
{
    /// <summary>
    /// Interaction logic for GaussianGapParam.xaml
    /// </summary>
    public partial class GaussianGapParam : Window
    {
        public double Sigma1 = 0;
        public double Sigma2 = 0;
        public GaussianGapParam()
        {
            InitializeComponent();
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(txtsignam1.Text, out Sigma1))
                MessageBox.Show("Sigma 1 does not contain a value");
            if (!double.TryParse(txtsignam2.Text, out Sigma2))
                MessageBox.Show("Sigma 2 does not contain a value");
            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Sigma1 = 0;
            Sigma2 = 0;
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }
    }
}
