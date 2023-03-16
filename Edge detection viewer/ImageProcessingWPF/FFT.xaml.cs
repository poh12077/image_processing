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

namespace ImageProcessingWPF
{
    /// <summary>
    /// Interaction logic for FFT.xaml
    /// </summary>
    public partial class FFT : Window
    {
        public FFT()
        {
            InitializeComponent();
        }

        public int number;

        private void Lena(object sender, RoutedEventArgs e)
        {
            number = 1;
            //Inverse(mainwindow.pix08);
            this.DialogResult = true;
        }

        private void Mri(object sender, RoutedEventArgs e)
        {
            number = 2;

            this.DialogResult = true;
        }
    }
}
