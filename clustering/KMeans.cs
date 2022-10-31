using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clustering
{
    // adapted from:
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/february/data-clustering-detecting-abnormal-data-using-k-means-clustering
    // https://blog.sverrirs.com/2013/08/simple-k-means-algorithm-in-c.html
    public static class KMeans
    {
        // number of clusters
        public static int K { get; set; } = 8;
     
        // 
        public static int MaxIterations { get; set; } = 10;

        // cluster index of each data point
        private static int[] Clusters;
        
        // indices of the centroids
        private static int[] CentroidIds;

        // average RGB values of each cluster
        private static RGB[] Means;

        public static string[] Cluster(RGB[] rgbData)
        {
            Clusters = new int[rgbData.Length];
            Means = new RGB[K];
            CentroidIds = new int[K];

            RandomiseClusters(rgbData.Length);
            
            UpdateMeans(rgbData);
            UpdateCentroids(rgbData);

            bool changed = true;
            int iterations = 0;
            while (changed && iterations < MaxIterations)
            {
                changed = AssignClusters(rgbData);
                UpdateMeans(rgbData);
                UpdateCentroids(rgbData);

                iterations++;
            }

            string[] centroids = new string[K];
            for (int i = 0; i < K; i++)
                centroids[i] = rgbData[CentroidIds[i]].ToString();

            return centroids;
        }

        // randomly assign intial clusters
        private static void RandomiseClusters(int n)
        {
            Random rnd = new Random();

            for (int i = 0; i < Clusters.Length; i++)
                Clusters[i] = rnd.Next(K);
        }

        // sum the RGB values of each cluster,
        // and divide by the total number of values of the cluster
        // to obtain the average RGB values of that cluster
        private static void UpdateMeans(RGB[] rgbData)
        {
            for (int i = 0; i < Means.Length; i++)
                Means[i].R = Means[i].G = Means[i].B = 0.0;

            int[] counts = new int[Clusters.Length];
            for (int i = 0; i < Clusters.Length; i++)
            {
                int cluster = Clusters[i];
                counts[cluster]++;
                Means[cluster].AddBy(rgbData[i]);
            }

            for (int i = 0; i < Means.Length; i++)
                Means[i].DivideBy(counts[i]);
        }

        // for each cluster, find the data point that is
        // closest to the average value of the cluster,
        // and assign it as the centroid
        private static void UpdateCentroids(RGB[] rgbData)
        {
            double[] closestDistances = new double[K];
            Array.Fill(closestDistances, double.MaxValue);

            for (int i = 0; i < rgbData.Length; i++)
            {
                int cluster = Clusters[i];
                double d = Distance(rgbData[i], Means[cluster]);

                if (d < closestDistances[cluster])
                {
                    closestDistances[cluster] = d;
                    CentroidIds[cluster] = i;
                }
            }
        }

        // assign each data point to the cluster with the closest centroid
        // if no points change clusters in the current iteration,
        // the algorithm has converged and is complete
        private static bool AssignClusters(RGB[] rgbData)
        {
            bool changed = false;

            for (int i = 0; i < rgbData.Length; i++)
            {
                int closestIndex = Clusters[i];
                double closestDistance = Distance(rgbData[i],
                    rgbData[CentroidIds[closestIndex]]);

                for (int j = 0; j < CentroidIds.Length; j++)
                {
                    if (j == Clusters[i])
                        continue;

                    double d = Distance(rgbData[i], rgbData[CentroidIds[j]]); 
                    
                    if (d < closestDistance)
                    {
                        changed = true;
                        closestIndex = j;
                        closestDistance = d;
                    }
                }

                Clusters[i] = closestIndex;
            }

            return changed;
        }

        // treat each RGB value as a 3D vector
        // and find the eulicdean distance between them
        private static double Distance(RGB v, RGB w)
        {
            return Math.Sqrt(Math.Pow(v.R - w.R, 2) +
                Math.Pow(v.G - w.G, 2) + Math.Pow(v.B - w.B, 2));
        }
    }
}
