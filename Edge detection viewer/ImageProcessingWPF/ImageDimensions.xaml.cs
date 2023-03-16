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
        private int numberOfPixels;
        public int width;
        public int height;
        private List<int> listOfFactors;
        public ImageDimensions(int NOP)
        {
            InitializeComponent();
            numberOfPixels = NOP;

            tbSize.Text = numberOfPixels.ToString();

            int Factor = 0;

            if (SquareRoot(out Factor))
                width = height = Factor;
            else
            {
                listOfFactors = Factors(numberOfPixels);
                int noFactors = listOfFactors.Count;
                width = listOfFactors[noFactors - 2];
                height = listOfFactors[noFactors - 1];
            }

            tbWidth.Text = width.ToString();
            tbHeight.Text = height.ToString();
        }

        public bool SquareRoot(out int Factor)
        {
            int temp = (int)Math.Floor(Math.Sqrt(numberOfPixels));
            Factor = 0;
            if (temp * temp == numberOfPixels)
            {
                Factor = temp;
                return true;
            }
            return false;
        }

        public List<int> Factors(int number)
        {
            List<int> factors = new List<int>();
            int max = (int)Math.Sqrt(number);  //round down 
            for (int factor = 1; factor <= max; ++factor)
            { //test from 1 to the square root, or the int below it, inclusive. 
                if (number % factor == 0)
                {
                    factors.Add(factor);
                    if (factor != max)
                    { // Don't add the square root twice!  Thanks Jon 
                        factors.Add(number / factor);
                    }
                }
            }
            return factors;
        }

        private void bnOK_Click(object sender, RoutedEventArgs e)
        {
            int num1 = Convert.ToInt32(tbWidth.Text);
            int num2 = Convert.ToInt32(tbHeight.Text);

            this.DialogResult = true;
            //if (num1 * num2 != numberOfPixels)
            //{
            //    MessageBox.Show("The dimensions you entered do not seem to match with the number of pixels. Please re-enter.",
            //        "Image Dimension", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
            //else
            //{
            //    this.DialogResult = true;
            //}
        }

    }
}
