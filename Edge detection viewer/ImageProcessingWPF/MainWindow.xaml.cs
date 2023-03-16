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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessingWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public struct COMPLEX
    {
        public double re, im;


    }

    public partial class MainWindow : Window
    {

        public byte[] pix08;
        public byte[] pix08_subsampling;

        double[] pixDouble;
        int[] pixInt;

        public byte[,] pix08_2d;
        public byte[,] pix08_2d_subsampling;

        public byte[] pix08_4times;
        double[,] pix_convMultiplied;
        double[,] pix_conv;
        byte[] pix1d_conv;
        COMPLEX[] m_CmpxTemp1;
        COMPLEX[] m_CmpxTemp2;

        double[] temp;
        float[] tempFloat;
        byte[] tempByte;
        int[] tempInt;

        public double[] pix64;
        //ushort[] pix16;
        int width;
        int height;
        int stride;
        public int iTotalSize;
        BitmapSource bmps;
        ImageDimensions ID;
        ConvolutionalWindow CW;
        NonConvWindow nonConvWindow;
        FFT fft;

         double[] highpass1 = { 0, (float)-1 / 4, 0, (float)-1 / 4, 2, (float)-1 / 4, 0, (float)-1 / 4, 0 };

         double[] smoothing1 = { (double)1/9, (double)1 /9, (double)1 /9, (double)1 /9, (double)1 /9, (double)1 /9, (double)1 /9, (double)1 /9, (double)1 /9 };
        double[] sobel1 = { 1, 0, -1, 2, 0, -2, 1, 0, -1 }; //x filter
        double[] sobel2 = { -1, -2, -1, 0, 0, 0, 1, 2, 1 }; //y filter
        double[] laplacian1 = { 0, -1, 0, -1, 4, -1, 0, -1, 0 };
        double[] laplacian2 = { -1, -1, -1, -1, 8, -1, -1, -1, -1 };

        Microsoft.ML.Probabilistic.Math.Matrix filter;
        public MainWindow()
        {
            InitializeComponent();
            bnSaveJPG.IsEnabled = false;

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
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.         
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                byte pixByte;
                int i;
                int iTotalSize = (int)br.BaseStream.Length;

                // Get the dimensions of the image from the user
                ID = new ImageDimensions(iTotalSize);
                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    canvas.Width = width;
                    canvas.Height = height;
                    img.Width = width;
                    img.Height = height;
                    pix08 = new byte[iTotalSize];
                    pixDouble = new double[iTotalSize];
                    pixInt = new int[iTotalSize];

                    for (i = 0; i < iTotalSize; ++i)
                    {
                        pixByte = (byte)(br.ReadByte());
                        pix08[i] = pixByte;
                        // pix08[i] = CW.pix08Conv[i];
                        
                    }
                    br.Close();

                    //ByteToDouble(iTotalSize, pix08, pixDouble);
                    //ByteToInt(iTotalSize, pix08, pixInt);

                    int bitsPerPixel = 8;
                    stride = (width * bitsPerPixel + 7) / 8;

                    // Single step creation of the image
                    
                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                      pix08, stride);

                    //test
                    //bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                    //  pixDouble, stride);
                    img.Source = bmps;
                    bnSaveJPG.IsEnabled = true;
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

        private void ButtonFFT(object sender, RoutedEventArgs e)
        {
             fft = new FFT();
            if (fft.ShowDialog() == true)
            {
                if (fft.number == 1)
                {
                    bnOpen08_Click_lenaFFT();
                }
                else
                {
                    bnOpen08_Click_mri();
                }
            }
        }

        /*
        private void bnOpen08_Click_mri()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage08_mri(fileName);
            }
        }
        private void DisplayImage08_mri(string fileName)
        {
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.         
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                //double pixByte;
                int i;
                //int iTotalSize = (int)br.BaseStream.Length;
                int iTotalSize = 512 * 256;

                // Get the dimensions of the image from the user
                ID = new ImageDimensions(iTotalSize);
                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    canvas.Width = width;
                    canvas.Height = height;
                    img.Width = width;
                    img.Height = height;
                    // pix64 = new double[iTotalSize];
                    m_CmpxTemp1 = new COMPLEX[iTotalSize];
                    m_CmpxTemp2 = new COMPLEX[iTotalSize];

                    temp = new double[iTotalSize];
                    tempFloat = new float[iTotalSize];
                    tempByte = new byte[iTotalSize];
                    tempInt = new int[iTotalSize];

                    //mri

                    for (i = 0; i < iTotalSize; ++i)
                    {
                        m_CmpxTemp1[i].re = (double)br.ReadSingle();//mri                                       
                        m_CmpxTemp1[i].im = (double)br.ReadSingle();
                        // pix08[i] = CW.pix08Conv[i];

                    }

                    //lena

                    //for (i = 0; i < iTotalSize; ++i)
                    //{
                    //    m_CmpxTemp1[i].re = (double)br.ReadByte();//mri                                       
                    //    m_CmpxTemp1[i].im = 0;
                    //    // pix08[i] = CW.pix08Conv[i];

                    //}
                    br.Close();

                    fftshift(m_CmpxTemp1, m_CmpxTemp2, height, width);
                    fft2D(m_CmpxTemp2, height, width,0);
                    fftshift(m_CmpxTemp2, m_CmpxTemp1, height, width);


                    Magnitude(height, width, m_CmpxTemp1, temp);

                    //DoubleToFloat(iTotalSize, tempFloat, temp);
                    //DoubleToByte(iTotalSize, tempByte, temp);
                    DoubleToInt(iTotalSize, tempInt, temp);

                    int bitsPerPixel = 8;
                    stride = (width * bitsPerPixel + 7) / 8;

                    // Single step creation of the image
                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                        tempInt, stride);
                    img.Source = bmps;
                    bnSaveJPG.IsEnabled = true;
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

        */


        private void bnOpen08_Click_mri()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage08_mri(fileName);
            }
        }
        private void DisplayImage08_mri(string fileName)
        {
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.         
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                //double pixByte;
                int i;
               // int iTotalSize = (int)br.BaseStream.Length; //size of byte
                int iTotalSize = 512 * 256;

                // Get the dimensions of the image from the user
                ID = new ImageDimensions(iTotalSize);
                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    canvas.Width = width;
                    canvas.Height = height;
                    img.Width = width;
                    img.Height = height;
                    // pix64 = new double[iTotalSize];
                    m_CmpxTemp1 = new COMPLEX[iTotalSize];
                    m_CmpxTemp2 = new COMPLEX[iTotalSize];

                    temp = new double[iTotalSize];
                    tempFloat = new float[iTotalSize];
                    tempByte = new byte[iTotalSize];
                    tempInt = new int[iTotalSize];

                    //mri

                    for (i = 0; i < iTotalSize; ++i)
                    {
                        m_CmpxTemp1[i].re = (double)br.ReadSingle();//mri                                       
                        m_CmpxTemp1[i].im = (double)br.ReadSingle();
                        // pix08[i] = CW.pix08Conv[i];

                    }

                   
                    br.Close();

                    fftshift(m_CmpxTemp1, m_CmpxTemp2, height, width);
                    fft2D(m_CmpxTemp2, height, width, 0);
                    fftshift(m_CmpxTemp2, m_CmpxTemp1, height, width);


                    Magnitude(height, width, m_CmpxTemp1, temp);

                    DoubleToFloat(iTotalSize, tempFloat, temp);
                    //DoubleToByte(iTotalSize, tempByte, temp);
                    // DoubleToInt(iTotalSize, tempInt, temp);
                    Calculate(tempFloat, iTotalSize);

                    int bitsPerPixel = 32;
                    stride = (width * bitsPerPixel + 7) / 8;
                  //  BitmapPalette myPalette = new BitmapPalette(colors);

                    // Single step creation of the image
                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray32Float, null,
                        tempFloat, stride);
                    img.Source = bmps;
                    bnSaveJPG.IsEnabled = true;
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

        private void Calculate(float[] array, int size)
        {
            for(int i=0;i<size;i++)
            {
                array[i] = array[i]/10  ;
            }
        }


        private void bnOpen08_Click_lenaFFT()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage08_lenaFFT(fileName);
            }
        }
        private void DisplayImage08_lenaFFT(string fileName)
        {
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.         
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                //double pixByte;
                int i;
                int iTotalSize = (int)br.BaseStream.Length;
                //int iTotalSize = 512 * 256;

                // Get the dimensions of the image from the user
                ID = new ImageDimensions(iTotalSize);
                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    canvas.Width = width;
                    canvas.Height = height;
                    img.Width = width;
                    img.Height = height;
                    // pix64 = new double[iTotalSize];
                    m_CmpxTemp1 = new COMPLEX[iTotalSize];
                    m_CmpxTemp2 = new COMPLEX[iTotalSize];

                    temp = new double[iTotalSize];
                  //  tempFloat = new float[iTotalSize];
                    tempByte = new byte[iTotalSize];
                   //tempInt = new int[iTotalSize];

                    for (i = 0; i < iTotalSize; ++i)
                    {
                        m_CmpxTemp1[i].re = (double)br.ReadByte();//mri                                       
                        m_CmpxTemp1[i].im = 0;
                        // pix08[i] = CW.pix08Conv[i];

                     }
                        br.Close();

                    fftshift(m_CmpxTemp1, m_CmpxTemp2, height, width);
                    fft2D(m_CmpxTemp2, height, width, 1);
                    fft2D(m_CmpxTemp2, height, width, 0);

                    fftshift(m_CmpxTemp2, m_CmpxTemp1, height, width);


                    Magnitude(height, width, m_CmpxTemp1, temp);

                    //DoubleToFloat(iTotalSize, tempFloat, temp);
                    DoubleToByte(iTotalSize, tempByte, temp);
                    //DoubleToInt(iTotalSize, tempInt, temp);

                    int bitsPerPixel = 8;
                    stride = (width * bitsPerPixel + 7) / 8;

                    // Single step creation of the image
                    bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                        tempByte, stride);
                    img.Source = bmps;
                    bnSaveJPG.IsEnabled = true;
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

        public void Magnitude(int nHeight, int nWidth, COMPLEX[] m_CmpxTemp1, double[] temp)
        {

            for (int i = 0; i < nHeight; i++)
            {
                for (int j = 0; j < nWidth; j++)
                {
                    temp[i * nWidth + j] = Math.Sqrt( m_CmpxTemp1[i * nWidth + j].re * m_CmpxTemp1[i * nWidth + j].re + m_CmpxTemp1[i * nWidth + j].im * m_CmpxTemp1[i * nWidth + j].im );
                    //  temp[i * nWidth + j] = Math.Sqrt( temp[i * nWidth + j] );

                     // temp[i * nWidth + j] =  temp[i * nWidth + j] /7 ;

                }
            }
                     

        }


        public void DoubleToFloat(int number, float[] floatArray, double[] DoubleArray)
        {

            for (int i = 0; i < number; i++)
            {
                floatArray[i] = (float)DoubleArray[i];
            }


        }

        public void DoubleToByte(int number,byte [] ByteArray, double[] DoubleArray)
        {

            for (int i = 0; i < number; i++)
            {
                ByteArray[i] = (byte)DoubleArray[i];
            }


        }

        public void DoubleToInt(int number, int[] IntArray, double[] DoubleArray)
        {

            for (int i = 0; i < number; i++)
            {
                IntArray[i] = (int)DoubleArray[i];
            }


        }

        public void ByteToDouble(int number, byte[] ByteArray, double[] DoubleArray)
        {

            for (int i = 0; i < number; i++)
            {
                DoubleArray[i] = (double)ByteArray[i];
            }


        }

        public void ByteToInt(int number, byte[] ByteArray, int[] IntArray)
        {

            for (int i = 0; i < number; i++)
            {
                IntArray[i] = (int)ByteArray[i];
            }


        }

        public void fft1D(COMPLEX[] data, int NumData, int dir0)
        //	"dir=1" is Discrete Fourier Transform (Forward)
        //	"dir=0" is inverse Discrete Fourier Transform (Backward)
        {
            int log2NumData;
            int HalfNumData;
            int i, j, k, mm;
            int step;
            int ButterFly, ButterFlyHalf;
            double RealValue, ImValue;
            double AngleRadian;
            double CosineValue, SineValue;
            double ArcRe, ArcIm, ReBuf, ImBuf, ArcBuf;
            bool dir = true;

            if (dir0 == 1)
            {
                dir = true;
            }

            if (dir0 == 0)
            {
                dir = false;
            }



            log2NumData = 0;
            while (NumData != (1 << log2NumData))
                log2NumData++;
            HalfNumData = NumData >> 1;
            j = 1;
            for (i = 1; i < NumData; i++)
            {
                if (i < j)
                {
                    RealValue = data[j - 1].re;
                    ImValue = data[j - 1].im;
                    data[j - 1].re = data[i - 1].re;
                    data[j - 1].im = data[i - 1].im;
                    data[i - 1].re = RealValue;
                    data[i - 1].im = ImValue;
                }
                k = HalfNumData;
                while (k < j)
                {
                    j -= k;
                    k = k >> 1;
                }
                j += k;
            }
            //butterfly ¡Æ??¢´ ¨ù??? 
            for (step = 1; step <= log2NumData; step++)
            {
                ButterFly = 1 << step;
                ButterFlyHalf = ButterFly >> 1;
                ArcRe = 1;
                ArcIm = 0;
                AngleRadian = (double)(Math.PI / ButterFlyHalf);
                CosineValue = (float)Math.Cos(AngleRadian);
                SineValue = (float)Math.Sin(AngleRadian);
                if (dir) //Foward
                    SineValue = -SineValue;
                for (j = 1; j <= ButterFlyHalf; j++)
                {
                    i = j;
                    while (i <= NumData)
                    {
                        mm = i + ButterFlyHalf;
                        ReBuf = data[mm - 1].re * ArcRe - data[mm - 1].im * ArcIm;
                        ImBuf = data[mm - 1].re * ArcIm + data[mm - 1].im * ArcRe;
                        data[mm - 1].re = data[i - 1].re - ReBuf;
                        data[mm - 1].im = data[i - 1].im - ImBuf;
                        data[i - 1].re = data[i - 1].re + ReBuf;
                        data[i - 1].im = data[i - 1].im + ImBuf;
                        i += ButterFly;
                    }
                    ArcBuf = ArcRe;
                    ArcRe = ArcRe * CosineValue - ArcIm * SineValue;
                    ArcIm = ArcBuf * SineValue + ArcIm * CosineValue;
                }
            }
            if (!dir) //Inverse
            {
                for (j = 0; j < NumData; j++)
                {
                    data[j].re /= NumData;
                    data[j].im /= NumData;
                }
            }
        }




        public void fft2D(COMPLEX[] data, int height, int width, int Forward)
        {
            int r, c, m_where;
            int width_Half = 256;
            int width_Quarter = 128;

            COMPLEX[] pTwoData;
            COMPLEX[] pHData;
            COMPLEX[] pVData;
            COMPLEX[] pDeci;
            COMPLEX[] pRest;

            //?¢¯?? ¢¬¨­¢¬©£¢¬¢ç ??¢¥?

            pHData = new COMPLEX[width];
            pVData = new COMPLEX[height];
            pTwoData = new COMPLEX[height * width];
            pDeci = new COMPLEX[height * width_Half];
            pRest = new COMPLEX[height * width_Half];


            //¢¯©ª ??©ö???¡¤?¨¬??? FFT¢¯????? ?¡×?? ??¡¾?¡Æ¨£ ¨ù©ø?¢´
            for (r = 0; r < height; r++)
                for (c = 0; c < width; c++)
                {
                    m_where = r * width + c;
                    pTwoData[m_where].re = data[m_where].re;
                    pTwoData[m_where].im = data[m_where].im;
                }

            //¨ù?¨¡? ©ö©¡???¢¬¡¤? 1?¡À?? FFT ¨ù??? 
            for (r = 0; r < height; r++)
            {
                //?¨ª¢¥???¢¥? ???? ?????? ¨¬©ö?? 
                for (c = 0; c < width; c++)
                {
                    m_where = r * width + c;
                    pHData[c].re = pTwoData[m_where].re;
                    pHData[c].im = pTwoData[m_where].im;
                }
                //1?¡À?? FFT ¨ù??? 
                fft1D(pHData, width, Forward);
                //1?¡À?? FFT ¨ù??? ¡Æ?¡Æ? ????

                for (c = 0; c < width; c++)
                {
                    m_where = r * width + c;
                    pTwoData[m_where].re = pHData[c].re;
                    pTwoData[m_where].im = pHData[c].im;
                }
            }
            /*
            //Decimation
            for(r=0;r<height;r++)
            {
                int g=0;
                int f=0;

                for(c=0;c<512;c++,g++)
                {
                    if(g < 128 || g > 383)
                    {
                        pDeci[r*256+f].re=pTwoData[r*512+g].re;
                        pDeci[r*256+f].im=pTwoData[r*512+g].im;
                        f++;
                    }
                }
            }
            //Decimation Rest Value
            for(r=0;r<height;r++)
            {
                int g=0;
                int f=0;

                for(c=0;c<512;c++,g++)
                {
                    if(127 < g && g < 384)
                    {
                        pRest[r*256+f].re=pTwoData[r*512+g].re;
                        pRest[r*256+f].im=pTwoData[r*512+g].im;
                        f++;
                    }
                }
            }

            //Decimation ?? width*height(512x256)¢¬? width_Half*height(256x256)¡¤? ¨¬?¡Æ©¡
            width = width_Half;	*/

            //¨ù??¡À ©ö©¡???¢¬¡¤? 1?¡À?? FFT ¨ù??? 
            for (c = 0; c < width; c++)
            {
                //?¨ª¢¥???¢¥? ???? ?????? ¨¬©ö?? 
                for (r = 0; r < height; r++)
                {
                    m_where = r * width + c;
                    pVData[r].re = pTwoData[m_where].re;
                    pVData[r].im = pTwoData[m_where].im;
                }
                //1?¡À?? FFT ¨ù??? 
                fft1D(pVData, height, Forward);
                //1?¡À?? FFT ¨ù??? ¡Æ?¡Æ? ????
                for (r = 0; r < height; r++)
                {
                    m_where = r * width + c;
                    data[m_where].re = pVData[r].re;
                    data[m_where].im = pVData[r].im;
                }
            }

            //?¢¯?? ¢¬¨­¢¬©£¢¬¢ç ??¢¥? ?¨ª?? 

        }





        public void fftshift(COMPLEX[] indata, COMPLEX[] outdata, int height, int width)
        {
            int i, j, m_where;
            int HeightHalf = height / 2;
            int width_Half = width / 2;

            for (i = 0; i < height; i++)
            {
                for (j = 0; j < width; j++)
                {
                    m_where = i * width + j;
                    if (i < HeightHalf && j < width_Half)                                       //1??¨¬¨¢¢¬?
                    {
                        outdata[(i + HeightHalf) * width + (j + width_Half)].re = indata[m_where].re;
                        outdata[(i + HeightHalf) * width + (j + width_Half)].im = indata[m_where].im;
                    }
                    else if (HeightHalf <= i && i < height && j < width_Half)                   //4??¨¬¨¢¢¬?
                    {
                        outdata[(i - HeightHalf) * width + (j + width_Half)].re = indata[m_where].re;
                        outdata[(i - HeightHalf) * width + (j + width_Half)].im = indata[m_where].im;
                    }
                    else if (i < HeightHalf && width_Half <= j && j < width)                    //2??¨¬¨¢¢¬?
                    {
                        outdata[(i + HeightHalf) * width + (j - width_Half)].re = indata[m_where].re;
                        outdata[(i + HeightHalf) * width + (j - width_Half)].im = indata[m_where].im;
                    }
                    else if (HeightHalf <= i && i < height && width_Half <= j && j < width)     //3??¨¬¨¢¢¬?
                    {
                        outdata[(i - HeightHalf) * width + (j - width_Half)].re = indata[m_where].re;
                        outdata[(i - HeightHalf) * width + (j - width_Half)].im = indata[m_where].im;
                    }
                }
            }
        }










        private void bnSaveJPG_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "JPG Images (.jpg)|*.jpg";

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            string targetPath = "";

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                targetPath = dlg.FileName;
                FileStream fs = new FileStream(targetPath, FileMode.Create);
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmps));
                encoder.Save(fs);
                fs.Close();
            }
        }

        private void img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Width = img.ActualWidth;
            canvas.Height = img.ActualHeight;
        }

      

        private void bnOpen08_Click_Conv(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage08_Conv(fileName);
            }
        }

        private void DisplayImage08_Conv(string fileName)
        {
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.         
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                byte pixByte;
                int i;
                 iTotalSize = (int)br.BaseStream.Length;
               
                

                // Get the dimensions of the image from the user
                ID = new ImageDimensions(iTotalSize);
                if (ID.ShowDialog() == true)
                {
                   
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    canvas.Width = width;
                    canvas.Height = height;
                    img.Width = width;
                    img.Height = height;
                    pix08 = new byte[iTotalSize];
                    pix_conv = new double[width, height];
                    pix_convMultiplied = new double[width - 2, height - 2];
                    pix1d_conv = new byte[(width-2)*(height-2)];

                    for (i = 0; i < iTotalSize; ++i)
                    {
                        pixByte = (byte)(br.ReadByte());
                        pix08[i] = pixByte;
                    }
                    br.Close();

                    CW = new ConvolutionalWindow();
                    if (CW.ShowDialog() == true)
                    {
                        if (CW.number == 1)
                        {
                            filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, smoothing1);
                        }
                        else if (CW.number==2)
                        {
                            filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, highpass1);

                        }
                        else if (CW.number == 3)
                        {
                            filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, sobel1);

                        }
                        else if (CW.number == 4)
                        {
                            filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, sobel2);

                        }
                        else if (CW.number == 5)
                        {
                            filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, laplacian1);

                        }
                        else if (CW.number == 6)
                        {
                            filter = new Microsoft.ML.Probabilistic.Math.Matrix(3, 3, laplacian2);

                        }

                        convert_1Dto2D_conv(pix08, pix_conv, width, height);
                            CW.convolutionalMultiply(width, pix_conv, pix_convMultiplied, filter);
                       
                        if (CW.number != 1 && CW.number != 2)
                        {
                            for (int z = 0; z < width - 2; z++)
                            {
                                for (int j=0; j < height - 2; j++)
                                {
                                    pix_convMultiplied[z,j] = pix_convMultiplied[z,j] / 20;

                                }
                            }

                        }

                        convert_2Dto1D_conv(pix1d_conv, pix_convMultiplied, width - 2, height - 2);
                       
                        //if (CW.number!=1 || CW.number!=2)
                        //{
                        //    pix1d_conv = pix1d_conv / 20;
                        //}

                        int bitsPerPixel = 8;
                        stride = ((width-2) * bitsPerPixel + 7) / 8;

                        // Single step creation of the image
                        bmps = BitmapSource.Create(width-2, height-2, 96, 96, PixelFormats.Gray8, null,
                            pix1d_conv, stride);
                        img.Source = bmps;
                        bnSaveJPG.IsEnabled = true;

                    }
                    else
                    {
                        br.Close();
                    }
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

        private void bnOpen08_Click_NonConv(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Files(*.raw)|*.raw";

            Nullable<bool> result = ofd.ShowDialog();
            string fileName = "";

            if (result == true)
            {
                fileName = ofd.FileName;
                DisplayImage08_NonConv(fileName);
            }
        }

        private void DisplayImage08_NonConv(string fileName)
        {
            // Open a binary reader to read in the pixel data. 
            // We cannot use the usual image loading mechanisms since this is raw 
            // image data.         
            try
            {
                BinaryReader br = new BinaryReader(File.Open(fileName, FileMode.Open));
                byte pixByte;
                int i;
                iTotalSize = (int)br.BaseStream.Length;



                // Get the dimensions of the image from the user
                ID = new ImageDimensions(iTotalSize);
                if (ID.ShowDialog() == true)
                {
                    width = Convert.ToInt32(ID.tbWidth.Text);
                    height = Convert.ToInt32(ID.tbHeight.Text);
                    canvas.Width = width;
                    canvas.Height = height;
                    img.Width = 2*width;
                    img.Height = 2*height;
                    pix08 = new byte[iTotalSize];
                    pix08_subsampling = new byte[iTotalSize/4];

                    pix08_2d = new byte[width, height];
                    pix08_2d_subsampling = new byte[width/2, height/2];

                    pix08_4times = new byte[iTotalSize*4];

                    for (i = 0; i < iTotalSize; ++i)
                    {
                        pixByte = (byte)(br.ReadByte());
                        pix08[i] = pixByte;
                    }
                    br.Close();

                    // writeInverse(iTotalSize, pix08);
                    nonConvWindow = new NonConvWindow();
                    if (nonConvWindow.ShowDialog() == true)
                    {
                        // Inverse(pix08);
                        if (nonConvWindow.number == 1)
                        {//inversion

                            nonConvWindow.Inverse(pix08);
                            int bitsPerPixel = 8;
                            stride = (width * bitsPerPixel + 7) / 8;

                            // Single step creation of the image
                            bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                                pix08, stride);
                            img.Source = bmps;
                            bnSaveJPG.IsEnabled = true;

                        }
                        else if(nonConvWindow.number==2)
                        {//upsampling
                            byte[,] increasedMatrix = new byte[2 * width, 2 * height];
                            convert_1Dto2D(pix08, pix08_2d, width, height);
                            nonConvWindow.upsampling(width, pix08_2d, increasedMatrix, 2);
                            convert_2Dto1D(pix08_4times, increasedMatrix, 2*width, 2*height);
                           
                            stride = (2*width * 8 + 7) / 8;

                            // Single step creation of the image
                            bmps = BitmapSource.Create(2*width, 2*height, 96, 96, PixelFormats.Gray8, null,
                                pix08_4times, stride);
                            img.Source = bmps;
                            bnSaveJPG.IsEnabled = true;
                            
                        }

                        else if(nonConvWindow.number==3)
                        {
                            convert_1Dto2D(pix08, pix08_2d, width, height);
                            nonConvWindow.Subsampling(width, 2, pix08_2d, pix08_2d_subsampling);
                            convert_2Dto1D( pix08_subsampling ,pix08_2d_subsampling, width/2, height/2);
                            width = width / 2;
                            height = height / 2;

                            int bitsPerPixel = 8;
                            stride = (width * bitsPerPixel + 7) / 8;

                            // Single step creation of the image
                            bmps = BitmapSource.Create(width, height, 96, 96, PixelFormats.Gray8, null,
                                pix08_subsampling, stride);
                            img.Source = bmps;
                            bnSaveJPG.IsEnabled = true;

                        }

                        else
                        {
                            byte[,] BackgroundArray = new byte[2 * width, 2 * height];
                            convert_1Dto2D(pix08, pix08_2d, width, height);
                            nonConvWindow.rotate(width, Math.PI / 3, pix08_2d, BackgroundArray);
                            convert_2Dto1D(pix08_4times, BackgroundArray, 2 * width, 2 * height);

                            stride = (2 * width * 8 + 7) / 8;

                            // Single step creation of the image
                            bmps = BitmapSource.Create(2 * width, 2 * height, 96, 96, PixelFormats.Gray8, null,
                                pix08_4times, stride);
                            img.Source = bmps;
                            bnSaveJPG.IsEnabled = true;
                        }
                    }
                    else
                    {
                        br.Close();
                    }
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

        private void convert_1Dto2D(byte[] arr1d, byte[,] arr2d, int width, int height)
        {
            for (int i =0; i <width; i++)
            {
                for (int j=0; j<height;j++)
                {
                    arr2d[i, j] = arr1d[j + i * width];
                }
            }
        }

        private void convert_1Dto2D_conv(byte[] arr1d, double[,] arr2d, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    arr2d[i, j] = (double)arr1d[j + i * width];
                }
            }
        }


        private void convert_2Dto1D(byte[] arr1d, byte[,] arr2d, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    arr1d[j + i * width] = arr2d[i, j];
                }
            }
        }

        private void convert_2Dto1D_conv(byte[] arr1d, double[,] arr2d, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    arr1d[j + i * width] = (byte)arr2d[i, j];
                }
            }
        }


       



    }
}
