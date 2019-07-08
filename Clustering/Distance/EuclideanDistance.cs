using System;
using System.Collections.Generic;
using System.Text;

namespace Clustering.Distance
{
    class EuclideanDistance : IDistance
    {
        public double Calculate(int[] x, int[] y)
        {
            double Sum = 0;

            for (int i = 0; i < x.Length; i++)
                Sum += Math.Pow(x[i] - y[i], 2);

            return Math.Sqrt(Sum);
        }
        public double Calculate(double[] x, double[] y)
        {
            double Sum = 0;

            for (int i = 0; i < x.Length; i++)
                Sum += Math.Pow(x[i] - y[i], 2);

            return Math.Sqrt(Sum);
        }
    }
}
