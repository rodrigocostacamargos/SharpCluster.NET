using SharpCluster.Metrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.ValidationIndex
{
    public class SilhouetteIndex
    {
        ArrayList agrupamento;
        public SilhouetteIndex(ArrayList agrupamento)
        {
            this.agrupamento = agrupamento;
        }
        public double CalculaIndice()
        {
            double somaAgrupamento = 0;
            for (int i = 0; i < agrupamento.Count; i++)
            {
                double padroesGrupo = 0;
                double somaGrupo = 0;
                Dictionary<String, Double[]> grupo = (Dictionary<String, Double[]>)agrupamento[i];
                foreach (KeyValuePair<String, Double[]> padrao in grupo)
                {
                    if (padrao.Key != "Centroide")
                    {
                        padroesGrupo++;
                        double maior = 0;
                        double a = CalculaA(padrao.Key, grupo);
                        double b = CalculaB(padrao.Value, i);
                        if (a > b)
                        {
                            maior = a;
                        }
                        else
                        {
                            maior = b;
                        }
                        a = b - a;
                        somaGrupo += a / maior;
                    }
                }
                somaAgrupamento += (somaGrupo / padroesGrupo);
            }
            return (somaAgrupamento / agrupamento.Count);
        }

        private double CalculaB(double[] padraoX, int indiceGrupo)
        {
            double distancia = 0;
            double menorDistancia = double.PositiveInfinity;
            int quantidade = 0;
            for (int i = 0; i < agrupamento.Count; i++)
            {
                if (i != indiceGrupo)
                {
                    Dictionary<String, Double[]> grupo = (Dictionary<String, Double[]>)agrupamento[i];
                    foreach (KeyValuePair<String, Double[]> padrao in grupo)
                    {
                        if (padrao.Key != "Centroide")
                        {
                            distancia += CalculaDistancias(padrao.Value, padraoX);
                            quantidade++;
                        }
                    }
                    distancia = (distancia / quantidade);
                    if (distancia < menorDistancia)
                    {
                        menorDistancia = distancia;
                    }
                    distancia = 0;
                    quantidade = 0;
                }
            }
            return menorDistancia;
        }
        private double CalculaA(string x, Dictionary<string, double[]> grupo)
        {
            double dissimilaridade = 0;
            int quantidade = 0;
            foreach (KeyValuePair<String, Double[]> padrao in grupo)
            {
                if ((x != padrao.Key) && (padrao.Key != "Centroide"))
                {
                    dissimilaridade += CalculaDistancias(padrao.Value, grupo[x]);
                    quantidade++;
                }
            }

            if (quantidade == 0)
            {
                return 0;
            }
            else
            {
                return (dissimilaridade / quantidade);
            }


        }
        private double CalculaDistancias(double[] padrao1, double[] padrao2)
        {
            return Distance.GetEuclidianDistance(padrao1, padrao2);
        }
    }
}
