using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.CrossOver
{
    class SinglePointCrossOver
    {
        private int DataSize;
        private Random Random;

        public SinglePointCrossOver (int dataSize)
        {
            DataSize = dataSize;
            Random = new Random();
        }

        public Tuple<double[], double[]> Calculate (Tuple<double[], double[]> parents)
        {
            double[][] offspring = new double[2][];
            offspring[0] = new double[DataSize];
            offspring[1] = new double[DataSize];
            
            offspring[0][1] = parents.Item2[0];
            offspring[1][0] = parents.Item1[1];

            return new Tuple<double[], double[]>(offspring[0], offspring[1]);
        }
    }
}
