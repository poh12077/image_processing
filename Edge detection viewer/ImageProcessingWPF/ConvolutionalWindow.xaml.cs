using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
//using Microsoft.ML.Probabilistic.Math;

namespace ImageProcessingWPF
{
    /// <summary>
    /// Interaction logic for ConvolutionalWindow.xaml
    /// </summary>
    public partial class ConvolutionalWindow : Window
    {
        int filterSize = 3;
        int brightness = 1;
        //double[] smoothoing1 = { 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public int number;
        //Microsoft.ML.Probabilistic.Math.Matrix filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, smoothoing1);

        public ConvolutionalWindow()
        {
            InitializeComponent();

        }

        private void lowpass(object sender, RoutedEventArgs e)
        {
            number = 1;
            this.DialogResult = true;
        }

        private void highpass(object sender, RoutedEventArgs e)
        {
            number = 2;
            this.DialogResult = true;
        }

        private void sobelx(object sender, RoutedEventArgs e)
        {
            number = 3;
            this.DialogResult = true;
        }
        private void sobely(object sender, RoutedEventArgs e)
        {
            number = 4;
            this.DialogResult = true;

        }

        private void laplacian1(object sender, RoutedEventArgs e)
        {
            number = 5;
            this.DialogResult = true;
        }

        private void laplacian2(object sender, RoutedEventArgs e)
        {
            number = 6;
            this.DialogResult = true;
        }
        public void convolutionalMultiply(int imageSize, double[,] originalImageArray, double[,] convolutionalMultipliedImageArray, Microsoft.ML.Probabilistic.Math.Matrix filter)
        {

            double[] matrixPieceArray = new double[filterSize * filterSize];

            int c1 = 0;
            double sumOfMatrix = 0;
            for (int a = 0; a < imageSize - filterSize + 1; a++)
            {
                for (int b = 0; b < imageSize - filterSize + 1; b++)
                {
                    //derive small piece matrix from whole matrix
                    for (int i = 0; i < filterSize; i++)
                    {
                        for (int j = 0; j < filterSize; j++)
                        {
                            matrixPieceArray[c1] = originalImageArray[a + i, j + b];

                            c1 += 1;
                        }
                    }
                    c1 = 0;
                    Microsoft.ML.Probabilistic.Math.Matrix matrixPiece = new Microsoft.ML.Probabilistic.Math.Matrix(filterSize, filterSize, matrixPieceArray);
                    Microsoft.ML.Probabilistic.Math.Matrix multipliedMatrix = new Microsoft.ML.Probabilistic.Math.Matrix(filterSize, filterSize);
                    multipliedMatrix.SetToElementwiseProduct(matrixPiece, filter); // multiply element by element
                                                                                   //summate every element 
                    for (int e = 0; e < filterSize; e++)
                    {
                        for (int f = 0; f < filterSize; f++)
                        {
                            sumOfMatrix += multipliedMatrix[e, f];
                        }
                    }
                    convolutionalMultipliedImageArray[a, b] = sumOfMatrix / brightness; // adjust brightness
                    sumOfMatrix = 0;
                }
            }

        }



    }
}
