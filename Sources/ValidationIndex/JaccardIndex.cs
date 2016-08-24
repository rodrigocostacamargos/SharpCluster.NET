using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.ValidationIndex
{
    public class JaccardIndex
    {
        ArrayList agrupamento;
        private string[] classe;
        private int quantidadePadroes;
        public JaccardIndex(string[] classe, ArrayList agrupamento)
        {
            this.agrupamento = agrupamento;
            this.classe = classe;
            quantidadePadroes = classe.GetLength(0);
        }

        public JaccardIndex(ArrayList agrupamento)
        {
            this.agrupamento = agrupamento;
        }


        public double CalculaJaccard()
        {
            double a = 0;
            double b = 0;
            double c = 0;
            for (int i = 0; i < agrupamento.Count; i++)
            {
                for (int j = 0; j < agrupamento.Count; j++)
                {
                    Dictionary<String, Double[]> grupo1 = (Dictionary<String, Double[]>)agrupamento[i];
                    Dictionary<String, Double[]> grupo2 = (Dictionary<String, Double[]>)agrupamento[j];
                    foreach (KeyValuePair<String, Double[]> padraoG1 in grupo1)
                    {
                        foreach (KeyValuePair<String, Double[]> padraoG2 in grupo2)
                        {
                            if ((padraoG1.Key != "Centroide") && (padraoG2.Key != "Centroide"))
                            {
                                int classeG1 = Convert.ToInt32(padraoG1.Key.Substring(1));
                                int classeG2 = Convert.ToInt32(padraoG2.Key.Substring(1));
                                if (classeG1 < classeG2)
                                {
                                    if ((padraoG1.Value[2] == padraoG2.Value[2]) && i == j)
                                    {
                                        a++;
                                    }
                                    else if ((padraoG1.Value[2] != padraoG2.Value[2]) && i == j)
                                    {
                                        b++;
                                    }
                                    else if ((padraoG1.Value[2] == padraoG2.Value[2]) && i != j)
                                    {
                                        c++;
                                    }

                                }

                            }
                        }
                    }
                }
            }

            double soma = a + b + c;
            double result = a / soma;
            return result;
        }

    }
}
