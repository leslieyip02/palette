using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Clustering
{
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
                // Console.WriteLine(rgbData[(i - 2) / 3]);
            }

            KMeans.K = 5;

            string[] centroids = KMeans.Cluster<RGB>(rgbData);
            Console.WriteLine(String.Join("   ", centroids));
        }
    }
}
