using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster
{
    // O objetivo desta classe é armazenar o historico da formacao de grupos juntamente com a distancia (altura) em que ocorreu tais fusoes
    // Para ver a estrutura de um dendrograma, ver a figura 8.13 do capitulo 8 do livro Introduction to Data Mining

    public class Dendrogram
    {
        private Dictionary<ClusterPair, double> _dendrogram;

        public Dendrogram()
        {
            _dendrogram = new Dictionary<ClusterPair, double>();
        }


        public void AddClusterPairAndDistance(ClusterPair clusterPair, double distance)
        {
            _dendrogram.Add(clusterPair, distance);
        }

    }
}
