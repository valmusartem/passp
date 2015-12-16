using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace RecognitionOfPassports
{
    public class TextRecognition
    {
        public static int PERCEPTRON_IMAGE_HEIGHT = 30;
        public static int PERCEPTRON_IMAGE_WIDTH = 30;
        public static List<String> RECOGNITION_SYMBOLS = new List<string>() { "0","1","2","3","4","5","6","7","8","9", "а","б","в","г","д","е","ё","ж","з","и",
            "й","к","л","м","н","о","п","р","с","т","у","ф","х","ц","ч","ш","щ","ъ","ы","ь","э","ю","я","і","ў","/","А","Б","Е","Ё","I"};
        public Dictionary<string, int> teatches = new Dictionary<string, int>() {
            {"i5.bmp",RECOGNITION_SYMBOLS.IndexOf("5")},{"i6.bmp",RECOGNITION_SYMBOLS.IndexOf("6")},
            {"i7.bmp",RECOGNITION_SYMBOLS.IndexOf("7")},{"7.bmp",RECOGNITION_SYMBOLS.IndexOf("ж")},
            {"23.bmp",RECOGNITION_SYMBOLS.IndexOf("х")},{"32.bmp",RECOGNITION_SYMBOLS.IndexOf("ю")},
        /*{"i0.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"i1.bmp",RECOGNITION_SYMBOLS.IndexOf("1")},
        {"i2.bmp",RECOGNITION_SYMBOLS.IndexOf("2")},{"i3.bmp",RECOGNITION_SYMBOLS.IndexOf("3")},
        {"i4.bmp",RECOGNITION_SYMBOLS.IndexOf("4")},{"i5.bmp",RECOGNITION_SYMBOLS.IndexOf("5")},
        {"i6.bmp",RECOGNITION_SYMBOLS.IndexOf("6")},{"i7.bmp",RECOGNITION_SYMBOLS.IndexOf("7")},
        {"i8.bmp",RECOGNITION_SYMBOLS.IndexOf("8")},{"i9.bmp",RECOGNITION_SYMBOLS.IndexOf("9")},
        //{"i10.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},{"2.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},
        {"3.bmp",RECOGNITION_SYMBOLS.IndexOf("г")},{"4.bmp",RECOGNITION_SYMBOLS.IndexOf("д")},
        {"5.bmp",RECOGNITION_SYMBOLS.IndexOf("е")},{"7.bmp",RECOGNITION_SYMBOLS.IndexOf("ж")},
        {"8.bmp",RECOGNITION_SYMBOLS.IndexOf("з")},//{"9.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},
        //{"11.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},{"12.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        //{"13.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},{"14.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        //{"15.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"16.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},
        //{"17.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"18.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"19.bmp",RECOGNITION_SYMBOLS.IndexOf("т")},//{"20.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},
        {"23.bmp",RECOGNITION_SYMBOLS.IndexOf("х")},{"24.bmp",RECOGNITION_SYMBOLS.IndexOf("ц")},
        {"25.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},{"26.bmp",RECOGNITION_SYMBOLS.IndexOf("ш")},
        {"27.bmp",RECOGNITION_SYMBOLS.IndexOf("щ")},{"28.bmp",RECOGNITION_SYMBOLS.IndexOf("ъ")},
        {"29.bmp",RECOGNITION_SYMBOLS.IndexOf("ы")},{"30.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},
        {"31.bmp",RECOGNITION_SYMBOLS.IndexOf("э")},{"32.bmp",RECOGNITION_SYMBOLS.IndexOf("ю")},
        //{"33.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},{"36.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},
        /*{"37.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},{"38.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},
        {"39.bmp",RECOGNITION_SYMBOLS.IndexOf("г")},{"40.bmp",RECOGNITION_SYMBOLS.IndexOf("д")},
        {"41.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},{"42.bmp",RECOGNITION_SYMBOLS.IndexOf("Ё")},
        {"43.bmp",RECOGNITION_SYMBOLS.IndexOf("ж")},{"44.bmp",RECOGNITION_SYMBOLS.IndexOf("з")},
        {"45.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"46.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},
        {"47.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},{"48.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"49.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},{"50.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"51.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"52.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},
        {"53.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"54.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"55.bmp",RECOGNITION_SYMBOLS.IndexOf("т")},{"56.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},
        {"57.bmp",RECOGNITION_SYMBOLS.IndexOf("ф")},{"58.bmp",RECOGNITION_SYMBOLS.IndexOf("х")},
        {"59.bmp",RECOGNITION_SYMBOLS.IndexOf("ц")},{"60.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},
        {"61.bmp",RECOGNITION_SYMBOLS.IndexOf("ш")},{"62.bmp",RECOGNITION_SYMBOLS.IndexOf("щ")},
        {"63.bmp",RECOGNITION_SYMBOLS.IndexOf("ъ")},{"66.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},
        {"67.bmp",RECOGNITION_SYMBOLS.IndexOf("э")},{"68.bmp",RECOGNITION_SYMBOLS.IndexOf("ю")},*/
        /*{"69.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},{"70.bmp",RECOGNITION_SYMBOLS.IndexOf("і")},
        {"71.bmp",RECOGNITION_SYMBOLS.IndexOf("ў")},{"82.bmp",RECOGNITION_SYMBOLS.IndexOf("б")},
        {"83.bmp",RECOGNITION_SYMBOLS.IndexOf("ё")},{"84.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},
        {"85.bmp",RECOGNITION_SYMBOLS.IndexOf("ў")},{"86.bmp",RECOGNITION_SYMBOLS.IndexOf("і")},
        {"90.bmp",RECOGNITION_SYMBOLS.IndexOf("ф")},{"91.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},*/
        {"100.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"101.bmp",RECOGNITION_SYMBOLS.IndexOf("э")},
        {"102.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"105.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"106.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"108.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},
        {"109.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},{"110.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},
        {"111.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"112.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},
        {"113.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"114.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},
        {"115.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"116.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},
        {"117.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},{"118.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},
        {"119.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},{"120.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"121.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},{"122.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},
        {"124.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"123.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"125.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"126.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},
        {"127.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},{"128.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"129.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},{"130.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"131.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},{"132.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},
        {"133.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},{"134.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"137.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},{"149.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},
        {"142.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},{"143.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},
        {"145.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},{"146.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},
        {"147.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"148.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"150.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},{"154.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},
        {"151.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},{"153.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},
        {"156.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},{"158.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"159.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"157.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},
        {"160.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},{"161.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},
        {"162.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"163.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"165.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},{"171.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},
        {"166.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},{"167.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},
        {"168.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},{"173.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},
        {"174.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},{"175.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"176.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},{"179.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"181.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},{"182.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"184.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"186.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},
        {"177.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"188.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},
        {"196.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},{"197.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},
        {"198.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},{"202.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},
        {"200.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},{"205.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},
        {"203.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"204.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},
        {"206.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},{"207.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},
        {"209.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},{"210.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},
        {"211.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},{"212.bmp",RECOGNITION_SYMBOLS.IndexOf("д")},
        {"213.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},{"215.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"222.bmp",RECOGNITION_SYMBOLS.IndexOf("д")},{"228.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"229.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"230.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},
        {"238.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"239.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"248.bmp",RECOGNITION_SYMBOLS.IndexOf("ф")},{"240.bmp",RECOGNITION_SYMBOLS.IndexOf("ы")},
        {"253.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"262.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},
        {"256.bmp",RECOGNITION_SYMBOLS.IndexOf("ы")},{"257.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},
        {"260.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"261.bmp",RECOGNITION_SYMBOLS.IndexOf("2")},
        {"263.bmp",RECOGNITION_SYMBOLS.IndexOf("4")},{"264.bmp",RECOGNITION_SYMBOLS.IndexOf("1")},
        {"265.bmp",RECOGNITION_SYMBOLS.IndexOf("9")},{"266.bmp",RECOGNITION_SYMBOLS.IndexOf("8")},
        {"267.bmp",RECOGNITION_SYMBOLS.IndexOf("8")},{"268.bmp",RECOGNITION_SYMBOLS.IndexOf("3")},
        {"269.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"271.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},
        {"272.bmp",RECOGNITION_SYMBOLS.IndexOf("4")},{"273.bmp",RECOGNITION_SYMBOLS.IndexOf("8")},
        {"274.bmp",RECOGNITION_SYMBOLS.IndexOf("8")},{"278.bmp",RECOGNITION_SYMBOLS.IndexOf("3")},
        {"270.bmp",RECOGNITION_SYMBOLS.IndexOf("2")},{"275.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},
        {"276.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"277.bmp",RECOGNITION_SYMBOLS.IndexOf("1")},
        {"279.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"280.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},
        {"281.bmp",RECOGNITION_SYMBOLS.IndexOf("3")},{"285.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"286.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},{"290.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},
        {"291.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},{"292.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},
        {"296.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"297.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},
        {"282.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},{"298.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},
        {"300.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"301.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},
        {"302.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},{"303.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"306.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},{"307.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},
        {"309.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"310.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},
        {"311.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"312.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},
        {"313.bmp",RECOGNITION_SYMBOLS.IndexOf("г")},{"315.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},
        {"316.bmp",RECOGNITION_SYMBOLS.IndexOf("Е")},{"317.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"318.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},{"319.bmp",RECOGNITION_SYMBOLS.IndexOf("/")},
        {"320.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"322.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"324.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},{"325.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"299.bmp",RECOGNITION_SYMBOLS.IndexOf("э")},{"326.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"327.bmp",RECOGNITION_SYMBOLS.IndexOf("и")},{"329.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"331.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"332.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},
        {"335.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"336.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},
        {"341.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"348.bmp",RECOGNITION_SYMBOLS.IndexOf("ы")},
        {"342.bmp",RECOGNITION_SYMBOLS.IndexOf("ь")},{"347.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},
        {"356.bmp",RECOGNITION_SYMBOLS.IndexOf("т")},{"357.bmp",RECOGNITION_SYMBOLS.IndexOf("э")},
        {"358.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"359.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},
        {"370.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"377.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},
        {"375.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"354.bmp",RECOGNITION_SYMBOLS.IndexOf("ч")},
        {"378.bmp",RECOGNITION_SYMBOLS.IndexOf("4")},{"389.bmp",RECOGNITION_SYMBOLS.IndexOf("2")},
        {"380.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"381.bmp",RECOGNITION_SYMBOLS.IndexOf("1")},
        {"382.bmp",RECOGNITION_SYMBOLS.IndexOf("3")},{"388.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},
        {"384.bmp",RECOGNITION_SYMBOLS.IndexOf("1")},{"385.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},
        {"387.bmp",RECOGNITION_SYMBOLS.IndexOf("2")},
        {"390.bmp",RECOGNITION_SYMBOLS.IndexOf("3")},
        {"393.bmp",RECOGNITION_SYMBOLS.IndexOf("0")},{"396.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"398.bmp",RECOGNITION_SYMBOLS.IndexOf("я")},{"399.bmp",RECOGNITION_SYMBOLS.IndexOf("к")},
        {"401.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},{"406.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},
        {"413.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"415.bmp",RECOGNITION_SYMBOLS.IndexOf("н")},
        {"417.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},{"420.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},
        {"423.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"425.bmp",RECOGNITION_SYMBOLS.IndexOf("а")},
        {"376.bmp",RECOGNITION_SYMBOLS.IndexOf("1")},{"386.bmp",RECOGNITION_SYMBOLS.IndexOf("4")},
        {"394.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"400.bmp",RECOGNITION_SYMBOLS.IndexOf("і")},
        {"404.bmp",RECOGNITION_SYMBOLS.IndexOf("ў")},{"424.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},
        {"426.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"427.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},
        {"428.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"429.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},
        {"430.bmp",RECOGNITION_SYMBOLS.IndexOf("г")},{"433.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},
        {"432.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},{"434.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"436.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},{"437.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},
        {"438.bmp",RECOGNITION_SYMBOLS.IndexOf("у")},{"439.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},
        {"440.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"444.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},
        {"445.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},{"451.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},
        {"452.bmp",RECOGNITION_SYMBOLS.IndexOf("А")},{"454.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"446.bmp",RECOGNITION_SYMBOLS.IndexOf("ы")},{"455.bmp",RECOGNITION_SYMBOLS.IndexOf("Ё")},
        {"458.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"459.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},
        {"460.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"462.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"464.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},{"469.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"470.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"471.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},
        {"472.bmp",RECOGNITION_SYMBOLS.IndexOf("р")},{"474.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},
        {"465.bmp",RECOGNITION_SYMBOLS.IndexOf("ц")},{"481.bmp",RECOGNITION_SYMBOLS.IndexOf("п")},
        {"482.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},{"483.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},
        {"485.bmp",RECOGNITION_SYMBOLS.IndexOf("м")},{"486.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},
        {"488.bmp",RECOGNITION_SYMBOLS.IndexOf("л")},{"489.bmp",RECOGNITION_SYMBOLS.IndexOf("Ё")},
        {"490.bmp",RECOGNITION_SYMBOLS.IndexOf("в")},{"491.bmp",RECOGNITION_SYMBOLS.IndexOf("с")},
        {"493.bmp",RECOGNITION_SYMBOLS.IndexOf("й")},{"494.bmp",RECOGNITION_SYMBOLS.IndexOf("о")},
        {"495.bmp",RECOGNITION_SYMBOLS.IndexOf("Б")},{"497.bmp",RECOGNITION_SYMBOLS.IndexOf("с")}
        };

        public void TeachSamples()
        {
            Dictionary<double[], double[]> samples = new Dictionary<double[], double[]>();
            int counter = 0;
            foreach(var el in teatches.Keys)
            {
                Bitmap bmp = new Bitmap(el);
                double[] bits = GetPerceptronInputFromImage(bmp);
                double[] res = new double[RECOGNITION_SYMBOLS.Count];
                res[teatches[el]] = 1;
                if(!samples.ContainsKey(bits))
                    samples.Add(bits,res);
                ++counter;
            }
            Perceptron pc = new Perceptron();
            pc.Reset();
            pc.Teach(samples);
        }

        public double[] GetPerceptronInputFromImage(Bitmap srcImage)
        {
            double[] res = new double[srcImage.Width * srcImage.Height];
            PixelFormat pixelFormat = (Image.GetPixelFormatSize(srcImage.PixelFormat) / 8 > 2) ? srcImage.PixelFormat : PixelFormat.Format24bppRgb;
            int nBytesPerPixel = Image.GetPixelFormatSize(pixelFormat) / 8;
            BitmapData bmpData = srcImage.LockBits(new Rectangle(0, 0, srcImage.Width, srcImage.Height), ImageLockMode.ReadWrite, pixelFormat);
            int bytes = bmpData.Stride * bmpData.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

            for (int y = 0; y < srcImage.Height; ++y)
            {
                for (int x = 0; x < srcImage.Width; ++x)
                {
                    int bytePosition = bmpData.Stride * y + x * nBytesPerPixel;
                    res[y*x] = (rgbValues[bytePosition + 2] == 0) ? 1 : 0;

                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpData.Scan0, bytes);
            srcImage.UnlockBits(bmpData);
            return res;
        }
    }
}
