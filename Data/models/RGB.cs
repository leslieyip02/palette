using System;
using System.Linq;

namespace Palette
{
    public struct RGB : IColor
    {
        public RGB()
        {
            this.Values = new double[] { 0.0, 0.0, 0.0 };
        }

        public RGB(byte r, byte g, byte b)
        {
            this.Values = new double[] { r, g, b };
        }

        // construct from a hexadecimal color string
        public RGB(string hexColor)
        {
            int r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
            int g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
            int b = Convert.ToInt32(hexColor.Substring(5, 2), 16);

            this.Values = new double[] { r, g, b };
        }

        public double[] Values { get; set; }
        
        public void Zero()
        {
            this.Values = this.Values.Select(v => 0.0)
                .ToArray();
        }

        public void AddBy(IColor other)
        {
            this.Values = this.Values.Zip(other.Values, (v0, v1) => v0 + v1)
                .ToArray();
        }

        public void DivideBy(int n)
        {
            this.Values = this.Values.Select(v => v / n)
                .ToArray();
        }

        public double DistanceTo(IColor other)
        {
            return this.Values.Zip(other.Values, (v0, v1) => Math.Pow(v0 - v1, 2))
                .Aggregate(0.0, (acc, v2) => acc + v2);
        }

        public override string ToString()
        {
            return '#' + String.Join("", Values.Select(v =>
                Convert.ToInt32(v).ToString("X").PadLeft(2, '0')));
        }
    }
}
