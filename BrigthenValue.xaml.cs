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
    /// Interaction logic for BrigthenValue.xaml
    /// </summary>
    public partial class BrigthenValue : Window
    {
        public int BrightenValue = 0;
        public BrigthenValue()
        {
            InitializeComponent();
            BrightValue.Text = "0";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(BrightValue.Text, out BrightenValue))
            {
                MessageBox.Show("Invalid Number Entered");
                return;
            }

            Window.GetWindow(this).DialogResult = true;
            Window.GetWindow(this).Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = false;
            Window.GetWindow(this).Close();
        }
    }
}
