using System;
using System.Linq;

namespace Palette
{
    // treat every data point as a tuple of values in the color space
    public interface IColor
    {
        // value of each component
        public double[] Values { get; set; }

        // set all components back to 0
        public void Zero();

        // for finding the average value of each cluster
        public void AddBy(IColor other);
        public void DivideBy(int n);

        // squared euclidean distance between points
        public double DistanceTo(IColor other);

        // convert to hex color 
        public string ToString();
    }
}
