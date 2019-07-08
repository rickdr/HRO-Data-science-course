using System;
using System.Collections.Generic;
using System.Text;

namespace Clustering.Distance
{
    class CosineDistance : IDistance
    {
        public double Calculate(int[] x, int[] y)
        {
            double Sum = 0;
            double p = 0;
            double q = 0;

            for (int i = 0; i < x.Length; i++)
            {
                Sum += x[i] * y[i];
                p += x[i] * x[i];
                q += y[i] * y[i];
            }

            double den = Math.Sqrt(p) * Math.Sqrt(q);

            return (Sum == 0) ? 0 : Sum / den;
        }

        public double Calculate(double[] x, double[] y)
        {
            double Sum = 0;
            double p = 0;
            double q = 0;

            for (int i = 0; i < x.Length; i++)
            {
                Sum += x[i] * y[i];
                p += x[i] * x[i];
                q += y[i] * y[i];
            }

            double den = Math.Sqrt(p) * Math.Sqrt(q);

            return (Sum == 0) ? 0 : Sum / den;
        }
    }
}
