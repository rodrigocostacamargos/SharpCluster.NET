using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster
{
    public class DissimilarityMatrix
    {
        // list to store distance values from a pair of clusters Dictionary<ClusterPair, Distance>
        private ConcurrentDictionary<ClusterPair, double> _distanceMatrix;

        public DissimilarityMatrix()
        {
            _distanceMatrix = new ConcurrentDictionary<ClusterPair, double>(new ClusterPair.EqualityComparer());
        }

        public void AddClusterPairAndDistance(ClusterPair clusterPair, double distance)
        {
            _distanceMatrix.TryAdd(clusterPair, distance);
        }

        public void RemoveClusterPair(ClusterPair clusterPair)
        {
            double outvalue;

            if (_distanceMatrix.ContainsKey(clusterPair))
                _distanceMatrix.TryRemove(clusterPair, out outvalue);
            else
                _distanceMatrix.TryRemove(new ClusterPair(clusterPair.Cluster2, clusterPair.Cluster1), out outvalue);
        }

        // get the lowest distance in distance matrix
        public double GetLowestDistance()
        {
            double minDistance = double.MaxValue;
            ClusterPair closestClusterPair = new ClusterPair();

            foreach (var item in _distanceMatrix)
            {
                if (item.Value < minDistance)
                {
                    minDistance = item.Value;
                }
            }

            return minDistance;
        }

        public double ReturnLowestDistanceOld()
        {
            var distanceList = _distanceMatrix.ToList();
            distanceList.Sort((x, y) => x.Value.CompareTo(y.Value)); // it is necessary to find a more performatic way to find this value (very important)
            return distanceList[0].Value;
        }


        // get the closest cluster pair (i.e., min cluster pair distance). it is also important to reduce computational time
        public ClusterPair GetClosestClusterPair()
        {
            double minDistance = double.MaxValue;
            ClusterPair closestClusterPair = new ClusterPair();

            foreach (var item in _distanceMatrix)
            {
                if(item.Value < minDistance)
                {
                    minDistance = item.Value;
                    closestClusterPair = item.Key;
                }
            }

            return closestClusterPair;
        }


        // get the distance value from a cluster pair. THIS METHOD DEPENDS ON THE EqualityComparer IMPLEMENTATION IN ClusterPair CLASS
        public double ReturnClusterPairDistance(ClusterPair clusterPair)
        {
            double clusterPairDistance = Double.MaxValue;

            // look in distance matrix if there is an input of cluster1 and cluster2 (remember that ClusterPair has two childs cluster1 and cluster2)
            if (_distanceMatrix.ContainsKey(clusterPair))
                clusterPairDistance = _distanceMatrix[clusterPair];
            else
                clusterPairDistance = _distanceMatrix[new ClusterPair(clusterPair.Cluster2, clusterPair.Cluster1)]; // if not, look in distance matrix for an input of cluster2 and cluster1 (remember that distance matrix is symetric)

            return clusterPairDistance;
        }
    

    }
}
