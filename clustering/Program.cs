using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace clustering
{
    public struct RGB
    {
        public double R, G, B;

        public RGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public void AddBy(RGB color)
        {
            R += color.R;
            G += color.G;
            B += color.B;
        }
        
        public void DivideBy(int d)
        {
            if (d == 0)
                d = 1;

            R /= d;
            G /= d;
            B /= d;
        }

        public override string ToString()
        {
            string hex = "#";

            hex += Convert.ToInt32(R).ToString("X").PadLeft(2, '0');
            hex += Convert.ToInt32(G).ToString("X").PadLeft(2, '0');
            hex += Convert.ToInt32(B).ToString("X").PadLeft(2, '0');

            return hex;
        }
    }

    public class Program
    {
        const int BMP_HEADER_SIZE = 54;

        static byte[] ImageToByteArray(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        static void Main(string[] args)
        {
            // Image img = Image.FromFile("..\\..\\..\\img\\city1.jpg");
            // Image img = Image.FromFile("..\\..\\..\\img\\city2.jpg");
            // Image img = Image.FromFile("..\\..\\..\\img\\scenery1.jpg");
            // Image img = Image.FromFile("..\\..\\..\\img\\scenery2.jpg");
            Image img = Image.FromFile("..\\..\\..\\img\\ui.png");

            byte[] byteData = ImageToByteArray(img)
                .Skip(BMP_HEADER_SIZE)
                .ToArray();

            RGB[] rgbData = new RGB[byteData.Length / 3];

            for (int i = 2; i < byteData.Length; i += 3)
            {
                // color byte data is given in BGR order
                rgbData[(i - 2) / 3] = new RGB(byteData[i],
                    byteData[i - 1], byteData[i - 2]);
            }

            KMeans.K = 8;

            string[] centroids = KMeans.Cluster(rgbData);
            Console.WriteLine(String.Join("   ", centroids));
        }
    }
}
