using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    public struct CMYK : IColor
    {
        public CMYK() =>
            Values = new double[]{ 0.0, 0.0, 0.0, 0.0 };

        public CMYK(byte r, byte g, byte b)
        {
            // edge case for black
            if (r == 0 && g == 0 && b == 0)
            {
                Values = new double[]{ 0.0, 0.0, 0.0, 1.0 };
                return;
            }

            // find reciprocals of RGB values
            double[] reciprocals = new double[]{ r / 255.0,
                g / 255.0, b / 255.0 };

            double k = 1 - reciprocals.Max();

            double c = (1 - reciprocals[0] - k) / (1 - k);
            double m = (1 - reciprocals[1] - k) / (1 - k);
            double y = (1 - reciprocals[2] - k) / (1 - k);

            Values = new double[]{ c, m, y, k };
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
            double k = Values[3];
            return '#' + String.Join("", Values.Take(3).Select(v =>
                Convert.ToInt32(255 * (1 - v) * (1 - k)).ToString("X").PadLeft(2, '0')));
        }
    }
}
