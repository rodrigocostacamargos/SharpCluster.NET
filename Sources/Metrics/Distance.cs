using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster.Metrics
{
    public static class Distance
    {
        public static double GetEuclidianDistance(double[] x, double[] y)
        {
            double distance = 0;
            double diff = 0;

            if (x.Length != y.Length)
                throw new ArgumentException("Input vectors must be of the same dimension.");

            for (int i = 0; i < x.Length; i++)
            {
                diff = x[i] - y[i];
                distance += diff * diff;
            }

            return System.Math.Sqrt(distance);

        }

        public static double CalculatePatternDistance(Pattern pattern1, Pattern pattern2)
        {
            return Metrics.Distance.GetEuclidianDistance(pattern1.GetAttributes(), pattern2.GetAttributes());
        }

    }
}
