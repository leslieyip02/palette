using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Clustering
{
    public class Program
    {
        public const int BMP_HEADER_SIZE = 54;

        public static byte[] ImageToByteArray(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        public static void CreatePalette<T>(T[] data) 
            where T : IColor, new()
        {
            string[] centroids = KMeans.Cluster<T>(data);
            Console.WriteLine("Palette: " + 
                String.Join("   ", centroids));
        }

        public static void Main(string[] args)
        {
            string[] imgNames = { "city1.jpg", "city2.jpg", 
                "scenery1.jpg", "scenery2.jpg", "ui.png" };

            Console.WriteLine("Choose an image: ");
            Console.WriteLine("- 1: city1");
            Console.WriteLine("- 2: city2");
            Console.WriteLine("- 3: scenery1");
            Console.WriteLine("- 4: scenery2");
            Console.WriteLine("- 5: ui");

            int n = Convert.ToInt32(Console.ReadLine());
            string path = "..\\..\\..\\img\\" + imgNames[n - 1];
            Image img = Image.FromFile(path);
            byte[] byteData = ImageToByteArray(img)
                .Skip(BMP_HEADER_SIZE)
                .ToArray();

            Console.WriteLine("Choose a format: ");
            Console.WriteLine("- 1: RGB");
            Console.WriteLine("- 2: CMYK");
            Console.WriteLine("- 3: HSL");
            Console.WriteLine("- 4: HSV");

            int m = Convert.ToInt32(Console.ReadLine());
            Stopwatch sw = Stopwatch.StartNew();

            switch (m)
            {
                case 1:
                    RGB[] RGBData = new RGB[byteData.Length / 3];

                    // color byte data is given in BGR order
                    for (int i = 2; i < byteData.Length; i++)
                        RGBData[(i - 2) / 3] = new RGB(byteData[i],
                            byteData[i - 1], byteData[i - 2]);

                    CreatePalette<RGB>(RGBData);
                    break;

                case 2:
                    CMYK[] CMYKData = new CMYK[byteData.Length / 3];

                    for (int i = 2; i < byteData.Length; i++)
                        CMYKData[(i - 2) / 3] = new CMYK(byteData[i],
                            byteData[i - 1], byteData[i - 2]);

                    CreatePalette<CMYK>(CMYKData);
                    break;

                case 3:
                    HSL[] HSLData = new HSL[byteData.Length / 3];

                    for (int i = 2; i < byteData.Length; i += 3)
                        HSLData[(i - 2) / 3] = new HSL(byteData[i],
                            byteData[i - 1], byteData[i - 2]);

                    CreatePalette<HSL>(HSLData);
                    break;

                case 4:
                    HSV[] HSVData = new HSV[byteData.Length / 3];

                    for (int i = 2; i < byteData.Length; i += 3)
                        HSVData[(i - 2) / 3] = new HSV(byteData[i],
                            byteData[i - 1], byteData[i - 2]);

                    CreatePalette<HSV>(HSVData);
                    break;

                default:
                    throw new Exception("Image could not be formatted properly.");
            }

            Console.WriteLine("Time taken: {0:f2} s", sw.Elapsed.TotalSeconds);
        }
    }
}
