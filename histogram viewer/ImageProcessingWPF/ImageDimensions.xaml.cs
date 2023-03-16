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
    /// Interaction logic for ImageDimensions.xaml
    /// </summary>
    public partial class ImageDimensions : Window
    {

        public ImageDimensions()
        {
            InitializeComponent();

        }

        private void bnOK_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;

        }

    }


}
