using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.ValidationIndex
{
    public static class DaviesBouldinIndex
    {
        // 1 passo: calcular o centro de cada grupo.
        private static Pattern ReturnClusterCenter(Cluster cluster)
        {
            double attributeSum = 0;
            double attributeMean = 0;

            double[] centroidAttribute = new double[cluster.GetPatterns()[0].GetDimension()];

            for (int j = 0; j < cluster.GetPattern(0).GetDimension(); j++)
            {
                // -- percorre cada padrao do grupo
                foreach (Pattern pattern in cluster.GetPatterns())
                {
                    attributeSum += pattern.GetAttribute(j);
                }

                attributeMean = attributeSum / cluster.QuantityOfPatterns();
                centroidAttribute[j] = attributeMean;
                attributeSum = 0;
            }

            Pattern newCentroid = new Pattern();
            newCentroid.AddAttributes(centroidAttribute);

            return newCentroid;
        }

        // 2 passo: calcular a media das distancias entre cada padrao e o centro do grupo.
        private static double ReturnMeanDistanceFromClusterCenter(Cluster cluster)
        {
            Pattern clusterCenter = ReturnClusterCenter(cluster);
            double clusterDistanceFromCenterSum = 0;

            foreach (Pattern pattern in cluster.GetPatterns())
            {
                clusterDistanceFromCenterSum += Metrics.Distance.CalculatePatternDistance(pattern, clusterCenter);
            }

            return clusterDistanceFromCenterSum / cluster.GetPatterns().Count();
        }

        // 3 passo: calcular a distancia entre os centros de dois grupos.
        private static double ReturnDistanceBetweenClusters(Cluster cluster1, Cluster cluster2)
        {
            double meanSum = ReturnMeanDistanceFromClusterCenter(cluster1) + ReturnMeanDistanceFromClusterCenter(cluster2);
            double distance = Metrics.Distance.CalculatePatternDistance(ReturnClusterCenter(cluster1), ReturnClusterCenter(cluster2));
            return meanSum / distance;
        }

        // 4 passo: calcula a distancia entre todos os grupos e armezena do dicionario
        private static Dictionary<ClusterPair, double> CalculateDistanceBetweenClusters(Cluster[] clusters)
        {
            Dictionary<ClusterPair, double> clusterDistanceList = new Dictionary<ClusterPair, double>();

            for (int i = 0; i < clusters.Count() - 1; i++)
            {
                for (int j = i + 1; j < clusters.Count(); j++)
                {
                    clusterDistanceList.Add(new ClusterPair(clusters[i], clusters[j]), ReturnDistanceBetweenClusters(clusters[i], clusters[j]));
                }
            }

            return clusterDistanceList;
        }

        private static double ReturnMaxValue(double value1, double value2)
        {
            return (value1 > value2) ? value1 : value2;
        }

        // 5 passo: retorna um vetor com as maiores distancias entre grupos
        private static double[] MaxDistanceBetweenClusters(Dictionary<ClusterPair, double> clusterDistanceList)
        {
            double[] maxDistances = new double[clusterDistanceList.Count()];

            if (clusterDistanceList.Count() == 1)
            {
                maxDistances[0] = clusterDistanceList.ElementAt(0).Value;
            }
            else
            {
                for (int i = 0; i < clusterDistanceList.Count() - 1; i++)
                {
                    for (int j = i + 1; j < clusterDistanceList.Count(); j++)
                    {
                        maxDistances[i] = ReturnMaxValue(clusterDistanceList.ElementAt(i).Value, clusterDistanceList.ElementAt(j).Value);
                    }
                }

            }


            return maxDistances;
        }

        public static double Validate(Cluster[] clusters)
        {
            double[] maxDistances = MaxDistanceBetweenClusters(CalculateDistanceBetweenClusters(clusters));
   
            return Math.Round(maxDistances.Sum() / maxDistances.Count(), 4); // retorna a razao entre as maiores distancias e a quantidade de distancias (media).
        }

    }
}
