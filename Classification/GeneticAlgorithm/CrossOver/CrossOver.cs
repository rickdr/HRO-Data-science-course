using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.CrossOver
{
    public interface ICrossOver
    {
        Tuple<double[], double[]> Calculate (Tuple<double[], double[]> parents);
    }
}
