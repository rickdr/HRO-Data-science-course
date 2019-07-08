using System;
using System.Collections.Generic;
using System.Text;

namespace Clustering
{
    public interface IDistance
    {
        double Calculate(int[] x, int[] y);
        double Calculate(double[] x, double[] y);
    }
}
