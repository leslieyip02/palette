using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    // treat every data point as a tuple of values in the color space
    public interface IColor
    {
        // value of each component
        public double[] Values { get; set; }

        // set all components back to 0
        public void Zero();

        // for finding the average value of each cluster
        public void AddBy(IColor c);
        public void DivideBy(int n);

        // euclidean distance between points
        public double DistanceTo(IColor c);

        // convert to hex color 
        public string ToString();
    }

    public struct RGB : IColor
    {
        public RGB() =>
            Values = new double[]{ 0.0, 0.0, 0.0 };

        public RGB(byte r, byte g, byte b) =>
            Values = new double[]{ r, g, b };

        public double[] Values { get; set; }
        
        public void Zero() =>
            Values = Values.Select(v => 0.0)
                .ToArray();

        public void AddBy(IColor c) =>
            Values = Values.Zip(c.Values, (v0, v1) => v0 + v1)
                .ToArray();

        public void DivideBy(int n) =>
            Values = Values.Select(v => v / n)
                .ToArray();

        public double DistanceTo(IColor c) =>
            Math.Sqrt(Values
                .Zip(c.Values, (v0, v1) => Math.Pow(v0 - v1, 2))
                .Aggregate(0.0, (acc, v2) => acc + v2));

        public override string ToString() =>
            '#' + String.Join("", Values.Select(v =>
                Convert.ToInt32(v).ToString("X").PadLeft(2, '0')));
    }
}
