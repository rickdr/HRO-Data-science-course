using GeneticAlgorithm.CrossOver;
using GeneticAlgorithm.Selection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithm<T>
    {
        private double CrossoverRate;
        private double MutationRate;
        private bool Elitism;
        private int PopulationSize;
        private int NumIterations;
        private int DataSize;
        private Random Random;
        private DataTable PurchaseData;
        private Func<double[][], double[], int, int> Selection;
        private Func<Tuple<T, T>, Tuple<T, T>> CrossOver;

        public GeneticAlgorithm(double crossoverRate, double mutationRate, bool elitism, int populationSize, int dataSize, int numIterations, DataTable purchaseData)
        {
            CrossoverRate = crossoverRate;
            MutationRate = mutationRate;
            Elitism = elitism;
            PopulationSize = populationSize;
            DataSize = dataSize;
            NumIterations = numIterations;
            PurchaseData = purchaseData;

            Random = new Random();
        }

        public Tuple<T, double> Run(Func<T> createIndividual, Func<T,double> computeFitness, Func<T[],double[],Func<Tuple<T,T>>> selectTwoParents,
            Func<Tuple<T, T>, Tuple<T, T>> crossover, Func<double[][], double[], int, int> selection, Func<T, double, T> mutation)
        {
            Selection = selection;
            CrossOver = crossover;

            // initialize the first population
            var initialPopulation = Enumerable.Range(0, PopulationSize).Select(i => createIndividual()).ToArray();
            
            var currentPopulation = initialPopulation;
            
            for (int generation = 0; generation < NumIterations; generation++)
            {
                // compute fitness of each individual in the population
                double[] fitnesses = Enumerable.Range(0, PopulationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();

                T[] nextPopulation = new T[PopulationSize];

                // apply elitism
                int startIndex = 0;
                Tuple<T, double> bestIndividual = null;
                if (Elitism)
                {
                    var populationWithFitness = currentPopulation.Select((individual, index) => new Tuple<T,double>(individual,fitnesses[index]));
                    var populationSorted = populationWithFitness.OrderBy(tuple => tuple.Item2); // item2 is the fitness
                    bestIndividual = populationSorted.First();
                    if (generation == 0)
                    {
                        nextPopulation[0] = bestIndividual.Item1;
                        startIndex = 1;
                    }
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = selectTwoParents(currentPopulation, fitnesses);
                
                // create the individuals of the next generation
                for (int newInd = startIndex; newInd < PopulationSize; newInd++)
                {
                    // select two parents
                    Tuple<T, T> parents = getTwoParents();

                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<T,T> offspring;
                    if (Random.NextDouble() < CrossoverRate)
                        offspring = CrossOver(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newInd++] = mutation(offspring.Item1, MutationRate);
                    if (newInd < PopulationSize) //there is still space for the second children inside the population
                        nextPopulation[newInd] = mutation(offspring.Item2, MutationRate);
                }
                
                if (Elitism && generation != 0 && bestIndividual != null)
                {
                    var nextPopulationWithFitness = nextPopulation.Select((individual, index) => new Tuple<T, double>(individual, fitnesses[index]));
                    var nextPopulationWorst = nextPopulationWithFitness.OrderByDescending(tuple => tuple.Item2).First(); // item2 is the fitness

                    nextPopulation[nextPopulation.ToList().IndexOf(nextPopulationWorst.Item1)] = bestIndividual.Item1;
                }
                // the new population becomes the current one
                currentPopulation = nextPopulation;
            }

            // recompute the fitnesses on the final population and return the best individual
            double[] finalFitnesses = Enumerable.Range(0, PopulationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();
            return currentPopulation.Select((individual, index) => new Tuple<T, double>(individual, finalFitnesses[index])).OrderBy(tuple => tuple.Item2).First();
        }

        public double[] CreateIndividual()
        {
            double[] individual = new double[DataSize];

            // Generate a random binary number for each index in the data size to create an individual.
            for (int i = 0; i < DataSize; i++)
            {
                individual[i] = Random.NextDouble() * (1 - -1) - 1;
            }

            return individual;

        }

        public double ComputeFitness(double[] individual)
        {
            // Fitness is measured by the sse; the lower the sse, the better the fitness.
            double sse = 0;

            // Loop through all purchase data rows
            foreach (DataRow purchase in PurchaseData.Rows)
            {
                double prediction = 0;

                // Loop through all columns except for PREGNANT collumn, so choose the individual length, which doesn't contain PREGNANT column.
                for (int i = 0; i < individual.Length; i++)
                {
                    prediction += (individual[i] * Convert.ToDouble(purchase[i]));
                }
                sse += Math.Pow(Convert.ToDouble(purchase[PurchaseData.Columns.Count - 1]) - prediction, 2);
            }

            return sse;
        }

        public Func<Tuple<double[], double[]>> SelectTwoParents(double[][] currentPopulation, double[] fitness)
        {
            return () => GetTwoParents(currentPopulation, fitness);
        }


        public Tuple<double[], double[]> GetTwoParents(double[][] currentPopulation, double[] fitness)
        {
            int[] parents = new int[2];

            // Get first parent
            parents[0] = Selection(currentPopulation, fitness, -1);
            // Get 2nd parent, now with an extra parameter: The first parent. This makes sure the first parent doesn't participate in this tournament.
            parents[1] = Selection(currentPopulation, fitness, parents[0]);

            return new Tuple<double[], double[]>(currentPopulation[parents[0]], currentPopulation[parents[1]]);
        }

        public double[] Mutation(double[] individual, double mutationRate)
        {
            for (int i = 0; i < individual.Length; i++)
            {
                
                if (Random.NextDouble() <= mutationRate)
                {
                    // Mirror value; Positive becomes negative and negative becomes positive.
                    individual[i] = new Random().Next() * 2 - 1;
                }
            }
            return individual;
        }
    }
}