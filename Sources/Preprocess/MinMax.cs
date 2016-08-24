using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCluster.Preprocess
{
    public class MinMax
    {
        public PatternMatrix Transform(HashSet<double[]> dataSet)
        {
            Pattern patternTemp = new Pattern(); // padrao
            PatternMatrix patternMatrix; // matriz de padroes
            int index = 0; // identificador do padrao

            PatternMatrix MinMaxPatternMatrix; //matriz de padroes normalizados
            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;
            double minMaxAttribute = 0;

            patternMatrix = new PatternMatrix();
            MinMaxPatternMatrix = new PatternMatrix();
            Pattern minMaxPattern; //armazena o padrao normalizado

            foreach (var item in dataSet)
            {
                patternTemp = new Pattern();
                patternTemp.Id = index;
                patternTemp.AddAttributes(item);
                patternMatrix.AddPattern(patternTemp);

                index++;
            }

            double[] minMaxAttributes = new double[patternTemp.GetDimension()];

            foreach (Pattern pattern in patternMatrix)
            {
                for (int i = 0; i < pattern.GetDimension(); i++)
                {
                    foreach (Pattern patternx in patternMatrix)
                    {
                        if (patternx.GetAttribute(i) < minValue)
                        {
                            minValue = patternx.GetAttribute(i);
                        }

                        if (patternx.GetAttribute(i) > maxValue)
                        {
                            maxValue = patternx.GetAttribute(i);
                        }

                    }


                    minMaxAttribute = (pattern.GetAttribute(i) - minValue) / (maxValue - minValue);
                    minValue = Double.MaxValue;
                    maxValue = Double.MinValue;
                    minMaxAttributes[i] = Math.Round(minMaxAttribute, 2);
                }

                minMaxPattern = new Pattern();
                minMaxPattern.AddAttributes(minMaxAttributes);
                minMaxPattern.Id = index;
                MinMaxPatternMatrix.AddPattern(minMaxPattern);
                index++;

            }



            return MinMaxPatternMatrix;
        }

        private double MinValue(PatternMatrix pm)
        {
            double _minValue = Double.MaxValue;

            foreach (Pattern pattern in pm)
            {
                for (int i = 0; i < pattern.GetDimension(); i++)
                {

                }
            }

            return _minValue;
        }

    }
}
