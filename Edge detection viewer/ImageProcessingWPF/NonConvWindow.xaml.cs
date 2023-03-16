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
    /// Interaction logic for NonConvWindow.xaml
    /// </summary>
    public partial class NonConvWindow : Window
    {
        //MainWindow mainwindow = new MainWindow();
        public int number;

        public NonConvWindow()
        {
            InitializeComponent();
        }

        private void InverseClick(object sender, RoutedEventArgs e)
        {
            number = 1;
            //Inverse(mainwindow.pix08);
            this.DialogResult = true;
        }

        private void UpsamplingClick(object sender, RoutedEventArgs e)
        {
            number = 2;
            //Inverse(mainwindow.pix08);
            this.DialogResult = true;
        }


        public void Inverse(byte[] pix08)
        {
            for (int j = 0; j < 256 * 256; j++)
            {

                pix08[j] = (byte)(255 - pix08[j]);
            }


        }


        public void upsampling(int pixelSize, byte[,] array, byte[,] increasedMatix, int sizeUp)
        {
            for (int j = 0; j < pixelSize; j++)
            {
                for (int k = 0; k < pixelSize; k++)
                {

                    for (int a = 0; a < sizeUp; a++)
                    {
                        for (int b = 0; b < sizeUp; b++)
                        {
                            increasedMatix[a + sizeUp * j, b + sizeUp * k] = array[j, k];

                        }
                    }



                }
            }
        }

        private void SubsamplingClick(object sender, RoutedEventArgs e)
        {
            number = 3;
            //Inverse(mainwindow.pix08);
            this.DialogResult = true;
        }

        public void Subsampling(int imageSize, int sizeDown,  byte[,] PastArray, byte[,] PostArray)
        {
            for (int j = 0; j < imageSize; j++)
            {
                for (int k = 0; k < imageSize; k++)
                {
                    if (k % sizeDown == 0 && j % sizeDown == 0)
                    {
                        PostArray[j/2, k/2] = PastArray[j, k];

                    }
                }
            }

           

        }


        private void RotationClick(object sender, RoutedEventArgs e)
        {
            number = 4;
            //Inverse(mainwindow.pix08);
            this.DialogResult = true;
        }


        public void rotate(int imageSize, double radian, byte[,] imageArray, byte[,] backgroundArray)
        {
            int i0 = 0;
            int j0 = 0;

            for (int i = 0; i < imageSize; i++)
            {
                for (int j = 0; j < imageSize; j++)
                {
                    i = i - imageSize / 2;
                    j = j - imageSize / 2;

                    //background[i, j] = crray[i, j];
                    i0 = (int)(Math.Cos(radian) * i - Math.Sin(radian) * j);
                    j0 = (int)(Math.Sin(radian) * i + Math.Cos(radian) * j);

                    i0 = imageSize + i0;
                    j0 = imageSize + j0;

                    i = i + imageSize / 2;
                    j = j + imageSize / 2;



                    backgroundArray[i0, j0] = imageArray[i, j];


                }
            }


        }





    }
}
