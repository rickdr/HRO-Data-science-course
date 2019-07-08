using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.Selection
{
    public interface ISelection
    {
        int Calculate (double[][] currentPopulation, double[] fitnesses, int firstParentIndex = -1);
    }
}
