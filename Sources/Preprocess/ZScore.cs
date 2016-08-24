using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.Preprocess
{
    public class ZScore
    {
        public PatternMatrix Transform(HashSet<double[]> dataSet)
        {
            Pattern patternTemp = new Pattern(); // padrao
            PatternMatrix patternMatrix; // matriz de padroes
            int index = 0; // identificador do padrao

            PatternMatrix zPatternMatrix; //matriz de padroes normalizados

            patternMatrix = new PatternMatrix();
            zPatternMatrix = new PatternMatrix();

            foreach (var item in dataSet)
            {
                patternTemp = new Pattern();
                patternTemp.Id = index;
                patternTemp.AddAttributes(item);
                patternMatrix.AddPattern(patternTemp);

                index++;
            }

            index = 0;

            double attributeSum = 0;
            double attributeMean = 0;
            double variance = 0;
            double attributeStandardDeviation = 0;

            double zAttribute = 0;
            double[] zpatternAttributes = new double[patternTemp.GetDimension()];

            Pattern zPattern; //armazena o valor normalizado do padrao

            foreach (Pattern pattern in patternMatrix)
            {

                for (int i = 0; i < pattern.GetDimension(); i++)
                {

                    foreach (Pattern patternx in patternMatrix)
                    {
                        attributeSum += patternx.GetAttribute(i);
                    }

                    // calcula a media do atributo
                    attributeMean = attributeSum / patternMatrix.Size();
                    attributeSum = 0;

                    foreach (Pattern patterny in patternMatrix)
                    {
                        variance += Math.Pow((patterny.GetAttribute(i) - attributeMean), 2);
                    }


                    // calcula a variancia do atributo
                    variance = variance / patternMatrix.Size();

                    attributeStandardDeviation = Math.Sqrt(variance); // o desvio padrao eh a raiz quadrada da variancia

                    zAttribute = (pattern.GetAttribute(i) - attributeMean) / attributeStandardDeviation; //atributo normalizado
                    zpatternAttributes[i] = Math.Round(zAttribute, 2);

                    attributeSum = 0;
                    attributeMean = 0;
                    variance = 0;
                    attributeStandardDeviation = 0;
                }

                zPattern = new Pattern();
                zPattern.AddAttributes(zpatternAttributes);
                zPattern.Id = index;
                zPatternMatrix.AddPattern(zPattern);
                index++;
            }

            return zPatternMatrix;
        }

    }
}
