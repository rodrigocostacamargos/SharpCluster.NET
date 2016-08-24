using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.Partitional
{
    public class KMeans
    {
        private Pattern pattern; // padrao
        private PatternMatrix patternMatrix; // matrix de padroes
        private int index = 0; // identificador do padrao

        public KMeans(HashSet<double[]> dataSet)
        {
            patternMatrix = new PatternMatrix();

            foreach (var item in dataSet)
            {
                pattern = new Pattern();
                pattern.Id = index;
                pattern.AddAttributes(item);
                patternMatrix.AddPattern(pattern);

                index++;
            }

        }

        public KMeans(HashSet<double[]> dataSet, bool hasClass)
        {
            patternMatrix = new PatternMatrix();

            if (hasClass)
            {
                foreach (var item in dataSet)
                {
                    pattern = new Pattern();
                    pattern.Id = index;
                    pattern.AddAttributes(item);
                    pattern.RemoveAttributeAt(pattern.GetDimension() - 1); //remove o atributo classe da colecao de atributos
                    pattern.ClassAttribute = Convert.ToInt32(item[pattern.GetDimension()]); //guarda a classe do padrao
                    patternMatrix.AddPattern(pattern);
                    index++;
                }
            }
            else
            {
                foreach (var item in dataSet)
                {
                    pattern = new Pattern();
                    pattern.Id = index;
                    pattern.AddAttributes(item);
                    patternMatrix.AddPattern(pattern);
                    index++;
                }

            }

        }


        public Cluster[] ExecuteClustering(int k)
        {

            //=============PASSO 1=========================================
            Cluster[] clusters = new Cluster[k]; //cria k grupos   

            //seleciona, randomicamente, K padroes como centroides iniciais
            SelectCentroids(k, clusters);

            //=============PASSO 2=============================================
            //forma k grupos colocando cada padrao ao seu centroide mais proximo
            Pattern[] patterns = patternMatrix.GetPatterns();

            BuildClusters(clusters, patterns);

            //=============PASSO 3=============================================
            // -- Recalcula o centroide de cada grupo. Exemplo: Cluster1 = {[2.6, 4.5], [1.5, 2.1], [5.3, 3.4]}
            //                                                             => (2.6 + 1.5 + 5.3)/3 = 3.1 
            //                                                             => (4.5 + 2.1 + 3.4)/3 = 3.3 
            //                                                             centroid = [3.1, 3.3]
            bool changed;
            RecomputeCentroids(clusters, out changed);

            //=============PASSO 4=============================================
            // -- Verifica se os centroids mudaram. 
            // -- Se sim, repete os passos 2 e 3

            while (changed)
            {
                BuildClusters(clusters,  patterns);
                RecomputeCentroids(clusters, out changed);
            }

            return clusters;
        }

        private void RemovePatternsFromClusters(Cluster[] clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                cluster.ClearPatterns();
            }
        }

        private void RecomputeCentroids(Cluster[] clusters, out bool changed)
        {
 
            double attributeSum = 0;           
            double attributeMean = 0;

            Pattern newCentroid;
            double[] centroidAttribute = new double[clusters[0].Centroid.GetDimension()];

            changed = false;

            // -- Percorre os grupos
            for (int i = 0; i < clusters.Count(); i++)
            {
                // -- percorre cada atributo do padrao
                for (int j = 0; j < clusters[0].GetPattern(0).GetDimension(); j++)
                {
                    // -- percorre cada padrao do grupo
                    foreach (Pattern pattern in clusters[i].GetPatterns())
                    {
                        attributeSum += pattern.GetAttribute(j);
                    }

                    attributeMean = attributeSum / clusters[i].QuantityOfPatterns();
                    centroidAttribute[j] = attributeMean;
                    attributeSum = 0;
                }

                newCentroid = new Pattern();
                newCentroid.AddAttributes(centroidAttribute);
                newCentroid.Id = i;

                // verifica se o centroide do grupo mudou. Se sim, entao atribui o novo centroide ao grupo.
                if (!clusters[i].Centroid.GetAttributes().SequenceEqual(newCentroid.GetAttributes()))
                {
                    changed = true;
                    clusters[i].Centroid = newCentroid; //atribui novo centroide ao grupo.
                }

            }

        }

        private void SelectCentroids(int k, Cluster[] clusters)
        {
            //seleciona, randomicamente, K padroes como centroides iniciais

            Dictionary<int, int> centroidPatternList = new Dictionary<int, int>();
            int centroidPattern = -1;
            Random random = new Random();

            while (centroidPatternList.Count < k)
            {
                centroidPattern = random.Next(0, patternMatrix.Size() - 1);
                if (!centroidPatternList.ContainsKey(centroidPattern))
                {
                    centroidPatternList.Add(centroidPattern, centroidPattern);
                }
            }

            for (int i = 0; i < k; i++)
            {
                //inicializa os k grupos
                clusters[i] = new Cluster();
                clusters[i].Id = i;
                clusters[i].Centroid = patternMatrix.GetPattern(centroidPatternList.ElementAt(i).Value); // vincula o centroide ao grupo
            }
        }

        private void BuildClusters(Cluster[] clusters, Pattern[] patterns)
        {
            double distance = 0;
            Dictionary<Pattern, double> distances = new Dictionary<Pattern, double>(); //armazena os centroides e as distancias entre padroes
            Pattern closestCentroid;

            // Remove padroes que ja estejam nos grupos.
            RemovePatternsFromClusters(clusters);


            // -- Calcula a distancia de cada padrao para cada centroide

            for (int i = 0; i < patterns.Count(); i++)
            {
                for (int j = 0; j < clusters.Count(); j++) 
                {
                    distance = Metrics.Distance.CalculatePatternDistance(patterns[i], clusters[j].Centroid); //calcula a distancia do padrao corrente com o centroide do grupo
                    distances.Add(clusters[j].Centroid, distance); // armazena a distancia entre padrao corrente e o centroide
                }

                //verifica qual centroide esta mais proximo do padrao e adiciona a um cluster
                distance = distances.Min(x => x.Value); //retorna a menor distancia entre o padrao e os k centroides.
                closestCentroid = distances.FirstOrDefault(x => x.Value == distance).Key; //retorna o centroide mais proximo do padrao
                distances.Clear();

                for (int l = 0; l < clusters.Count(); l++)
                {
                    if (clusters[l].Centroid.Id == closestCentroid.Id) //encontra o grupo do centroide
                    {
                        clusters[l].AddPattern(patterns[i]); //adiciona o padrao ao grupo
                    }
                }
            }

        }
    }
}
