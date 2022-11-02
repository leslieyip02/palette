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
            string[] imgNames = { "city1.jpg", "city2.jpg", 
                "scenery1.jpg", "scenery2.jpg", "ui.png" };

            Console.WriteLine("Choose an image: ");
            Console.WriteLine("- 1: city1");
            Console.WriteLine("- 2: city2");
            Console.WriteLine("- 3: scenery1");
            Console.WriteLine("- 4: scenery2");
            Console.WriteLine("- 5: ui");

            try
            {
                int n = Convert.ToInt32(Console.ReadLine());
                string path = "..\\..\\..\\img\\" + imgNames[n - 1];

                Image img = Image.FromFile(path);

                byte[] byteData = ImageToByteArray(img)
                    .Skip(BMP_HEADER_SIZE)
                    .ToArray();

                // RGB[] rgbData = new RGB[byteData.Length / 3];

                // for (int i = 2; i < byteData.Length; i += 3)
                // {
                //     // color byte data is given in BGR order
                //     rgbData[(i - 2) / 3] = new RGB(byteData[i],
                //         byteData[i - 1], byteData[i - 2]);
                //     // Console.WriteLine(rgbData[(i - 2) / 3]);
                // }

                // KMeans.K = 5;

                // string[] centroids = KMeans.Cluster<RGB>(rgbData);
                // Console.WriteLine("Palette: " + 
                //     String.Join("   ", centroids));

                CMYK[] cmykData = new CMYK[byteData.Length / 3];
                
                for (int i = 2; i < byteData.Length; i += 3)
                {
                    cmykData[(i - 2) / 3] = new CMYK(byteData[i],
                        byteData[i - 1], byteData[i - 2]);
                }

                KMeans.K = 5;

                string[] centroids = KMeans.Cluster<CMYK>(cmykData);
                Console.WriteLine("Palette: " + 
                    String.Join("   ", centroids));
            }
            catch (Exception e)
            {
                Console.WriteLine("Oops, something went wrong!");
                Console.WriteLine(e);
            }
        }
    }
}
