using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace RecognitionOfPassports
{
    public class Form1
    {
        Bitmap  image, tempImage;
        int height, width, kHW = 5;
        MorphologicalFilter filter = new MorphologicalFilter();
        string resultPath;

        public Bitmap открытьToolStripMenuItem_Click(string path, string resultPath)
        {
            this.resultPath = resultPath;

            {
                image = new Bitmap(path);
                height = image.Size.Height / kHW;
                width = image.Size.Width / kHW;
                tempImage = new Bitmap(image, width, height);
                // pbLeft.Image = (Bitmap)image.Clone();
                //create strips
               
                var alpha = Rotate90(tempImage);
                if (alpha == 90)
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    var temp = height;
                    height = width;
                    width = temp;
                }
                else if (alpha == 270)
                {
                    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    var temp = height;
                    height = width;
                    width = temp;
                }
                else
                {
                    Deskew d = new Deskew();
                    var angle = d.DeskewImage(tempImage);
                    image = Rotator.Rotate(image, angle);
                }

             //   try
                {
                    return Start();
                }
              //  catch (Exception ex) { MessageBox.Show(ex.ToString(), "Нежданчик"); }
            }
        }
        private int Rotate90(Bitmap temp)
        {
            temp = new Bitmap(temp, width, height);
            //Sobel
            temp = Filters.ConvolutionFilter(temp, Filters.Sobel3x3Horizontal, Filters.Sobel3x3Vertical, 1.0, 0);
            //Inverse
            temp = Filters.InverseBitmap(temp);
            //Otsu
            temp = Filters.OtsuFilter(temp);
            MorphologicalFilter filter = new MorphologicalFilter();
            temp = filter.ApplyFilter(temp, 3, 2);
            temp = filter.ApplyFilter(temp, 4, 3);
            BitmapData bmpData = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height),
                   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmpData.Stride * temp.Height;
            byte[] rgbValues = new byte[numBytes];
            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            int[] hist = new int[temp.Width];
            hist.Initialize();
            for (int i = 6; i < temp.Width - 6; i++)
            {
                for (int j = 6; j < temp.Height - 6; j++)
                {
                    int index = bmpData.Stride * j + i * 3;
                    hist[i] = rgbValues[index] == 0 ? hist[i] += 1 : hist[i] += 0;
                    if (hist[i] == temp.Width)
                    {
                        hist[i] = 0;
                    }
                }
                
            }
            int sum = 0;
            for (int i = 0; i < temp.Width; i++)
            {
                sum += hist[i];
            }
            sum /= temp.Width;
            temp.UnlockBits(bmpData);
            int start = 0, end = 0;
            for (int i = 0; i < temp.Width - 1; i++)
            {
                if (hist[i] - sum > 0 && end == 0 && start == 0)
                {
                    start = i;
                }
                if (hist[i] - sum > 0 && start != 0 && hist[i + 1] == 0)
                {
                    end = i;
                }
            }
            if(start != 0 || end != 0)
            for (int i = start; i < end; i++)
            {
                if (hist[i] == 0)
                {
                    int max = 0, indexMax = 0;
                    for (int j = start; j < end; j++)
                    {
                        if (max < hist[j])
                        {
                            max = hist[j];
                            indexMax = j;
                        }
                    }
                        if (Math.Abs(indexMax - end) >= Math.Abs(indexMax - start))
                        {
                        return 270;
                          
                        }
                        else
                        {
                        return 90;
                        }
                }
            }
           
            return 0;
    }

        Bitmap Start()
        {
            var rect = DetectionText(image);
            image = image.Clone(rect, image.PixelFormat);
            List<Tuple<int, int>> rows = SearchStrings(image);
            Bitmap filteredImage = new Bitmap(image);
            filteredImage = Filters.OtsuFilter(filteredImage);
            Perceptron pc = new Perceptron();
            pc.FromFile();
            TextRecognition tc = new TextRecognition();
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(resultPath))
            {
                foreach (var row in rows)
                {
                    string str = "";
                    List<Tuple<Point, Point>> symbols = DrawSymbolEdge(filteredImage, image, new List<Tuple<int, int>>() { row });
                    foreach (var symbol in symbols)
                    {
                        
                        Bitmap newBmp = ImageRecizer.Cut(filteredImage, new Rectangle(symbol.Item1.X, symbol.Item1.Y, (symbol.Item2.X - symbol.Item1.X), (symbol.Item2.Y - symbol.Item1.Y)));
                        Bitmap bmp = ImageRecizer.Resize(newBmp, new Size(TextRecognition.PERCEPTRON_IMAGE_WIDTH, TextRecognition.PERCEPTRON_IMAGE_HEIGHT));
                        double[] dd = tc.GetPerceptronInputFromImage(bmp);
                        double[] temp = pc.Classify(dd);
                        int max = temp.ToList().IndexOf(temp.Max());
                        str += (TextRecognition.RECOGNITION_SYMBOLS[max] + " ");
                    }
                    file.WriteLine(str.ToLower());
                }
            }
            
            /*TextRecognition tc = new TextRecognition();
            tc.TeachSamples();*/


            return image;
            //pictureBoxImage.Image = image;   
        }


        public List<Tuple<int, int>> SearchStrings(Bitmap Image)
        {
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();
            Deskew d = new Deskew();
            var angle = d.DeskewImage(Image);
            Image = Rotator.Rotate(image, angle);
            //tempImage = new Bitmap(image, width, height);
            //Sobel
            tempImage = Filters.ConvolutionFilter(Image, Filters.Sobel3x3Horizontal, Filters.Sobel3x3Vertical, 1.0, 0);
            //Inverse
            tempImage = Filters.InverseBitmap(tempImage);
            tempImage = Filters.OtsuFilter(tempImage);
            tempImage = filter.ApplyFilter(tempImage, 3, 1);
            tempImage = filter.ApplyFilter(tempImage, 4, 3);

            BitmapData bmpData = tempImage.LockBits(new Rectangle(0, 0, tempImage.Width, tempImage.Height),
                  ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmpData.Stride * tempImage.Height;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            int[] hist = new int[tempImage.Height];
            hist.Initialize();
            int maxIndex = 0;
            int max = 0;
            for (int i = 0; i < tempImage.Height; i++)
            {
                for (int j = 0; j < tempImage.Width; j++)
                {
                    int index = bmpData.Stride * i + j * 3;
                    hist[i] = rgbValues[index] == 0 ? hist[i] += 1 : hist[i] += 0;
                    if (hist[i] > max && i > 20 && i < tempImage.Height - 20)
                    {
                        max = hist[i];
                        maxIndex = i;
                    }
                }
                if (tempImage.Width / 16 > hist[i])
                {
                    hist[i] = 0;
                }
            }
            tempImage.UnlockBits(bmpData);

            //Graphics graphics = Graphics.FromImage(image);
            for (int i = 0; i < tempImage.Height - 3; i++)
            {
                int startPoint = 0, endPoint = 0;
                if (hist[i] == 0 && hist[i + 1] != 0 && i > 2)
                {
                    startPoint = i - 2;
                    // graphics.DrawLine(new Pen(Brushes.Red, 1), 0, i -= 2, tempImage.Width, i);
                    //i += 2;
                }
                while (hist[i + 1] != 0)
                {
                    if (i + 1 == tempImage.Height - 4)
                        hist[i + 2] = 0;
                    i++;
                }
                endPoint = i + 4;
                // graphics.DrawLine(new Pen(Brushes.Green, 1), 0, i += 2, tempImage.Width, i);
                // i -= 2;

                if (endPoint != 0 && startPoint != 0 && endPoint - startPoint > 10)
                    result.Add(new Tuple<int,int>(startPoint,endPoint));
                //  graphics.DrawLine(new Pen(Brushes.Red, 1), 0, end, tempImage.Width, end);
            }
          /*  foreach (var str in tempPointStr)
            {
                graphics.DrawLine(new Pen(Brushes.Red, 1), 0, str.X, tempImage.Width, str.X);
                graphics.DrawLine(new Pen(Brushes.Green, 1), 0, str.Y, tempImage.Width, str.Y);
            }*/
            return result;
        }

        private List<Tuple<Point, Point>> DrawSymbolEdge(Bitmap filteredImage, Bitmap startImage, List<Tuple<int, int>> rows)
        {
            PixelFormat pixelFormat = (Image.GetPixelFormatSize(filteredImage.PixelFormat) / 8 > 2) ? filteredImage.PixelFormat : PixelFormat.Format24bppRgb;
            int nBytesPerPixel = Image.GetPixelFormatSize(pixelFormat) / 8;
            BitmapData bmpData = filteredImage.LockBits(new Rectangle(0, 0, filteredImage.Width, filteredImage.Height),
                   ImageLockMode.ReadWrite, pixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmpData.Stride * filteredImage.Height;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            filteredImage.UnlockBits(bmpData);
            Graphics graphics = Graphics.FromImage(image);
            List<Tuple<Point, Point>> symbols = new List<Tuple<Point, Point>>();
            foreach (var row in rows)
            {
                symbols.AddRange(ParseRow(rgbValues, graphics, new Point(0, row.Item1), new Point(filteredImage.Width, row.Item2), bmpData.Stride, nBytesPerPixel));
            }
            return symbols;
            
        }

        private List<Tuple<Point, Point>> ParseRow(byte[] rgbValues, Graphics graphics, Point startPoint, Point endPoint, int bmpStride, int nBytesPerPixel)
        {
            graphics.DrawLine(new Pen(Brushes.Red, 1), 0, startPoint.Y, image.Width, startPoint.Y);
            graphics.DrawLine(new Pen(Brushes.Red, 1), 0, endPoint.Y, image.Width, endPoint.Y);
            List<Tuple<Point, Point>> result = new List<Tuple<Point, Point>>();
            List<int> blackCounter = new List<int>();
            int height = endPoint.Y - startPoint.Y;
            int minValue = height / 7;
            for (int x = startPoint.X; x < endPoint.X; ++x)
            {
                int currCounter = 0;
                for (int y = startPoint.Y; y < endPoint.Y; ++y)
                {
                    int bytePos = bmpStride * y + x * nBytesPerPixel;
                    if (rgbValues[bytePos] == 0)
                        ++currCounter;
                }
                if (currCounter < minValue)
                    currCounter = 0;
                blackCounter.Add(currCounter);
            }
            int colNumber = 0;
            while (colNumber < blackCounter.Count)
            {
                if (blackCounter[colNumber] != 0)
                {
                    int startCol = colNumber;
                    while (colNumber < blackCounter.Count && blackCounter[colNumber] != 0)
                    {
                        while (colNumber < blackCounter.Count && blackCounter[colNumber] != 0)
                            ++colNumber;
                        while (colNumber - startCol < height / 2.15)
                            ++colNumber;
                    }
                    graphics.DrawLine(new Pen(Brushes.Red, 1), new Point(startPoint.X + startCol, startPoint.Y), new Point(startPoint.X + startCol, endPoint.Y));
                    graphics.DrawLine(new Pen(Brushes.Red, 1), new Point(startPoint.X + colNumber, startPoint.Y), new Point(startPoint.X + colNumber, endPoint.Y));
                    result.Add(new Tuple<Point, Point>(new Point(startPoint.X + startCol, startPoint.Y), new Point(startPoint.X + colNumber, endPoint.Y)));

                }
                ++colNumber;
            }
            return result;
        }

        public Rectangle DetectionText(Bitmap tempI)
        {
            var temp = new Bitmap(tempI, width, height);
            //Sobel
            temp = Filters.ConvolutionFilter(temp, Filters.Sobel3x3Horizontal, Filters.Sobel3x3Vertical, 1.0, 0);
            //Inverse
            temp = Filters.InverseBitmap(temp);
            //Otsu
            temp = Filters.OtsuFilter(temp);
            temp = filter.ApplyFilter(temp, 3, 2);
            temp = filter.ApplyFilter(temp, 4, 3);
            BitmapData bmpData = temp.LockBits(new Rectangle(0, 0, temp.Width, temp.Height),
                   ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmpData.Stride * temp.Height;
            byte[] rgbValues = new byte[numBytes];
            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            int[] hist = new int[temp.Height];
            hist.Initialize();
            for (int i = 6; i < temp.Height - 6; i++)
            {
                for (int j = 6; j < temp.Width - 6; j++)
                {
                    int index = bmpData.Stride * i + j * 3;
                    hist[i] = rgbValues[index] == 0 ? hist[i] += 1 : hist[i] += 0;
                    if (hist[i] == temp.Width)
                    {
                        hist[i] = 0;
                    }
                }
            }
            int sum = 0;
            for (int i = 0; i < temp.Height; i++)
            {
                sum += hist[i];
            }
            sum /= temp.Height;
            int start = 0, end = 0;
            for (int i = temp.Height-1; i > 0; i--)
            {
                if (hist[i] - sum > 0)
                {
                    end = i;
                    break;
                }
            }
            int x = 0;
            for (int i = end; i > 0; i--)
            {
                if (hist[i] - sum > 0 && hist[i - 1] - sum <= 0)
                {
                    start = i;
                    if (++x == 3)
                        break;
                }
            }
            ////////////////////////
            hist = new int[temp.Width];
            hist.Initialize();
            for (int i = 6; i < temp.Width - 6; i++)
            {
                for (int j = 6; j < temp.Height - 6; j++)
                {
                    int index = bmpData.Stride * j + i * 3;
                    hist[i] = rgbValues[index] == 0 ? hist[i] += 1 : hist[i] += 0;
                    if (hist[i] == temp.Height)
                    {
                        hist[i] = 0;
                    }
                }
            }
            sum = 0;
            for (int i = 0; i < temp.Width; i++)
            {
                sum += hist[i];
            }
            sum /= temp.Width;
            int startW = 0, endW = 0;
            for (int i = 0 ; i < temp.Width; i++)
            {
                if (hist[i] > 0 && hist[i+10] > 0)
                {
                    startW = i;
                    break;
                }
            }
            for (int i = start; i < temp.Width; i++)
            {
                if (hist[i] > 0 && hist[i + 1] <= 0)
                {
                    endW = i;
                        break;
                }
            }
            temp.UnlockBits(bmpData);
            startW *= kHW;
            start *= kHW;
            endW *= kHW;
            end *= kHW;
            endW = endW - startW + 20 < tempI.Width ? endW - startW + 20 : endW - startW;
            end = end - start + 5 < tempI.Height ? end - start + 5 : end - start;
            return new Rectangle(startW, start , endW, end);
        }
        private void DrawRectangle(Rectangle rectangle, Bitmap tempImage)
        {
            Graphics graphics = Graphics.FromImage(tempImage);
            graphics.DrawRectangle(new Pen(Brushes.Red, 1), rectangle);
            //pictureBoxImage.Image = tempImage;

        }

    }
}
