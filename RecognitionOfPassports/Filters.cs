using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace RecognitionOfPassports
{
    public static class Filters
    {
        private static float Px(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;
            for (i = init; i <= end; i++)
                sum += hist[i];

            return (float)sum;
        }
        private static float Mx(int init, int end, int[] hist)
        {
            int sum = 0;
            int i;
            for (i = init; i <= end; i++)
                sum += i * hist[i];

            return (float)sum;
        }
        public static Bitmap OtsuFilter(Bitmap bmp)
        {
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[numBytes];
            int height = 0, width = 0;
            int tempH = 0, tempW = 0;
            int startH = 0, startW = 0;
            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);
            for (int counter = 0; counter < rgbValues.Length - 2; counter += 3)
            {
                rgbValues[counter] = rgbValues[counter + 1] = rgbValues[counter + 2] = (byte)(0.2125 * rgbValues[counter] + 0.7154 * rgbValues[counter + 1] + 0.0721 * rgbValues[counter + 2]);
            }
            for (int cicleH = 0; cicleH <= 3; cicleH++)
            {
                switch (cicleH)
                {
                    case 0:
                        tempH = height = (int)bmp.Height / 3;
                        startH = 0;
                        break;
                    case 1:
                        startH = height;
                        height = tempH * 2;
                        break;
                    case 2:
                        startH = height;
                        height = tempH * 3;
                        break;
                    case 3:
                        startH = height;
                        height = bmp.Height;
                        break;
                    case 4:
                        startH = height;
                        height = tempH * 5;
                        break;
                    case 5:
                        startH = height;
                        height = bmp.Height;
                        break;
                    case 6:
                        startH = height;
                        height = tempH * 7;
                        break;
                    case 7:
                        startH = height;
                        height = tempH * 8;
                        break;
                    case 8:
                        startH = height;
                        height = tempH * 9;
                        break;
                    case 9:
                        startH = height;
                        height = tempH * 10;
                        break;
                    case 10:
                        startH = height;
                        height = bmp.Height;
                        break;
                }
                for (int cicleW = 0; cicleW <= 0; cicleW++)
                {
                    switch (cicleW)
                    {
                        case 0:
                            tempW = width = (int)bmp.Width;
                            startW = 0;
                            break;
                        case 1:
                            startW = width;
                            width = bmp.Width;
                            break;
                        case 2:
                            startW = width;
                            width = bmp.Width;
                            break;
                        case 3:
                            startW = width;
                            width = bmp.Width;
                            break;
                        case 4:
                            startW = width;
                            width = tempW * 5;
                            break;
                        case 5:
                            startW = width;
                            width = bmp.Width;
                            break;
                        case 6:
                            startW = width;
                            width = tempW * 7;
                            break;
                        case 7:
                            startW = width;
                            width = tempW * 8;
                            break;
                        case 8:
                            startW = width;
                            width = tempW * 9;
                            break;
                        case 9:
                            startW = width;
                            width = tempW * 10;
                            break;
                        case 10:
                            startW = width;
                            width = bmp.Width;
                            break;
                    }
                    //gistogramma
                    int[] hist = new int[256];
                    hist.Initialize();
                    for (int i = startH; i < height; i++)
                    {
                        int index = bmpData.Stride * i + startW * 3;
                        for (int j = startW; j < width; j++)
                        {
                            hist[rgbValues[index]]++;
                            index += 3;
                        }
                    }
                    float temp1, temp2, temp3;
                    float temp = 0;
                    int t = 0;
                    float[] vet = new float[256];
                    //Пробегаемся по всем полутонам для поиска такого, при котором внутриклассовая дисперсия минимальна
                    for (int i = 0; i < 256; i++)
                    {
                        temp1 = Px(0, i, hist);
                        temp2 = Px(i + 1, 255, hist);
                        temp3 = temp1 * temp2;
                        if (temp3 == 0) temp3 = 1;
                        float diff = (Mx(0, i, hist) * temp2) - (Mx(i + 1, 255, hist) * temp1);
                        if (temp < (float)diff * diff / temp3)
                        {
                            temp = (float)diff * diff / temp3;
                            t = i;
                        }
                    }
                    //Сам алгоритм Отсу 
                    for (int i = startH; i < height; i++)
                    {
                        int offset = bmpData.Stride * i + startW * 3;
                        for (int j = startW; j < width; j++)
                        {
                            rgbValues[offset] = (byte)((rgbValues[offset] > (byte)t) ? 255 : 0);
                            rgbValues[offset + 1] = (byte)((rgbValues[offset + 1] > (byte)t) ? 255 : 0);
                            rgbValues[offset + 2] = (byte)((rgbValues[offset + 2] > (byte)t) ? 255 : 0);

                            offset += 3;
                        }
                    }
                }
            }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
        public static System.Drawing.Bitmap ApplyFilter(System.Drawing.Bitmap srcImage)
        {
            YUVModel yuvModel = new YUVModel(srcImage);
            int min = (int)yuvModel.Ymin;
            int max = (int)yuvModel.Ymax;

            int alpha1 = 0; // Сумма высот всех бинов для класса 1, домноженных на положение их середины
            int beta1 = 0; // Сумма высот всех бинов для класса 1

            float maxSigma = -1; // Максимальное значение межклассовой дисперсии
            int threshold = 0; // Порог, соответствующий maxSigma

            int m = 0; // m - сумма высот всех бинов, домноженных на положение их середины
            int n = 0; // n - сумма высот всех бинов
            for (int t = 0; t <= max - min; t++)
            {
                m += t * (int)yuvModel.histY[t];
                n += (int)yuvModel.histY[t];
            }

            // t пробегается по всем возможным значениям порога
            for (int t = 0; t < 256; t++)
            {
                alpha1 += t * (int)yuvModel.histY[t];
                beta1 += (int)yuvModel.histY[t];

                // Вероятность класса 1.
                float w1 = (float)beta1 / n;

                // a = a1 - a2, где a1, a2 - средние арифметические для классов 1 и 2
                float a = (float)alpha1 / beta1 - (float)(m - alpha1) / (n - beta1);

                float sigma = w1 * (1 - w1) * a * a;

                // Если sigma больше текущей максимальной, то обновляем maxSigma и порог
                if (sigma > maxSigma)
                {
                    maxSigma = sigma;
                    threshold = t;
                }
            }

            for (int i = 0; i < yuvModel.width * yuvModel.height; i++)
            {
                if ((int)yuvModel.Y[i] > threshold)
                {
                    yuvModel.Y[i] = 255;
                    yuvModel.U[i] = 128;
                    yuvModel.V[i] = 128;
                }
                else
                {
                    yuvModel.Y[i] = 0;
                    yuvModel.U[i] = 128;
                    yuvModel.V[i] = 128;
                }
            }

            return yuvModel.GetRGBFromYUV();
        }

        public static double[,] Sobel3x3Horizontal
        {
            get
            {
                return new double[,]
                { { -1,  0,  1, },
                  { -2,  0,  2, },
                  { -1,  0,  1, }, };
            }
        }

        public static double[,] Sobel3x3Vertical
        {
            get
            {
                return new double[,]
                { {  1,  2,  1, },
                  {  0,  0,  0, },
                  { -1, -2, -1, }, };
            }
        }


        public static Bitmap ConvolutionFilter(Bitmap sourceBitmap, double[,] xFilterMatrix, double[,] yFilterMatrix, double factor = 1, int bias = 0)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0,
                                     sourceBitmap.Width, sourceBitmap.Height),
                                                       ImageLockMode.ReadOnly,
                                                  PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            /*  if (grayscale == true)
              {
                  float rgb = 0;

                  for (int k = 0; k < pixelBuffer.Length; k += 4)
                  {
                      rgb = pixelBuffer[k] * 0.11f;
                      rgb += pixelBuffer[k + 1] * 0.59f;
                      rgb += pixelBuffer[k + 2] * 0.3f;

                      pixelBuffer[k] = (byte)rgb;
                      pixelBuffer[k + 1] = pixelBuffer[k];
                      pixelBuffer[k + 2] = pixelBuffer[k];
                      pixelBuffer[k + 3] = 255;
                  }
              }*/

            double blueX = 0.0;
            double greenX = 0.0;
            double redX = 0.0;

            double blueY = 0.0;
            double greenY = 0.0;
            double redY = 0.0;

            double blueTotal = 0.0;
            double greenTotal = 0.0;
            double redTotal = 0.0;

            int filterOffset = 1;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blueX = greenX = redX = 0;
                    blueY = greenY = redY = 0;

                    blueTotal = greenTotal = redTotal = 0.0;

                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            blueX += (double)(pixelBuffer[calcOffset]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            greenX += (double)(pixelBuffer[calcOffset + 1]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            redX += (double)(pixelBuffer[calcOffset + 2]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            blueY += (double)(pixelBuffer[calcOffset]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            greenY += (double)(pixelBuffer[calcOffset + 1]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            redY += (double)(pixelBuffer[calcOffset + 2]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];
                        }
                    }

                    blueTotal = Math.Sqrt((blueX * blueX) + (blueY * blueY));
                    greenTotal = Math.Sqrt((greenX * greenX) + (greenY * greenY));
                    redTotal = Math.Sqrt((redX * redX) + (redY * redY));

                    if (blueTotal > 255)
                    { blueTotal = 255; }
                    else if (blueTotal < 0)
                    { blueTotal = 0; }

                    if (greenTotal > 255)
                    { greenTotal = 255; }
                    else if (greenTotal < 0)
                    { greenTotal = 0; }

                    if (redTotal > 255)
                    { redTotal = 255; }
                    else if (redTotal < 0)
                    { redTotal = 0; }

                    resultBuffer[byteOffset] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                  PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }


        public static Bitmap InverseBitmap(Bitmap sourcePicture)
        {
            YUVModel yuvModel = new YUVModel(sourcePicture);
            for (int i = 0; i < yuvModel.width * yuvModel.height; i++)
            {
                yuvModel.Y[i] = 255 - yuvModel.Y[i];
            }
            return yuvModel.GetRGBFromYUV();
        }


    }

    class YUVModel
    {
        public double[] Y;
        public double[] U;
        public double[] V;
        public double Ymin = 255;
        public double Ymax = 0;
        public double[] histY;
        public int width;
        public int height;

        public YUVModel(Bitmap srcImage)
        {
            histY = new double[256];

            BitmapData srcBitmapData = srcImage.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height),
               ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            width = srcBitmapData.Width;
            height = srcBitmapData.Height;
            byte[] pixels = new byte[width * height * 3];
            Y = new double[width * height];
            U = new double[width * height];
            V = new double[width * height];

            Marshal.Copy(srcBitmapData.Scan0, pixels, 0, pixels.Length);
            srcImage.UnlockBits(srcBitmapData);

            for (int i = 0; i < pixels.Length; i += 3)
            {
                byte r = pixels[i + 2];
                byte g = pixels[i + 1];
                byte b = pixels[i];
                double y = 0.299 * r + 0.587 * g + 0.114 * b;
                Y[i / 3] = y;
                histY[Convert.ToInt32(y)]++;
                if (y < Ymin)
                {
                    Ymin = y;
                }
                if (y > Ymax)
                {
                    Ymax = y;
                }
                U[i / 3] = -0.14713 * r - 0.28886 * g + 0.436 * b + 128;
                V[i / 3] = 0.615 * r - 0.51499 * g - 0.10001 * b + 128;
            }
        }

        public Bitmap GetRGBFromYUV()
        {
            Bitmap resBitmap = new Bitmap(width, height);

            BitmapData resBitmapData = resBitmap.LockBits(new Rectangle(0, 0, resBitmap.Width,
                resBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            byte[] pixels = new byte[width * height * 3];
            for (int i = 0; i < pixels.Length; i += 3)
            {
                pixels[i + 2] = ToByte(Y[i / 3] + 1.4075 * (V[i / 3] - 128));
                pixels[i + 1] = ToByte(Y[i / 3] - 0.3455 * (U[i / 3] - 128) - (0.7169 * (V[i / 3] - 128)));
                pixels[i] = ToByte(Y[i / 3] + 1.7790 * (U[i / 3] - 128));
            }
            Marshal.Copy(pixels, 0, resBitmapData.Scan0, pixels.Length);
            resBitmap.UnlockBits(resBitmapData);

            return resBitmap;
        }

        private byte ToByte(double value)
        {
            return Convert.ToByte(Math.Min(Math.Max(value, 0), 255));
        }

    }
}

