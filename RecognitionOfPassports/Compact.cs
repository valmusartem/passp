using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RecognitionOfPassports
{
    [Serializable]
    public class Compact
    {
        public int[][] Strips { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public float Scale { get; private set; }

        public int SourceHeight { get; private set; }
        public int SourceWidth { get; private set; }
        public bool IsVertical { get; private set; }

        public const int MaxHeight = 600;

        public Compact(Bitmap img, int stripCount, bool vert = false)
        {
            SourceHeight = img.Height;
            SourceWidth = img.Width;
            IsVertical = vert;


            Scale = SourceHeight > MaxHeight ? 1f * MaxHeight / SourceHeight : 1f;

            Height = (int)((IsVertical ? SourceWidth : SourceHeight) * Scale);
            Width = stripCount;

            var w = vert ? Height : Width;
            var h = vert ? Width : Height;

            using (var bmp = new Bitmap(w, h))
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.InterpolationMode = InterpolationMode.Low;
                gr.DrawImage(img, 0, 0, w, h);

                using (var wr = new ImageWrapper(bmp, true))
                {
                    Strips = new int[Width][];
                    for (int x = 0; x < Strips.Length; x++)
                    {
                        var strip = Strips[x] = new int[Height];

                        if (vert)
                            for (int y = 0; y < strip.Length; y++) strip[y] = wr[y, x].G;
                        else
                            for (int y = 0; y < strip.Length; y++) strip[y] = wr[x, y].G;
                    }
                }
            }
        }
    }
}