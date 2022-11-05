using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Clustering
{
    // adapted from:
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/february/data-clustering-detecting-abnormal-data-using-k-means-clustering
    // https://blog.sverrirs.com/2013/08/simple-k-means-algorithm-in-c.html
    public static class KMeans
    {
        // number of clusters
        public static int K { get; set; } = 8;

        // stop after a certain number of iterations
        // since the result is probably good enough
        public static int MaxIterations { get; set; } = 10;

        // cluster index of each data point
        private static int[] Clusters;
        
        // indices of the centroids
        private static int[] CentroidIds;

        // the aim of the algorithm is to separate the data into K clusters
        // clusters are initially randomly assigned once the cluster
        // centroids are determined, the algorithm reassigns the data points
        // to new clusters
        // the process ends when there are no more changes to the cluster
        // assignment, or the max number of iterations has been reached
        public static string[] Cluster<T>(T[] data) 
            where T : IColor, new()
        {
            Clusters = new int[data.Length];
            RandomiseClusters(data.Length);
            
            // average values of each cluster
            T[] means = new T[K];
            for (int i = 0; i < K; i++)
                means[i] = new T();
            
            CentroidIds = new int[K];
            
            UpdateMeans(data, ref means);
            UpdateCentroids(data, means);
            
            // Console.WriteLine(String.Join(", ", Means));
            // Console.WriteLine(String.Join(", ", CentroidIds));

            bool changed = true;
            int iterations = 0;
            while (changed && iterations < MaxIterations)
            {
                changed = AssignClusters(data);
                UpdateMeans(data, ref means);
                UpdateCentroids(data, means);

                iterations++;
            }

            string[] centroids = new string[K];
            for (int i = 0; i < K; i++)
                centroids[i] = data[CentroidIds[i]].ToString();

            return centroids;
        }

        // randomly assign intial clusters
        private static void RandomiseClusters(int n)
        {
            Random rnd = new Random();

            Parallel.For(0, Clusters.Length, i => {
                Clusters[i] = rnd.Next(K);
            });
        }

        // sum the components of each point for each cluster,
        // and divide by the total number of values of the cluster
        // to obtain the average values of that cluster
        private static void UpdateMeans<T>(T[] data, ref T[] means) 
            where T : IColor
        {
            for (int i = 0; i < means.Length; i++)
                means[i].Zero();

            int[] counts = new int[Clusters.Length];
            for (int i = 0; i < Clusters.Length; i++)
            {
                int cluster = Clusters[i];
                counts[cluster]++;
                means[cluster].AddBy(data[i]);
            }

            for (int i = 0; i < means.Length; i++)
                means[i].DivideBy(counts[i]);
        }

        // for each cluster, find the data point that is
        // closest to the average value of the cluster,
        // and assign it as the centroid
        private static void UpdateCentroids<T>(T[] data, T[] means)
            where T : IColor
        {
            double[] closestDistances = new double[K];
            Array.Fill(closestDistances, double.MaxValue);

            Parallel.For(0, data.Length, i => {
                int cluster = Clusters[i];
                double d = data[i].DistanceTo(means[cluster]);

                if (d < closestDistances[cluster])
                {
                    closestDistances[cluster] = d;
                    CentroidIds[cluster] = i;
                }
            });
        }

        // assign each data point to the cluster with the closest centroid
        // if no points change clusters in the current iteration,
        // the algorithm has converged and is complete
        private static bool AssignClusters<T>(T[] data)
            where T : IColor
        {
            bool changed = false;

            Parallel.For(0, data.Length, i => {
                int closestIndex = Clusters[i];
                double closestDistance = data[i]
                    .DistanceTo(data[CentroidIds[closestIndex]]);

                for (int j = 0; j < CentroidIds.Length; j++)
                {
                    if (j == Clusters[i])
                        continue;

                    double d = data[i].DistanceTo(data[CentroidIds[j]]);

                    // reassign if there is a closer centroid                    
                    if (d < closestDistance)
                    {
                        changed = true;
                        closestIndex = j;
                        closestDistance = d;
                    }
                }

                Clusters[i] = closestIndex;
            });

            return changed;
        }
    }
}
