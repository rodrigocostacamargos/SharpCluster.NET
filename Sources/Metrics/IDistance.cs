using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpCluster.Metrics
{
    interface IDistance
    {
        double GetEuclidianDistance(double[] x, double[] y);
    }
}
