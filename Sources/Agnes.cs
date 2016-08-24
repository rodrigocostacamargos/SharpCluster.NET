using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace SharpCluster
{
    // This class implements a agglomerative clustering algorithm based on the description of the Agnes algorithm (AGglomerative NEsting - kaufman rousseeuw 1990 - Finding groups in data)
    public class Agnes
    {

        #region private members
        private Pattern _pattern;
        private PatternMatrix _patternMatrix; 
        private int _patternIndex = 0;
        private Clusters _clusters; // data structure for clustering
        private DissimilarityMatrix _dissimilarityMatrix;
        #endregion

        #region class constructors

        // dataset has no class attribute
        public Agnes(HashSet<double[]> dataSet)
        {
            _clusters = new Clusters();
            _patternMatrix = new PatternMatrix();

            foreach (var item in dataSet)
            {
                _pattern = new Pattern();
                _pattern.Id = _patternIndex;
                _pattern.AddAttributes(item);
                _patternMatrix.AddPattern(_pattern);
                _patternIndex++;
            }
        }

        //dataset can have class attribute
        public Agnes(HashSet<double[]> dataSet, bool hasClass)
        {
            _clusters = new Clusters();
            _patternMatrix = new PatternMatrix();

            if (hasClass)
            {
                foreach (var item in dataSet)
                {
                    _pattern = new Pattern();
                    _pattern.Id = _patternIndex;
                    _pattern.AddAttributes(item);
                    _pattern.RemoveAttributeAt(_pattern.GetDimension() -1); //remove class atribute from attribute collection
                    _pattern.ClassAttribute = Convert.ToInt32(item[_pattern.GetDimension()]); //move class atribute to pattern.ClassAttribute (used for external validation indexes rand and jaccard)
                    _patternMatrix.AddPattern(_pattern);
                    _patternIndex++;
                }
            }
            else
            {
                foreach (var item in dataSet)
                {
                    _pattern = new Pattern();
                    _pattern.Id = _patternIndex;
                    _pattern.AddAttributes(item);
                    _patternMatrix.AddPattern(_pattern);
                    _patternIndex++;
                }

            }

        }
        #endregion
 
        #region deprecated method
        // calcula a distancia entre todos os clusters e as armazena na matrix de dissimilaridade
        private void CreateDissimilarityMatrix()
        {
            double distanceBetweenTwoClusters; 
            _dissimilarityMatrix = new DissimilarityMatrix();
            ClusterPair clusterPair;

            for (int i = 0; i < _clusters.Count(); i++)
            {
                for (int j = i+1; j < _clusters.Count(); j++)
                {
                    clusterPair = new ClusterPair();
                    clusterPair.Cluster1 = _clusters.GetCluster(i);
                    clusterPair.Cluster2 = _clusters.GetCluster(j);

                    distanceBetweenTwoClusters = ClusterDistance.ComputeDistance(clusterPair.Cluster1, clusterPair.Cluster2);
                    _dissimilarityMatrix.AddClusterPairAndDistance(clusterPair, distanceBetweenTwoClusters);
                }
            }
        }
        #endregion

        #region core algorithm functions

        #region Build dissimilarity matrix using .NET parallelism resources
        // compute the distance between all pair of clusters and store it on the dissimilarity matrix. this algorithm step is done using parallelization to improve performance.
        private void _BuildDissimilarityMatrixParallel()
        {
            double distanceBetweenTwoClusters;
            _dissimilarityMatrix = new DissimilarityMatrix();

            Parallel.ForEach(_ClusterPairCollection(), clusterPair =>
            {
                distanceBetweenTwoClusters = ClusterDistance.ComputeDistance(clusterPair.Cluster1, clusterPair.Cluster2);
                _dissimilarityMatrix.AddClusterPairAndDistance(clusterPair, distanceBetweenTwoClusters);
            });
        }

        private IEnumerable<ClusterPair> _ClusterPairCollection()
        {
            ClusterPair clusterPair;

            for (int i = 0; i < _clusters.Count(); i++)
            {
                for (int j = i + 1; j < _clusters.Count(); j++)
                {
                    clusterPair = new ClusterPair();
                    clusterPair.Cluster1 = _clusters.GetCluster(i);
                    clusterPair.Cluster2 = _clusters.GetCluster(j);

                    yield return clusterPair;
                }
            }

        }

        #endregion

        //initially, each pattern form a cluster
        private void _BuildSingletonCluster()
        {
            _clusters.BuildSingletonCluster(_patternMatrix);
        }


        // update dissimilarity matrix with the distance of the new formed cluster
        private void _UpdateDissimilarityMatrix(Cluster newCluster, ClusterDistance.Strategy strategie)
        {
            double distanceBetweenClusters;
            for (int i = 0; i < _clusters.Count(); i++)
            {
                // compute the distance between old clusters to the new cluster
                distanceBetweenClusters = ClusterDistance.ComputeDistance(_clusters.GetCluster(i), newCluster, _dissimilarityMatrix, strategie);
                // insert the new cluster's distance
                _dissimilarityMatrix.AddClusterPairAndDistance(new ClusterPair(newCluster, _clusters.GetCluster(i)), distanceBetweenClusters);
                //remove all old distance values of the old clusters (subclusters of the newcluster)
                _dissimilarityMatrix.RemoveClusterPair(new ClusterPair(newCluster.GetSubCluster(0), _clusters.GetCluster(i)));
                _dissimilarityMatrix.RemoveClusterPair(new ClusterPair(newCluster.GetSubCluster(1), _clusters.GetCluster(i)));
            }

            // finally, remove the distance of the old cluster pair
            _dissimilarityMatrix.RemoveClusterPair(new ClusterPair(newCluster.GetSubCluster(0), newCluster.GetSubCluster(1)));

        }

        private ClusterPair _GetClosestClusterPairInDissimilarityMatrix()
        {
            return _dissimilarityMatrix.GetClosestClusterPair();
        }

        private void BuildHierarchicalClustering(int indexNewCluster, ClusterDistance.Strategy strategy, int k)
        {

            ClusterPair closestClusterPair = this._GetClosestClusterPairInDissimilarityMatrix();

            // create a new cluster by merge the closest cluster pair
            Cluster newCluster = new Cluster();
            newCluster.AddSubCluster(closestClusterPair.Cluster1);
            newCluster.AddSubCluster(closestClusterPair.Cluster2);
            newCluster.Id = indexNewCluster;
            newCluster.UpdateTotalQuantityOfPatterns(); //update the total quantity of patterns of the new cluster (this quantity is used by UPGMA clustering strategy)
     
            //remove the closest cluster pair from the clustering data structure (clusters)
            _clusters.RemoveClusterPair(closestClusterPair);
            _UpdateDissimilarityMatrix(newCluster, strategy);
            //add the new cluster to clustering
            _clusters.AddCluster(newCluster);
            closestClusterPair = null;

            // recursive call of this method while there is more than 1 cluster (k>2) in the clustering
            if (_clusters.Count() > k)
                this.BuildHierarchicalClustering(indexNewCluster + 1, strategy, k);
        }

        public Clusters ExecuteClustering(ClusterDistance.Strategy strategy, int k)
        {

            // Step 1
            // build a clustering only with singleton clusters
            this._BuildSingletonCluster();
            //build the dissimilarity matrix
            this._BuildDissimilarityMatrixParallel();

            // Step 3
            // build the hierarchical clustering 
            this.BuildHierarchicalClustering(_clusters.Count(), strategy, k);

            return _clusters; //represents the clustering data structure
        }

        // this method transform a hierarchical clustering into a partional clustering with k clusters. (this is necessary if we want to compare AGNES and K-Means results)
        public Cluster[] BuildFlatClustersFromHierarchicalClustering(Clusters clusters, int k)
        {
            Cluster[] flatClusters = new Cluster[k]; 
            for (int i = 0; i < k; i++)
            {
                flatClusters[i] = new Cluster();
                flatClusters[i].Id = i;
                foreach (Pattern pattern in clusters.GetCluster(i).GetAllPatterns())
                    flatClusters[i].AddPattern(pattern);
            }
  
            return flatClusters;
        }

        #endregion


        #region optional funcionality
        //this functionality is usefull to see the distance matrix computed and to use as in input to other clustering algorithms implementation (R or Python)
        public void CreateCSVMatrixFile(string path)
        {
            File.Delete(path);
            this._BuildSingletonCluster();

            StringBuilder matrix = new StringBuilder();
            string headerLine = "AggloCluster";

            foreach (Cluster cluster in _clusters)
                headerLine = headerLine + ", Cluster" + cluster.Id;

            bool writeBlank = false;

            matrix.Append(headerLine);

            double distanceBetweenTwoClusters;
            ClusterPair clusterPair;

            for (int i = 0; i < _clusters.Count(); i++)
            {
                matrix.Append("\r\n");
                matrix.Append("Cluster" + _clusters.GetCluster(i).Id);
                writeBlank = false;

                for (int j = 0; j < _clusters.Count(); j++)
                {
                    clusterPair = new ClusterPair();
                    clusterPair.Cluster1 = _clusters.GetCluster(i);
                    clusterPair.Cluster2 = _clusters.GetCluster(j);

                    distanceBetweenTwoClusters = ClusterDistance.ComputeDistance(clusterPair.Cluster1, clusterPair.Cluster2);

                    if (distanceBetweenTwoClusters == 0)
                    {
                        writeBlank = true;
                        matrix.Append(",0");
                    }
                    else
                    {
                        if (writeBlank)
                            matrix.Append("," + string.Empty);
                        else
                            matrix.Append("," + distanceBetweenTwoClusters);
                    }

                }
            }

            File.AppendAllText(path, matrix.ToString());
        }
        #endregion
    }
}
