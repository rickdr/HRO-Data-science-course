using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using GeneticAlgorithm.CrossOver;
using GeneticAlgorithm.Selection;

namespace GeneticAlgorithm
{
    class Program
    {
        private const int DataSize = 20;
        private const int PopulationSize = 100;
        private const int NumberOfIterations = 40;

        private const string FilePath = "../../RetailMart.csv";

        static void Main(string[] args)
        {
            FileReader reader = new FileReader();
            DataTable purchaseData = reader.ReadDataFromFile(FilePath);

            ICrossOver crossOver = new RandomCrossOver(DataSize);
            ISelection selection = new RouletteSelection();

            GeneticAlgorithm<double[]> geneticAlgorithm = new GeneticAlgorithm<double[]>(0.95, 0.01, true, PopulationSize, DataSize, NumberOfIterations, purchaseData);
            Tuple<double[], double> solution = geneticAlgorithm.Run(geneticAlgorithm.CreateIndividual, geneticAlgorithm.ComputeFitness, geneticAlgorithm.SelectTwoParents, crossOver.Calculate, selection.Calculate, geneticAlgorithm.Mutation);
                        
            Console.WriteLine("Solution: ");

            Console.Write("[");
            int counter = 0;
            foreach (var number in solution.Item1)
            {
                counter++;
                Console.Write(number);
                if (counter < solution.Item1.Length)
                {
                    Console.Write(", ");
                }
            }
            Console.Write("]");

            Console.WriteLine("\n\nTotal fitness: ");
            Console.WriteLine(solution.Item2);
            
            Console.ReadKey();

        }
    }
}
