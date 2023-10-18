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
            string[] imgNames = { "city1.jpg", "city2.jpg", "desert.png",
                "mountains1.jpg", "mountains2.jpg", "ocean.jpg",
                "scenery1.jpg", "scenery2.jpg", "ui.png", "winter.jpg" };

            Console.WriteLine("Choose an image: ");
            Console.WriteLine("- 1 : city1");
            Console.WriteLine("- 2 : city2");
            Console.WriteLine("- 3 : desert");
            Console.WriteLine("- 4 : mountains1");
            Console.WriteLine("- 5 : mountains2");
            Console.WriteLine("- 6 : ocean");
            Console.WriteLine("- 7 : scenery1");
            Console.WriteLine("- 8 : scenery2");
            Console.WriteLine("- 9 : ui");
            Console.WriteLine("- 10: winter");
            Console.WriteLine("- 11: custom");

            int n = Convert.ToInt32(Console.ReadLine());
            if (n == 11)
            {
                Console.WriteLine("Name of image (in the img folder): ");
            }
            string imgName = n == 11
                ? Console.ReadLine().Trim()
                : imgNames[n - 1];
            string path = "..\\..\\..\\img\\" + imgName;
            byte[] byteData;
            try
            {
                Image img = Image.FromFile(path);
                byteData = ImageToByteArray(img)
                    .Skip(BMP_HEADER_SIZE)
                    .ToArray();
            }
            catch (System.IO.FileNotFoundException _)
            {
                Console.WriteLine("{0} not found.", path);
                return;
            }

            if (byteData == null)
            {
                Console.WriteLine("Could not load image.");
                return;
            }

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
                    for (int i = 2; i < byteData.Length; i += 3)
                        RGBData[(i - 2) / 3] = new RGB(byteData[i],
                            byteData[i - 1], byteData[i - 2]);

                    CreatePalette<RGB>(RGBData);
                    break;

                case 2:
                    CMYK[] CMYKData = new CMYK[byteData.Length / 3];

                    for (int i = 2; i < byteData.Length; i += 3)
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
