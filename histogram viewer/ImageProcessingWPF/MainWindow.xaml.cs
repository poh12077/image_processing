using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Drawing;

namespace ImageProcessingWPF
{
    public partial class MainWindow : Window
    {
        int width;
        int height;
      
        BitmapSource bmps;
        int stride;

        ImageDimensions ID;
        const int numberOfPixelsInWidth = 256; //size of downsampled array

        public MainWindow() //start at the beginning of the run
        {
            InitializeComponent();
        }
        void OpenColorImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                img.Source = new BitmapImage(fileUri);
                DisplayColorImage(openFileDialog.FileName);
            }
        }
        public void DrawingLineGraph8bit(int[] PixelForGraph8bit)
        {
            Line[] LineGray8bit = new Line[256];
            int pixelValueRange = 256;
            int max = 0;

            for (int i = 0; i < pixelValueRange; i++)
            {
                if (max < PixelForGraph8bit[i])
                {
                    max = PixelForGraph8bit[i];
                }
            }

            double ResizeConstantForX = (double)Graph.ActualWidth / pixelValueRange;
            double ResizeConstantForY = (double)Graph.ActualHeight / max;

            SolidColorBrush lineColor = new SolidColorBrush();
            lineColor.Color = Colors.Gray;

            for (int i = 0; i < pixelValueRange - 1; i++)
            {
                LineGray8bit[i] = new Line();

                LineGray8bit[i].Stroke = lineColor;
                LineGray8bit[i].StrokeThickness = 2;

                LineGray8bit[i].X1 = i * ResizeConstantForX;
                LineGray8bit[i].Y1 = Graph.ActualHeight - (PixelForGraph8bit[i] * ResizeConstantForY);
                LineGray8bit[i].X2 = (i + 1) * ResizeConstantForX;
                LineGray8bit[i].Y2 = Graph.ActualHeight - (PixelForGraph8bit[i + 1] * ResizeConstantForY);

                Graph.Children.Add(LineGray8bit[i]);
            }
        }

        public void DrawingLineGraphRGB(int[] redArray, int[] greenArray, int[] blueArray)
        {
            Graph.Children.Clear();

            Line[] redLine = new Line[256];
            Line[] greenLine = new Line[256];
            Line[] blueLine = new Line[256];

            int max = 0;
            int size = redArray.Length ;

            for (int i = 0; i < size ; i++)
            {
                if (max < redArray[i] )
                {
                    max = redArray[i];
                }
                if (max < greenArray[i])
                {
                    max = greenArray[i];
                }
                if (max < blueArray[i])
                {
                    max = blueArray[i];
                }

            }

            double ResizeConstantForX = (double)Graph.ActualWidth / size;
            double ResizeConstantForY;

            if (max != 0)
            {
                ResizeConstantForY = (double)Graph.ActualHeight / max;
            }
            else
            {
                ResizeConstantForY = 1;
            }

            SolidColorBrush redlineColor = new SolidColorBrush();
            redlineColor.Color = Colors.Red;

            SolidColorBrush greedlineColor = new SolidColorBrush();
            greedlineColor.Color = Colors.Green;

            SolidColorBrush bluelineColor = new SolidColorBrush();
            bluelineColor.Color = Colors.Blue;

            for (int i = 0; i < size - 1; i++)
            {
                redLine[i] = new Line();
                redLine[i].Stroke = redlineColor;
                redLine[i].StrokeThickness = 2;

                redLine[i].X1 = i * ResizeConstantForX;
                redLine[i].Y1 = Graph.ActualHeight - (redArray[i] * ResizeConstantForY);
                redLine[i].X2 = (i + 1) * ResizeConstantForX;
                redLine[i].Y2 = Graph.ActualHeight - (redArray[i + 1] * ResizeConstantForY);

                Graph.Children.Add(redLine[i]);

                greenLine[i] = new Line();
                greenLine[i].Stroke = greedlineColor;
                greenLine[i].StrokeThickness = 2;

                greenLine[i].X1 = i * ResizeConstantForX;
                greenLine[i].Y1 = Graph.ActualHeight - (greenArray[i] * ResizeConstantForY);
                greenLine[i].X2 = (i + 1) * ResizeConstantForX;
                greenLine[i].Y2 = Graph.ActualHeight - (greenArray[i + 1] * ResizeConstantForY);

                Graph.Children.Add(greenLine[i]);

                blueLine[i] = new Line();
                blueLine[i].Stroke = bluelineColor;
                blueLine[i].StrokeThickness = 2;

                blueLine[i].X1 = i * ResizeConstantForX;
                blueLine[i].Y1 = Graph.ActualHeight - (blueArray[i] * ResizeConstantForY);
                blueLine[i].X2 = (i + 1) * ResizeConstantForX;
                blueLine[i].Y2 = Graph.ActualHeight - (blueArray[i + 1] * ResizeConstantForY);

                Graph.Children.Add(blueLine[i]);

            }
        }

        public void DrawingGraph8bit(int[] PixelForGraph8bit)
        {
            Graph.Children.Clear();

            Ellipse[] EllipseGray8bit = new Ellipse[256];
            int pixelValueRange = 256;
            int max = 0;

            for (int i = 0; i < pixelValueRange; i++)
            {
                if (max < PixelForGraph8bit[i])
                {
                    max = PixelForGraph8bit[i];
                }
            }

            double ResizeConstantForX = (double)Graph.ActualWidth / pixelValueRange;
            double ResizeConstantForY = (double)Graph.ActualHeight / max;
            double radius = 7;

            for (int i = 0; i < pixelValueRange; i++)
            {
                EllipseGray8bit[i] = new Ellipse();
                EllipseGray8bit[i].Width = radius;
                EllipseGray8bit[i].Height = radius;
                EllipseGray8bit[i].Fill = new SolidColorBrush(Colors.Red);

                Canvas.SetLeft(EllipseGray8bit[i], i * ResizeConstantForX - radius / 2);
                Canvas.SetBottom(EllipseGray8bit[i], PixelForGraph8bit[i] * ResizeConstantForY - radius / 2);

                Graph.Children.Add(EllipseGray8bit[i]);
            }
        }
        private void bnOpen08_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage08(fileName);
            }

        }
        private void DisplayImage08(string fileName)
        {

            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                int iTotalSize = (int)br.BaseStream.Length;//256*256
                int width;
                int height;
                byte[] pix08;
                int[] PixelForGraph8bit = new int[256];
                ID = new ImageDimensions();

                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text); // get image dimension from user input       
                    height = Convert.ToInt32(ID.tbHeight.Text);

                    pix08 = new byte[iTotalSize];

                    for (int i = 0; i < iTotalSize; ++i)
                    {
                        pix08[i] = br.ReadByte();

                    }
                    br.Close();


                    for (int i = 0; i < iTotalSize; i++)
                    {
                        PixelForGraph8bit[pix08[i]] += 1;
                    }

                    int bitsPerPixel = 8;
                    int padding = 0;
                    stride = width * (bitsPerPixel / 8) + padding;
                    // stride =  pixel byte *  width + padding

                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                      pix08, stride);

                    img.Source = bmps;
                    DrawingGraph8bit(PixelForGraph8bit);
                    DrawingLineGraph8bit(PixelForGraph8bit);
                }
                else
                {
                    br.Close();
                }
            }


            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayRawRGB(string fileName)
        {

            int[] redGraph = new int[256];
            int[] greenGraph = new int[256];
            int[] blueGraph = new int[256];
            byte[] pix08;
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                int iTotalSize = (int)br.BaseStream.Length ;//256*256*3

                ID = new ImageDimensions();
                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text); // get image dimension from user input       
                    height = Convert.ToInt32(ID.tbHeight.Text);

                    pix08 = new byte[iTotalSize];

                    for (int i = 0; i < iTotalSize; ++i)
                    {
                        pix08[i] = br.ReadByte();

                    }
                    br.Close();


                    for (int i = 0; i < iTotalSize; i++)
                    {
                        redGraph[pix08[i]] += 1;
                        i++;
                        greenGraph[pix08[i]] += 1;
                        i++;
                        blueGraph[pix08[i]] += 1;

                    }

                    int bitsPerPixel = 24;
                    int padding = 0;
                    stride = width * (bitsPerPixel / 8) + padding;
                    // stride =  pixel byte *  width + padding

                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null,
                      pix08, stride);

                    img.Source = bmps;
                    DrawingLineGraphRGB(redGraph, greenGraph, blueGraph);
                }
                else
                {
                    br.Close();
                }
            }


            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayColorImage(string fileName)
        {
            int[] redGraph = new int[256];
            int[] greenGraph = new int[256];
            int[] blueGraph = new int[256];

            try
            {
                Graph.Children.Clear();
                Bitmap image = new Bitmap(fileName);

                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                       System.Drawing.Color pixel = image.GetPixel(i, j);
                        
                        redGraph[pixel.R] += 1;
                        greenGraph[pixel.G] += 1;
                        blueGraph[pixel.B] += 1;

                    }
                }
                    DrawingLineGraphRGB(redGraph, greenGraph, blueGraph);
             
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bnOpen16_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage16(fileName);
            }

        }

        void OpenRawRGB(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
          //  ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayRawRGB(fileName);
            }
        }
        private void DisplayImage16(string fileName)
        {
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                int iTotalSize = (int)br.BaseStream.Length / 2;
                // int iTotalSize = 512*512*2;
                ID = new ImageDimensions();
                int width;
                int height;
                ushort[] pix16;
                int[] PixelForGraph16bit = new int[256 * 256];
                double[] DownsampledPixelForGraph16bit = new double[numberOfPixelsInWidth];

                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    pix16 = new ushort[iTotalSize];
                    

                    int numberOfZero = 0;
                    int min = 65535;
                    int max = 0;

                    // lena 16bit : min = 5912, max = 62707
                    for (int i = 0; i < iTotalSize; ++i)
                    {
                        pix16[i] = br.ReadUInt16();

                        // calcute min,max

                        //if (pix16[i] < min)
                        //{
                        //    min = pix16[i];
                            
                        //}
                        //if(pix16[i] > max)
                        //{
                        //    max = pix16[i];
                        //}

                    }
                    br.Close();


                    for (int i = 0; i < iTotalSize; i++)
                    {
                        PixelForGraph16bit[pix16[i]] += 1;
                        
                    }

                    // calculate number of zero
                    // number of zero in lena16 bit is 65320,  
                    // number of not zero is 216

                    //for(int i=0; i < 256*256 ; i++)
                    //{
                    //    if (PixelForGraph16bit[i] == 0)
                    //    {
                    //        numberOfZero++;
                    //    }
                    //}
                  
                    int bitsPerPixel = 16;
                    int padding = 0;
                    stride = width * (bitsPerPixel / 8) + padding;
                    // stride =  pixel byte *  width + padding
                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray16, null,
                      pix16, stride);
                    img.Source = bmps;
                    
                    // SimpleDownSampling(PixelForGraph16bit, DownsampledPixelForGraph16bit);
                  //    MaxDownSampling(PixelForGraph16bit, DownsampledPixelForGraph16bit);
                   //   MinDownSampling(PixelForGraph16bit, DownsampledPixelForGraph16bit);
                   AverageDownSampling(PixelForGraph16bit, DownsampledPixelForGraph16bit);

                    // DrawingEllipseAndLine16bit(PixelForGraph16bit);
                       DrawingEllipseAndLine16bit(DownsampledPixelForGraph16bit);
                }

                else
                {
                    br.Close();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DrawingEllipse16bit()
        {
            Ellipse[] EllipseGray16bit = new Ellipse[numberOfPixelsInWidth];
            int max = 0;
            int[] DownsampledPixelForGraph16bit = new int[numberOfPixelsInWidth];

            for (int i = 0; i < numberOfPixelsInWidth; i++)
            {
                if (max < DownsampledPixelForGraph16bit[i])
                {
                    max = DownsampledPixelForGraph16bit[i];
                }
            }
            double ResizeConstantForX = (double)Graph.ActualWidth / numberOfPixelsInWidth;
            //double ResizeConstantForX = 1;
            double ResizeConstantForY = (double)Graph.ActualHeight / max;
            double radius = 5;

              for (int i = 0; i < numberOfPixelsInWidth; i++)
            {
                EllipseGray16bit[i] = new Ellipse();
                EllipseGray16bit[i].Width = radius;
                EllipseGray16bit[i].Height = radius;
                EllipseGray16bit[i].Fill = new SolidColorBrush(Colors.Red);

                Canvas.SetLeft(EllipseGray16bit[i], i * ResizeConstantForX - radius / 2);
                Canvas.SetBottom(EllipseGray16bit[i], DownsampledPixelForGraph16bit[i] * ResizeConstantForY - radius / 7);

                Graph.Children.Add(EllipseGray16bit[i]);
            }
        }
        public void DrawingLine16bit()
        {
            Line[] lineGray16bit = new Line[numberOfPixelsInWidth];
            int max = 0;
            int[] DownsampledPixelForGraph16bit = new int[numberOfPixelsInWidth];

            for (int i = 0; i < numberOfPixelsInWidth; i++)
            {
                if (max < DownsampledPixelForGraph16bit[i])
                {
                    max = DownsampledPixelForGraph16bit[i];
                }
            }

            double ResizeConstantForX = (double)Graph.ActualWidth  / numberOfPixelsInWidth;
            double ResizeConstantForY = (double)Graph.ActualHeight / max;
          //   double ResizeConstantForX = 1;

            for (int i = 0; i < numberOfPixelsInWidth - 1; i++)
            {
                lineGray16bit[i] = new Line();

                SolidColorBrush lineColor = new SolidColorBrush();
                lineColor.Color = Colors.Gray;

                lineGray16bit[i].Stroke = lineColor;
                lineGray16bit[i].StrokeThickness = 2;

                lineGray16bit[i].X1 = i * ResizeConstantForX;
                lineGray16bit[i].Y1 = Graph.ActualHeight - (DownsampledPixelForGraph16bit[i] * ResizeConstantForY);
                lineGray16bit[i].X2 = (i + 1) * ResizeConstantForX;
                lineGray16bit[i].Y2 = Graph.ActualHeight - (DownsampledPixelForGraph16bit[i + 1] * ResizeConstantForY);

                Graph.Children.Add(lineGray16bit[i]);
            }
        
        }
        public void DrawingEllipseAndLine16bit( double[] array )
        {
            Graph.Children.Clear();

            int size = array.Length;
            Ellipse[] EllipseGray16bit = new Ellipse[size];
            Line[] lineGray16bit = new Line[size];
            double max = 0;

            for (int i = 0; i < size; i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                }
            }
         

            double ResizeConstantForX = (double)Graph.ActualWidth / size;
           // double ResizeConstantForX = 1;
            double ResizeConstantForY;

            if (max != 0)
            {
                ResizeConstantForY = (double)Graph.ActualHeight / max;
            }
            else
            {
                ResizeConstantForY = 1;
            }
            
            double radius = 5;

            for (int i = 0; i < size; i++)
            {
                EllipseGray16bit[i] = new Ellipse();
                EllipseGray16bit[i].Width = radius;
                EllipseGray16bit[i].Height = radius;
                EllipseGray16bit[i].Fill = new SolidColorBrush(Colors.Red);
                Canvas.SetLeft(EllipseGray16bit[i], i * ResizeConstantForX - radius / 2);
                Canvas.SetBottom(EllipseGray16bit[i], array[i] * ResizeConstantForY - radius / 2);
                Graph.Children.Add(EllipseGray16bit[i]);

                if (i < size - 1)
                {
                    lineGray16bit[i] = new Line();
                    SolidColorBrush lineColor = new SolidColorBrush();
                    lineColor.Color = Colors.Gray;
                    lineGray16bit[i].Stroke = lineColor;
                    lineGray16bit[i].StrokeThickness = 2;
                    lineGray16bit[i].X1 = i * ResizeConstantForX;
                    lineGray16bit[i].Y1 = Graph.ActualHeight - (array[i] * ResizeConstantForY);
                    lineGray16bit[i].X2 = (i + 1) * ResizeConstantForX;
                    lineGray16bit[i].Y2 = Graph.ActualHeight - (array[i + 1] * ResizeConstantForY);

                    Graph.Children.Add(lineGray16bit[i]);
                }

            }
        }
        // for test
        public void DrawingEllipseAndLine16bitForTest_origion(double[] array)
        {
            Graph.Children.Clear();
            int size = array.Length;

            Ellipse[] EllipseGray16bit = new Ellipse[size];
            Line[] lineGray16bit = new Line[size];
            double max = 0;

            for (int i = 0; i < size; i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                }
            }

            double ResizeConstantForX = (double)Graph.ActualWidth / size;
            double ResizeConstantForY = (double)Graph.ActualHeight / 10;
            double radius = 3;

            for (int i = 0; i < size; i++)
            {
                EllipseGray16bit[i] = new Ellipse();
                EllipseGray16bit[i].Width = radius;
                EllipseGray16bit[i].Height = radius;
                EllipseGray16bit[i].Fill = new SolidColorBrush(Colors.Red);
                Canvas.SetLeft(EllipseGray16bit[i], i * ResizeConstantForX - radius / 2);
                Canvas.SetBottom(EllipseGray16bit[i], array[i] * ResizeConstantForY - radius / 7);
                Graph.Children.Add(EllipseGray16bit[i]);

            }
        }
        //for test
        public void DrawingEllipseAndLine16bitForTest_downsampling(double[] array)
        {
           
            int size = array.Length;

            Ellipse[] EllipseGray16bit = new Ellipse[size];
            Line[] lineGray16bit = new Line[size];
            double max = 0;

            for (int i = 0; i < size; i++)
            {
                if (max < array[i])
                {
                    max = array[i];
                }
            }

            double ResizeConstantForX = (double)Graph.ActualWidth / size;
            double ResizeConstantForY = (double)Graph.ActualHeight / 10;
            double radius = 3;

            for (int i = 0; i < size; i++)
            {
                EllipseGray16bit[i] = new Ellipse();
                EllipseGray16bit[i].Width = radius;
                EllipseGray16bit[i].Height = radius;
                EllipseGray16bit[i].Fill = new SolidColorBrush(Colors.Blue);
                Canvas.SetLeft(EllipseGray16bit[i], i * ResizeConstantForX - radius / 2);
                Canvas.SetBottom(EllipseGray16bit[i], array[i] * ResizeConstantForY - radius / 7);
                Graph.Children.Add(EllipseGray16bit[i]);

            }
        }
        // replace 0 into nearby value in array  
        static void RemoveZero(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] == 0)
                {
                    array[i] = array[i - 1];
                }
            }
        }
        static void SimpleDownSampling(int[] origionalArray, int[] arrayForDownsampling)
        {
            int origionalArraySize = origionalArray.Length;
            int arrayForDownsamplingSize = arrayForDownsampling.Length;

            for (int i = 0; i<arrayForDownsamplingSize; i++)
            {
                arrayForDownsampling[i] = origionalArray[ (origionalArraySize/arrayForDownsamplingSize)*i ];
            }

         
        }

        static void MaxDownSampling(int[] preArray, double[] postArray)
        {
            int preArraySize = preArray.Length;
            int postArraySize = postArray.Length;
            double max = 0;
            int j = 0;

            for (int i = 0; i < preArraySize; i++)
            {
                if (max < preArray[i])
                {
                    max = preArray[i];
                }

                if ((i + 1) % (preArraySize / postArraySize) == 0)
                {
                    postArray[j] = max;
                    j++;
                    max = 0;
                }

            }
        }
        static void MinDownSampling(int[] preArray, double[] postArray)
        {
            int preArraySize = preArray.Length;
            int postArraySize = postArray.Length;
            double min = 65535;
            int j = 0;

            for (int i = 0; i < preArraySize; i++)
            {
                if (min > preArray[i])
                {
                    min = preArray[i];
                }

                if ((i + 1) % (preArraySize / postArraySize) == 0)
                {
                    postArray[j] = min;
                    j++;
                    min = 65535;
                }

            }
        }
        static void AverageDownSampling(int[] preArray, double[] postArray)
        {
            int preArraySize = preArray.Length;
            int postArraySize = postArray.Length;
            double average = 0;
            double sum = 0;
            int j = 0;
            int interval = preArraySize / postArraySize;

            for (int i = 0; i < preArraySize; i++)
            {
                sum = sum + preArray[i];

                if ((i + 1) % interval == 0)
                {
                    average = (double)sum / interval;

                    postArray[j] = average;
                    j++;
                    sum = 0;
                }

            }
        }

        //for test
        static void MaxDownSamplingForDouble(double[] origionalArray, double[] arrayForDownsampling)
        {
            int origionalArraySize = origionalArray.Length;
            int arrayForDownsamplingSize = arrayForDownsampling.Length;
            double max = 0;
            int j = 0;

            for (int i = 0; i < origionalArraySize; i++)
            {
                if (max < origionalArray[i])
                {
                    max = origionalArray[i];
                }

                if ((i + 1) % (origionalArraySize / arrayForDownsamplingSize) == 0)
                {
                    arrayForDownsampling[j] = max;
                    j++;
                    max = 0;
                }

            }
        }
        //for test
        static void MinDownSamplingForDouble(double[] origionalArray, double[] arrayForDownsampling)
        {
            int origionalArraySize = origionalArray.Length;
            int arrayForDownsamplingSize = arrayForDownsampling.Length;
            double min = 65535;
            int j = 0;

            for (int i = 0; i < origionalArraySize; i++)
            {
                if (min > origionalArray[i])
                {
                    min = origionalArray[i];
                }

                if ((i + 1) % (origionalArraySize / arrayForDownsamplingSize) == 0)
                {
                    arrayForDownsampling[j] = min;
                    j++;
                    min = 65535;
                }

            }
        }
        //for test
        static void AverageDownSamplingForDouble(double[] origionalArray, double[] arrayForDownsampling)
        {
            int origionalArraySize = origionalArray.Length;
            int arrayForDownsamplingSize = arrayForDownsampling.Length;
            double average = 0;
            double sum = 0;
            int j = 0;

            for (int i = 0; i < origionalArraySize; i++)
            {
                sum = sum + origionalArray[i];

                if ((i + 1) % (origionalArraySize / arrayForDownsamplingSize) == 0)
                {
                    average = sum / (origionalArraySize / arrayForDownsamplingSize);

                    arrayForDownsampling[j] = average;
                    j++;
                    sum = 0;
                }

            }
        }
        //for test
        void DownsamplingTest(object sender, RoutedEventArgs e)
        {
            int halfFrequency = 16;
            int frequency = halfFrequency / 2;
            const int size = 1024;
            const int downsampledSize = 32;
            double[] x = new double[size];
            double[] y = new double[size];
            double[] downsampledY = new double[downsampledSize];
            double pi = Math.PI;
            //  1 frequency : number of pixel = 1 : size / frequency
            for (int i = 0; i < size; i++)
            {
                x[i] = ((2 * pi) * frequency ) / size * i;
                y[i] = Math.Abs(Math.Sin(x[i]));

            }

              MaxDownSamplingForDouble(y, downsampledY);
            //    MinDownSamplingForDouble(y, downsampledY);
           // AverageDownSamplingForDouble(y, downsampledY);

            DrawingEllipseAndLine16bitForTest_origion(y);
            DrawingEllipseAndLine16bitForTest_downsampling(downsampledY);
        }

    }
}

//test