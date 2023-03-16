using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;

namespace DicomViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DcmFileManager _dcm_file_mgr =  new DcmFileManager();
        private Point _ptDown;
        public MainWindow()
        {
            InitializeComponent();
            //_dcm_file_mgr = new DcmFileManager();
            bnSaveJPG.IsEnabled = true;

        }


        string FilePath;

        //save jpg 
        BitmapSource bmps;



        private void btnNew_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.Hide();

            Thread.Sleep(500);
            //
            // Capture Screen ---------------------------------------------------------
            CaptureScreenWindow CS = new CaptureScreenWindow();
            CS.SetImageSource = ScreenCapture.GetImageStream(new ScreenCapture().CaptureScreen());

            CS.Captured += (s, ea) =>
            {
                Image image = new Image();
                image.Source = (ImageSource)s;
                image.Stretch = Stretch.Uniform;
                image.Width = 350;
                image.Height = 200;

                lstImages.Items.Add(image);
            };

            CS.ShowDialog();
            // ---------------------------------------------------------
            //this.Show();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<BitmapSource> images = new List<BitmapSource>();
            foreach (var item in lstImages.Items)
                images.Add((BitmapSource)(item as Image).Source);

            System.Drawing.Bitmap bmp = MargeImages.Combine(images.ToArray());

            //
            // Open Save Dialog
            //
            System.Windows.Forms.SaveFileDialog SFD = new System.Windows.Forms.SaveFileDialog();
            SFD.Title = "Select Save Path";
            SFD.DefaultExt = ".png";
            SFD.FileName = "ScreenSnips.png";
            if (SFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                bmp.Save(SFD.FileName);
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

        //Dicom.Imaging.Render.IPixelData a = new Dicom.Imaging.Render.IPixelData();

        private void Measure()
        {
            FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            //_dcm_file_mgr.ImgWidth;
            //int width = 420*2; // byte
            int width = _dcm_file_mgr.ImgWidth; //ushort
            int height = _dcm_file_mgr.ImgHeight;
            //ushort[,] ImageArray = new ushort[width, height];
            // byte[,] ImageArray = new byte[width, height];
            double[,] ImageArray = new double[width, height];
            double x,y;
            double centerX, centerY;
            double EllipseWidth, EllipseHeight;
            int k, z;
            double sum=0;
            double count = 0;
            int StdCount = 0;
            double StdSum = 0;
            double Std;
            double Mean;
            double Max = 0;
            double Min = 65535;
            double[] StdArray = new double[width * height];
            
            centerX = Math.Abs(currentPoint1.X + currentPoint2.X) / 2;
            centerY = Math.Abs(currentPoint1.Y + currentPoint2.Y) / 2;
            EllipseWidth = Math.Abs(currentPoint1.X - currentPoint2.X);
            EllipseHeight = Math.Abs(currentPoint1.Y - currentPoint2.Y);

            //byte data
            //byte[] originalRawBytes = _dcm_file_mgr._dcm_image.PixelData.GetFrame(0).Data;

            double[,] MaskArray = new double[width, height];

            //ushort data
            Dicom.Imaging.BitDepth bitDepth = _dcm_file_mgr._dcm_image.PixelData.BitDepth;
            Dicom.IO.Buffer.IByteBuffer byteBuffer = _dcm_file_mgr._dcm_image.PixelData.GetFrame(0);
            Dicom.Imaging.Render.GrayscalePixelDataU16 DicomPixel = new Dicom.Imaging.Render.GrayscalePixelDataU16(width, height, bitDepth, byteBuffer);


            


            //remove header data
            //for (int q = 0; q < 193330 / 2; q++)
            //{
            //    ImageArray[0, 0] = br.ReadUInt16();
            //}


            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    //ImageArray[i, j] = br.ReadUInt16();
                   // ImageArray[i, j] = br.ReadByte();

                   // x = (double)i;
                  //  y = (double)j;

                 
                    if (  Math.Pow( i-centerX , 2) / Math.Pow(EllipseWidth/2 , 2) + Math.Pow( j - centerY , 2) / Math.Pow( EllipseHeight/2 , 2) < 1)
                    {
                        count += 1;
                       // k = (int)x;
                       // z = (int)y;
                        sum += DicomPixel.GetPixel(i, j);

                        if(DicomPixel.GetPixel(i, j) > Max)
                        {
                            Max = DicomPixel.GetPixel(i, j);
                        }

                        if (Min > DicomPixel.GetPixel(i, j))
                        {
                            Min = DicomPixel.GetPixel(i, j);
                        }


                        StdArray[StdCount] = DicomPixel.GetPixel(i, j);
                        StdCount += 1;

                        MaskArray[i, j] = 1;


                    }
                }
            }

            Mean = sum / count;

            for(int t = 0; t < StdCount; t++)
            {
               StdSum+= Math.Pow(StdArray[t] - Mean, 2);
            }

            Std = Math.Sqrt(StdSum / StdCount);

            x1y1.Content = string.Format("x1 = {0} , y1 = {1}", currentPoint1.X ,currentPoint1.Y);
            x2y2.Content = string.Format("x2 = {0} , y2 = {1}", currentPoint2.X, currentPoint2.Y);
            numberofpixel.Content = string.Format("NumberOfPixel={0}",count);

            MeanLabel.Content = string.Format("Mean : {0:F2}", Mean);
            MaxLabel.Content = string.Format("Max : {0}", Max);
            MinLabel.Content = string.Format("Min : {0}", Min);
            StdLabel.Content = string.Format("Std : {0:F2}", Std);

            //ROImeasurment roi = new ROImeasurment();
            //roi.MinLabel.Content = string.Format("Min : {0}", Min);
            //if (roi.ShowDialog() == true)
            //{
            //    roi.MinLabel.Content = string.Format("Min : {0}", Min);

            //}
            //roi.MinLabel.Content = string.Format("Min : {0}", Min);
            br.Close();
            fs.Close();


            double[] MaskArray1D = new double[width * height];
            double[] MaskArray1D_ReArranged = new double[width * height];

            //convert mask 2D to 1D
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // MaskArray1D[i * height + j] = MaskArray[i, j];
                    MaskArray1D[i * height + j] = MaskArray[i, j];

                }

            }

            //rearrange mask
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // MaskArray1D[i * height + j] = MaskArray[i, j];
                    MaskArray1D_ReArranged[j * width + i] = MaskArray1D[i * height + j];
                }

            }

            //420,512
            

    //        //make roi mask raw file
    //        FileStream fs1 =
    //          new FileStream("C:/mask.raw", FileMode.Create, FileAccess.Write);
    //        BinaryWriter bw = new BinaryWriter(fs1);

    //        int b = 250;

    //        for (int i = 0; i < width * height; i++)
    //        {

    //                bw.Write( 250 * MaskArray1D_ReArranged[i]);

    //        }

    //        bw.Close();
    //        fs1.Close();


    //        //make dicom image raw file
    //        ushort[] ImageArray1D = DicomPixel.Data;
    //        ushort a1 = 20;
    //        double[] ImageArrayDouble = new double[width * height];

    //        for (int i = 0; i < height * width; i++)
    //        {
    //            ImageArrayDouble[i] = ImageArray1D[i] / a1;
    //        }

    //        FileStream fs2 =
    //         new FileStream("C:/Users/dicom.raw", FileMode.Create, FileAccess.Write);
    //        BinaryWriter bw2 = new BinaryWriter(fs2);

    //        for (int i = 0; i < height * width; i++)
    //        {
    //            bw2.Write((ushort)ImageArrayDouble[i]);
    //        }

    //        bw2.Close();
    //        fs2.Close();





    //        //make overlap roi dicom raw file
    //        ushort[] DicomArray = DicomPixel.Data;
    //        //ushort a1 = 20;
    //        double[] NewDicomArray = new double[width * height];

    //        for (int i = 0; i < width * height ; i++)
    //        {
    //                NewDicomArray[i] = MaskArray1D_ReArranged[i] * DicomArray[i];
    //                //NewDicomArray[i * height + j] = MaskArray[i,j] * DicomArray[i * height + j];


    //        }

          

    //        FileStream fs3 =
    //         new FileStream("C:/Users/overlappedDicom.raw", FileMode.Create, FileAccess.Write);
    //        BinaryWriter bw3 = new BinaryWriter(fs3);

    //        for (int i = 0; i < height * width; i++)
    //        {
    //            bw3.Write(NewDicomArray[i]);
    //        }

    //        bw3.Close();
    //        fs3.Close();

    //        //binary writer
    //        /*
    //        FileStream fs4 =
    //new FileStream("D:/test.txt", FileMode.Create, FileAccess.Write);
    //        BinaryWriter bw4 = new BinaryWriter(fs4);


    //        string mean = Mean.ToString();
    //        string std = Std.ToString();

    //        bw4.Write("Mean :" + mean + Environment.NewLine);
    //        bw4.Write(std);
            

    //        bw4.Close();
    //        fs4.Close();
    //        */


    //        //streamwriter
    //        FileStream fs5 =
    //new FileStream("C:/Users/RoiResult.csv", FileMode.Append, FileAccess.Write);
    //        StreamWriter sw5 = new StreamWriter(fs5);


    //        //string mean = Mean.ToString();
    //        //string std = Std.ToString();


    //        sw5.WriteLine(Max+ "," + Min +","+Mean+ "," +Std);

    //        sw5.Close();
    //        fs5.Close();






        }




        private void btn_ROImeasurement(object sender, RoutedEventArgs e)
        {
            ROImeasurment roi = new ROImeasurment();
            if (roi.ShowDialog() == true)
            {

            }
        }



        //event method switching
        private void btn_ROI(object sender,RoutedEventArgs e)
        {
            MouseLeftButtonDown -= Img_LeftButtonDown;
            MouseMove -= Img_MouseMove;
            MouseLeftButtonUp -= Img_LeftButtonUp;

            MouseLeftButtonUp += Canvas_MouseUp_1;
            MouseLeftButtonDown += Canvas_MouseDown_1;
        }

        private void btn_WindowLevel(object sender, RoutedEventArgs e)
        {
            MouseLeftButtonUp -= Canvas_MouseUp_1;
            MouseLeftButtonDown -= Canvas_MouseDown_1;

            MouseLeftButtonDown += Img_LeftButtonDown;
            MouseMove += Img_MouseMove;
            MouseLeftButtonUp += Img_LeftButtonUp;
        }


        //ellipse
        Point currentPoint1 = new Point();
        Point currentPoint2 = new Point();


        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            // if (e.ButtonState == MouseButtonState.Pressed)
            currentPoint1 = e.GetPosition(canvas);

        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            currentPoint2 = e.GetPosition(canvas);

            Draw();
        }

        private void Draw()
        {
            Ellipse myEllipse = new Ellipse();


            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.Yellow;

            // Set the width and height of the Ellipse.
            myEllipse.Width = Math.Abs(currentPoint1.X - currentPoint2.X);
            myEllipse.Height = Math.Abs(currentPoint1.Y - currentPoint2.Y);
            myEllipse.Margin = new Thickness(currentPoint1.X, currentPoint1.Y, 0, 0);

            /*
            //test
            myEllipse.Width = Math.Abs(50);
            myEllipse.Height = Math.Abs(5);
            myEllipse.Margin = new Thickness(100, 100, 0, 0);
            */

            // Add the Ellipse to the StackPanel.
            canvas.Children.Add(myEllipse);

            Measure();
        }



        //zooming,pan
        Point initial;

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var element = sender as UIElement;
            var position = e.GetPosition(element);
            var transform = element.RenderTransform as MatrixTransform;
            var matrix = transform.Matrix;
            var scale = e.Delta >= 0 ? 1.1 : (1.0 / 1.1); // choose appropriate scaling factor
            matrix.ScaleAtPrepend(scale, scale, position.X, position.Y);
            transform.Matrix = matrix;
        }


        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.RightButton == MouseButtonState.Pressed)
            {
                var element = sender as UIElement;
                var transform = element.RenderTransform as MatrixTransform;
                var matrix = transform.Matrix;
                var position = e.GetPosition(element);
                var xOffset = e.GetPosition(this).X - initial.X;
                var yOffset = e.GetPosition(this).Y - initial.Y;
                matrix.Translate(xOffset, yOffset);
                transform.Matrix = matrix;

            }
            initial = Mouse.GetPosition(this);


        }




        //dicom display

        private void Btn_OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "Open Dicom";
            dialog.Filter = "Dicom File(*.dcm; *.dic;)|*.dcm;*.dic;|AllFiles(*.*)|*.*";

            //dialog.Filter = "所有文件(*.*)|*.*";
            dialog.InitialDirectory = "C:\\";
            string dcmFileName = "";
            if (dialog.ShowDialog() == true)
            {
                dcmFileName = dialog.FileName;
                FilePath = dialog.FileName;
            }
            _dcm_file_mgr.OpenDcmFile(dcmFileName);
            Update_Image();
        }

        private void Update_Image(double offsetX = 0, double offsetY = 0)
        {
            img.Source = _dcm_file_mgr.RendImage(offsetX, offsetY); //rendering 
            bmps = _dcm_file_mgr.RendImage(offsetX, offsetY);
        }






        //header display
        HeaderDisplay headerdisplay;
        private void Btn_HeaderDisplay(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog dialog = new OpenFileDialog();
            headerdisplay = new HeaderDisplay();

            if( headerdisplay.ShowDialog()==true)
            {

            }

        }





        //window level
        private void Img_LeftButtonDown(object sender, MouseEventArgs e)
        {
            _ptDown = e.GetPosition(img);
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point ptCurrent = e.GetPosition(img);
                Update_Image(ptCurrent.X - _ptDown.X, ptCurrent.Y - _ptDown.Y);
            }
        }

        private void Img_LeftButtonUp(object sender, MouseEventArgs e)
        {
            Point ptCurrent = e.GetPosition(img);
            _dcm_file_mgr.WinWidth += ptCurrent.X - _ptDown.X;
            _dcm_file_mgr.WinCenter += ptCurrent.Y - _ptDown.Y;
            Update_Image();
        }
        




    }
}
