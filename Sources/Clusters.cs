using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    // clustering data structure 
    public class Clusters : IEnumerable
    {
        private HashSet<Cluster> _clusters;

        public Clusters()
        {
            _clusters = new HashSet<Cluster>();
        }

        public int Id { get; private set; }

 
        public void AddCluster(Cluster cluster)
        {
            _clusters.Add(cluster);
        }

        public void RemoveCluster(Cluster cluster)
        {
            _clusters.Remove(cluster);
        }

        public Cluster GetCluster(int index)
        {
            return _clusters.ElementAt(index);
        }

        public Cluster[] GetClusters()
        {
            return _clusters.ToArray<Cluster>();
        }

        public int Count()
        {
            return _clusters.Count;
        }

        //add a single pattern to a cluster 
        public void BuildSingletonCluster(PatternMatrix patternMatrix)
        {
            int clusterId = 0;
            Cluster cluster;

            foreach (Pattern item in patternMatrix)
            {
                cluster = new Cluster();
                cluster.Id = clusterId;
                cluster.AddPattern(item);
                cluster.TotalQuantityOfPatterns = 1;
                _clusters.Add(cluster);
                clusterId++;
            }
        }

        //remove a cluster pair from the clustering data structure
        public void RemoveClusterPair(ClusterPair clusterPair)
        {
            this.RemoveCluster(clusterPair.Cluster1);
            this.RemoveCluster(clusterPair.Cluster2);
        }

        public IEnumerator GetEnumerator()
        {
            return _clusters.GetEnumerator();
        }


    }
}
