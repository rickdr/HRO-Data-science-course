using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Selection
{
    class RouletteSelection : ISelection
    {
        Random random;
        public RouletteSelection ()
        {
            random = new Random();
        }

        // 3, 4, 5 voorbeeld
        public int Calculate (double[][] currentPopulation, double[] fitnesses, int firstParentIndex = -1)
        {
            List<double> cumulativeFitnesses = new List<double>();
            double sum = 0;
            cumulativeFitnesses.Add(0);
            for (int i = 1; i < fitnesses.Length; i++)
            {
                sum += fitnesses[i];
            }

            double finalSum = 0;
            for (int i = 1; i < fitnesses.Length; i++)
            {
                fitnesses[i] = 1 - (fitnesses[i] / sum);
                finalSum += fitnesses[i];
                cumulativeFitnesses.Add(cumulativeFitnesses.ElementAt(i) + fitnesses[i]);
            }

            double rand = random.NextDouble(0, finalSum);

            var index = -1;
            foreach (var Fitness in cumulativeFitnesses)
            {
                if (Fitness < rand)
                {
                    index = cumulativeFitnesses.IndexOf(Fitness);
                }
            }
            if (index == -1)
            {
                index = 0;
            }
            
            return index;
        }
    }
}
