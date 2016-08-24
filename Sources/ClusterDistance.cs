using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster
{
    public static class ClusterDistance
    {
        public enum Strategy
        {
            SingleLinkage,
            CompleteLinkage,
            AverageLinkageWPGMA,
            AverageLinkageUPGMA
        }

        // this method compute distance between 2 singleton clusters
        public static double ComputeDistance(Cluster cluster1, Cluster cluster2)
        {
            double distance = 0;

            // if singleton cluster, then compute distance between patterns
            if (cluster1.QuantityOfPatterns() == 1 && cluster2.QuantityOfPatterns() == 1)
                distance = _ComputePatternDistance(cluster1.GetPattern(0), cluster2.GetPattern(0));

            return distance;
        }

        // this method compute distance between clusters thas has subclusters (cluster2 represents the new cluster)
        public static double ComputeDistance(Cluster cluster1, Cluster cluster2, DissimilarityMatrix dissimilarityMatrix, Strategy strategy)
        {
            double distance1, distance2, distance = 0;
            //get the distance between cluster1 and subcluster0 of the cluster2
            distance1 = dissimilarityMatrix.ReturnClusterPairDistance(new ClusterPair(cluster1, cluster2.GetSubCluster(0)));
            //get the distance between cluster1 and subcluster1 of the cluster cluster2
            distance2 = dissimilarityMatrix.ReturnClusterPairDistance(new ClusterPair(cluster1, cluster2.GetSubCluster(1)));

            switch (strategy)
            {
                case Strategy.SingleLinkage: distance = _MinValue(distance1, distance2); break;
                case Strategy.CompleteLinkage: distance = _MaxValue(distance1, distance2); break;
                case Strategy.AverageLinkageWPGMA: distance = (distance1 + distance2) / 2; break;
                case Strategy.AverageLinkageUPGMA:
                    distance = ((cluster2.GetSubCluster(0).TotalQuantityOfPatterns * distance1) / cluster2.TotalQuantityOfPatterns) + ((cluster2.GetSubCluster(1).TotalQuantityOfPatterns * distance2) / cluster2.TotalQuantityOfPatterns);
                    break;
                default: break;
            }

            return distance;

        }

        private static double _MinValue(double value1, double value2)
        {
            return value1 < value2 ? value1 : value2;
        }

        private static double _MaxValue(double value1, double value2)
        {
            return value1 > value2 ? value1 : value2;
        }

        private static double _ComputePatternDistance(Pattern pattern1, Pattern pattern2)
        {
              return  Metrics.Distance.GetEuclidianDistance(pattern1.GetAttributes(), pattern2.GetAttributes());
        }
    }
}
