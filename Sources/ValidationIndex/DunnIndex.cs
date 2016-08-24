using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.ValidationIndex
{
    public static class DunnIndex
    {
        // D=dmin/dmax onde dmin é a menor distância entre todos os grupos e dmax é o maior diâmetro encontrado entre todos os grupos.
        public static double Validate(Cluster[] clusters)
        {
            return Math.Round(MinDistanceBetweenClusters(clusters) / MaxClusterDiameter(clusters), 2);
        }

        private static double MaxClusterDiameter(Cluster[] clusters)
        {
            Dictionary<Cluster, double> diameterList = new Dictionary<Cluster, double>();
            double maxDiameter = double.MinValue;

            foreach (Cluster cluster in clusters)
            {
                diameterList.Add(cluster, ReturnClusterDiameter(cluster));
            }

            maxDiameter = diameterList.Max(x => x.Value);
            return maxDiameter;
        }

        private static double ReturnClusterDiameter(Cluster cluster)
        {
            double diameter = double.MinValue; // diametro do grupo
            double distance = double.MaxValue;

            for (int i = 0; i < cluster.GetPatterns().Count() - 1; i++)
            {
                for (int j = 1; j < cluster.GetPatterns().Count(); j++)
                {
                    distance = Metrics.Distance.CalculatePatternDistance(cluster.GetPatterns()[i], cluster.GetPatterns()[j]);

                    if (distance > diameter)
                    {
                        diameter = distance;
                    }
                }
            }

            return diameter;
        }

        // Calcula a menor distancia entre todos os grupos
        private static double MinDistanceBetweenClusters(Cluster[] clusters)
        {
            DissimilarityMatrix dissimilarityMatrix = new DissimilarityMatrix();
 
            for (int i = 0; i < clusters.Count() - 1; i++)
            {
                for (int j = i + 1; j < clusters.Count(); j++)
                {
                    CalculateDistanceBetweenClusters(clusters[i], clusters[j], dissimilarityMatrix);
                }
            }

            return dissimilarityMatrix.GetLowestDistance();

        }

        // Calcula a menor distancia entre dois grupos
        private static void CalculateDistanceBetweenClusters(Cluster cluster0, Cluster cluster1, DissimilarityMatrix dissimilarityMatrix)
        {
            double distanceBetweenClusters = 0.0; // distancia entre dois grupos
            double minDistance = Double.MaxValue; // menor distancia entre dois grupos

            foreach (Pattern patternCluster0 in cluster0.GetPatterns())
            {
                foreach (Pattern patternCluster1 in cluster1.GetPatterns())
                {
                    distanceBetweenClusters = Metrics.Distance.CalculatePatternDistance(patternCluster0, patternCluster1);

                    if (distanceBetweenClusters < minDistance)
                    {
                        minDistance = distanceBetweenClusters;
                    }
                }
            }

            dissimilarityMatrix.AddClusterPairAndDistance(new ClusterPair(cluster0, cluster1), minDistance);
        }
    }
}
