using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.CrossOver
{
    class RandomCrossOver : ICrossOver
    {
        private int DataSize;
        private Random Random;

        public RandomCrossOver (int dataSize)
        {
            DataSize = dataSize;
            Random = new Random();
        }

        public Tuple<double[], double[]> Calculate (Tuple<double[], double[]> parents)
        {
            double[][] offspring = new double[2][];
            offspring[0] = new double[DataSize];
            offspring[1] = new double[DataSize];

            for (int i = 0; i < DataSize; i++)
            {
                // Uniform crossover, so randomly crossover data points. Generate number between 0 and 1 to determine which parent to select from.
                if (Random.Next(0, 2) == 0)
                {
                    offspring[0][i] = parents.Item1[i];
                    offspring[1][i] = parents.Item2[i];
                }
                else
                {
                    offspring[0][i] = parents.Item2[i];
                    offspring[1][i] = parents.Item1[i];
                }
            }

            return new Tuple<double[], double[]>(offspring[0], offspring[1]);
        }
    }
}
