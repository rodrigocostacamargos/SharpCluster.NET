using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster
{
    public class Cluster
    {
        #region private members
        private int _id; // cluster id
        private HashSet<Pattern> _cluster; // singleton cluster formed by one pattern
        private HashSet<Cluster> _subClusters; // child clusters
        private List<Pattern> _patternList; // list with all cluster's patterns and its subclusters
        private int _totalQuantityOfPatterns = 0; // total number of cluster's patterns
        private Pattern _centroid; //cluster's centroid used by k-means clustering algorithm
        #endregion

        #region class properties
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int TotalQuantityOfPatterns
        {
            get
            {
                return _totalQuantityOfPatterns;
            }

            set
            {
                _totalQuantityOfPatterns = value;
            }
        }


        public Pattern Centroid
        {
            get
            {
                return _centroid;
            }

            set
            {
                _centroid = value;
            }
        }

        #endregion

        #region class constructor
        public Cluster()
        {
            _cluster = new HashSet<Pattern>();
            _subClusters = new HashSet<Cluster>();
        }
        #endregion

        #region class methods
        public void AddPattern(Pattern pattern)
        {
            _cluster.Add(pattern);
        }

        public int QuantityOfPatterns()
        {
            return _cluster.Count;
        }

        public Pattern[] GetPatterns()
        {
            return _cluster.ToArray<Pattern>();
        }

        public Pattern GetPattern(int index)
        {
            return _cluster.ElementAt(index);
        }

        public void AddSubCluster(Cluster subCluster)
        {
            _subClusters.Add(subCluster);
        }

        public Cluster[] GetSubClusters()
        {
            return _subClusters.ToArray<Cluster>();
        }

        public int QuantityOfSubClusters()
        {
            return _subClusters.Count;
        }

        public Cluster GetSubCluster(int index)
        {
            return _subClusters.ElementAt(index);
        }

        public int UpdateTotalQuantityOfPatterns()
        {
            //if cluster has subclustes, then calculate how many patterns there is in each subcluster
            if (this.GetSubClusters().Count() > 0)
            {
                _totalQuantityOfPatterns = 0;
                foreach (Cluster subcluster in this.GetSubClusters())
                    _totalQuantityOfPatterns = _totalQuantityOfPatterns + subcluster.UpdateTotalQuantityOfPatterns();
            }

            // if there is no subcluster, it is because is a singleton cluster (i.e., totalNumberOfPatterns = 1)
            return _totalQuantityOfPatterns;
        }

        public void ClearPatterns()
        {
            _cluster.Clear();
        }

        public List<Pattern> GetAllPatterns()
        {
            _patternList = new List<Pattern>();
            if (this.QuantityOfSubClusters() == 0)
            {
                foreach (Pattern pattern in this.GetPatterns())
                    _patternList.Add(pattern);
            }
            else
            {
                foreach (Cluster subCluster in this.GetSubClusters())
                    _GetSubClusterPattern(subCluster);
            }
   
            return _patternList;
        }

        private void _GetSubClusterPattern(Cluster subCluster)
        {
            if (subCluster.QuantityOfSubClusters() == 0)
            {
                foreach (Pattern pattern in subCluster.GetPatterns())
                    _patternList.Add(pattern);
            }
            else
            {
                foreach (Cluster _subCluster in subCluster.GetSubClusters())
                    _GetSubClusterPattern(_subCluster);
            }
        }

        #endregion
    }
}
