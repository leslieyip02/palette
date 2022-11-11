using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    public struct HSV : IColor
    {
        public HSV() =>
            Values = new double[]{ 0.0, 0.0, 0.0 };

        public HSV(byte r, byte g, byte b)
        {
            // find reciprocals of RGB values
            double[] reciprocals = new double[]{ r / 255.0,
                g / 255.0, b / 255.0 };

            // edge case when all RGB values are the same
            if (reciprocals[0] == reciprocals[1] &&
                reciprocals[1] == reciprocals[2])
            {
                Values = new double[]{ 0.0, 0.0, reciprocals[0] };
                return;
            }

            int cMaxIndex = 0;
            int cMinIndex = 0;

            for (int i = 1; i < 3; i++)
            {
                if (reciprocals[i] > reciprocals[cMaxIndex])
                    cMaxIndex = i;

                if (reciprocals[i] < reciprocals[cMinIndex])
                    cMinIndex = i;
            }

            double d = reciprocals[cMaxIndex] - reciprocals[cMinIndex];

            double h = 60;
            switch (cMaxIndex)
            {
                case 0:
                    h *= (((reciprocals[1] - reciprocals[2]) / d) % 6);
                    break;

                case 1:
                    h *= ((reciprocals[2] - reciprocals[0]) / d + 2);
                    break;

                case 2:
                    h *= ((reciprocals[0] - reciprocals[1]) / d + 4);
                    break;

                default:
                    throw new IndexOutOfRangeException("Color could not be formatted properly.");
            }

            // make sure 0 ≤ h < 360
            h = (h + 360) % 360;

            double s = d / reciprocals[cMaxIndex];
            double v = reciprocals[cMaxIndex];

            Values = new double[]{ h, s, v };
        }

        public double[] Values { get; set; }
        
        public void Zero() =>
            Values = Values.Select(v => 0.0)
                .ToArray();

        public void AddBy(IColor other) =>
            Values = Values.Zip(other.Values, (v0, v1) => v0 + v1)
                .ToArray();

        public void DivideBy(int n) =>
            Values = Values.Select(v => v / n)
                .ToArray();

        public double DistanceTo(IColor other) =>
            Values.Zip(other.Values, (v0, v1) => Math.Pow(v0 - v1, 2))
                .Aggregate(0.0, (acc, v2) => acc + v2);

        public override string ToString()
        {
            double c = Values[1] * Values[2];
            double x = (1 - Math.Abs((Values[0] / 60) % 2 - 1)) * c;
            double m = Values[2] - c;

            double[] reciprocals;
            switch (Values[0])
            {
                case >= 0 and < 60:
                    reciprocals = new double[]{ c, x, 0.0 };
                    break;
                
                case >= 60 and < 120:
                    reciprocals = new double[]{ x, c, 0.0 };
                    break;

                case >= 120 and < 180:
                    reciprocals = new double[]{ 0.0, c, x };
                    break;

                case >= 180 and < 240:
                    reciprocals = new double[]{ 0.0, x, c };
                    break;

                case >= 240 and < 300:
                    reciprocals = new double[]{ x, 0.0, c };
                    break;

                case >= 300 and < 360:
                    reciprocals = new double[]{ c, 0.0, x };
                    break;

                default:
                    throw new IndexOutOfRangeException("Invalid hue");
            }

            return '#' + String.Join("", reciprocals.Select(v =>
                Convert.ToInt32(255 * (v + m)).ToString("X").PadLeft(2, '0')));
        }
    }
}
