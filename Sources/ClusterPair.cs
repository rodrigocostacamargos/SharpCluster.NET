using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster
{
    //This class stores the pairs of cluster's id which is the dissimilarity matrix entry
    public class ClusterPair
    {
        #region private members
        private Cluster _cluster1;
        private Cluster _cluster2;
        #endregion

        #region constructors
        public ClusterPair()
        {
        }

        public ClusterPair(Cluster cluster1, Cluster cluster2)
        {

            if (cluster1 == null)
                throw new ArgumentNullException("cluster1");

            if (cluster2 == null)
                throw new ArgumentNullException("cluster2");

            this.Cluster1 = cluster1;
            this.Cluster2 = cluster2;
        }
        #endregion

        #region class properties
        public Cluster Cluster1
        {
            get
            {
                return _cluster1;
            }

            set
            {
                _cluster1 = value;
            }
        }

        public Cluster Cluster2
        {
            get
            {
                return _cluster2;
            }

            set
            {
                _cluster2 = value;
            }
        }
        #endregion

        #region class method
        public class EqualityComparer : IEqualityComparer<ClusterPair>
        {
            //see IEqualyComparer_Example in ProgrammingTips folder for better understanding of this concept
            //the implementation of the IEqualityComparer is necessary because ClusterPair has two keys (cluster1.Id and cluster2.Id in ClusterPair) to compare

            public bool Equals(ClusterPair x, ClusterPair y)
            {
                return x._cluster1.Id == y._cluster1.Id && x._cluster2.Id == y._cluster2.Id;
            }

            public int GetHashCode(ClusterPair x)
            {
                return x._cluster1.Id ^ x._cluster2.Id;
            }
        }
        #endregion

    }
}
