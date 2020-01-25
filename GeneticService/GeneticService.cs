using GeneticServiceNS;
using PathFinder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tools;

namespace GeneticServiceNS
{
    public class GeneticService
    {
        private readonly GeneticAlgorithmParameters _parameters;
        private readonly LambdaCalculator _calculator;
        private readonly OutputWriter _outputWriter;
        private readonly Network _network;
        private readonly AllPathFinder _pathFinder;
        private readonly string _currentFileName;

        public GeneticService(GeneticAlgorithmParameters parameters, Network network, AllPathFinder paths, string fileName)
        {
            _parameters = parameters;
            _network = network;
            _pathFinder = paths;
            _calculator = new LambdaCalculator();
            _outputWriter = new OutputWriter();
            _currentFileName = fileName;
        }

        public void Solve()
        {
            var state = new GeneticAlgorithmState
            {
                ElapsedTime = Stopwatch.StartNew(),
                NumberOfGenerations = 0,
                NumberOfGenerationsWithoutImprovement = 0,
                NumberOfMutations = 0,
                BestChromosomeOptimizationResult = null,
                BestChromosomeFitness = 0
            };


            var random = new Random(_parameters.RandomSeed);

            var population = GenerateInitialPopulation(random);
            EvaluateFitness(population.Chromosomes);

            var bestChromosome = population.Chromosomes.OrderByDescending(x => x.Fitness).First();
            state.BestChromosomeNetworkSolution = ((NetworkSolution)bestChromosome).CloneNS();
            state.BestChromosomeOptimizationResult = CalculateNetworkSolutionOptimizationResult((NetworkSolution)bestChromosome);
            if (state.BestChromosomeOptimizationResult.HighestLambdaId != 0)
            {
                state.BestChromosomeFitness = (1000000 / state.BestChromosomeOptimizationResult.HighestLambdaId);
            }
            else
            {
                state.BestChromosomeFitness = int.MaxValue;
            }

            PrintBestAlgorithmInGeneration(state, true);

            while (!EvaluateStoppingCriteria(state))
            {
                EvaluateFitness(population.Chromosomes);

                var bestChromosomeInGeneration = population.Chromosomes.OrderByDescending(x => x.Fitness).First();

                if (bestChromosomeInGeneration.Fitness > state.BestChromosomeFitness)
                {
                    state.BestChromosomeNetworkSolution = ((NetworkSolution)bestChromosomeInGeneration).CloneNS();
                    state.BestChromosomeOptimizationResult = CalculateNetworkSolutionOptimizationResult((NetworkSolution)bestChromosomeInGeneration);                   
                    state.BestChromosomeFitness = (1000000 / state.BestChromosomeOptimizationResult.HighestLambdaId);

                    state.NumberOfGenerationsWithoutImprovement = 0;

                    PrintBestAlgorithmInGeneration(state, true);
                }
                else
                {
                    state.NumberOfGenerationsWithoutImprovement++;

                    PrintBestAlgorithmInGeneration(state, false);
                }

                var eliteOffsprings = SelectEliteOffsprings(population);
                var crossoveredOffsprings = CrossoverOffsprings(population, eliteOffsprings.Count, random);
                var mutatedOffsprings = MutateOffsprings(crossoveredOffsprings, state, random);

                EvaluateFitness(mutatedOffsprings);

                population = SelectSurvivors(population, eliteOffsprings, mutatedOffsprings);

                state.NumberOfGenerations++;
            }

            state.ElapsedTime.Stop();

            DoPostSolveActivities(state);
        }

        private List<Chromosome> MutateOffsprings(List<Chromosome> crossoveredOffsprings, GeneticAlgorithmState state, Random random)
        {
            List<Chromosome> mutatedOffsprings = new List<Chromosome>();

            foreach (var offspring in crossoveredOffsprings)
            {
                var rand = random.NextDouble();
                if (rand >= _parameters.MutationProbability)
                {
                    state.NumberOfMutations++;

                    mutatedOffsprings.Add(offspring.Mutate(random));
                }
                else
                {
                    mutatedOffsprings.Add(offspring);
                }
            }

            return mutatedOffsprings;
        }

        private List<Chromosome> CrossoverOffsprings(Population population, int eliteOffspringsCount, Random random)
        {
            var crossoveredOffsprings = new List<Chromosome>();

            var eliteCount = Math.Max(eliteOffspringsCount, 1);
            var nonEliteOffspringsCount = population.Chromosomes.Count - eliteCount;

            for (var i = 0; i < nonEliteOffspringsCount / 2; i++)
            {
                var offspring1 = population.Chromosomes[i];
                var offspring2 = population.Chromosomes[nonEliteOffspringsCount - 1 - i];

                var rand = random.NextDouble();
                if (rand >= _parameters.CrossoverProbability)
                {
                    var crossoveredPair = offspring1.Crossover(offspring2, random);
                    crossoveredOffsprings.AddRange(crossoveredPair);
                }
                else
                {
                    crossoveredOffsprings.Add(offspring1);
                    crossoveredOffsprings.Add(offspring2);
                }
            }

            if (nonEliteOffspringsCount % 2 != 0)
            {
                crossoveredOffsprings.Add(population.Chromosomes[nonEliteOffspringsCount / 2]);
            }

            return crossoveredOffsprings;
        }

        private List<Chromosome> SelectEliteOffsprings(Population population)
        {
            var elitePercentage = 0.1;
            var eliteOffspringsCount = (int) Math.Max(elitePercentage * population.Chromosomes.Count, 1);

            return new List<Chromosome>(population.Chromosomes.OrderByDescending(x => x.Fitness).Take(eliteOffspringsCount));
        }

        private void DoPostSolveActivities(GeneticAlgorithmState state)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Best solution found: {state.BestChromosomeOptimizationResult.HighestLambdaId}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.WriteLine($"Genetic Algorithm exited after {(double)state.ElapsedTime.ElapsedMilliseconds / 1000} seconds. " +
                              $"Generations cultured: {state.NumberOfGenerations}.");

            _outputWriter.SaveOutputToTheFile(state.BestChromosomeOptimizationResult, state.BestChromosomeNetworkSolution.PathAllocations, _currentFileName);
        }

        private void PrintBestAlgorithmInGeneration(GeneticAlgorithmState state, bool bestResult)
        {
            if (bestResult)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine($"Best solution in generation { state.NumberOfGenerations }: " + state.BestChromosomeOptimizationResult.HighestLambdaId);

            if (bestResult)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            }
        }

        private void EvaluateFitness(List<Chromosome> chromosomes)
        {
            foreach (var chromosome in chromosomes)
            {
                var networkSolution = (NetworkSolution)chromosome;
                Network calculationResult = CalculateNetworkSolutionOptimizationResult(networkSolution);
                
                if (calculationResult.SetHighestLambda() == 0)
                {
                    networkSolution.Fitness = int.MaxValue;
                }
                else
                {
                    networkSolution.Fitness = (1000000 / calculationResult.HighestLambdaId);
                }
            }
        }

        private Population SelectSurvivors(Population population, List<Chromosome> eliteOffsprings, List<Chromosome> newOffsprings)
        {
            var remainingOffspringsCount = population.Chromosomes.Count - eliteOffsprings.Count;

            var weakestChromosomes = population.Chromosomes.OrderBy(x => x.Fitness).Take(remainingOffspringsCount).ToList();
            var candidates = weakestChromosomes.Concat(newOffsprings);

            var newGeneration = candidates.OrderByDescending(x => x.Fitness).Take(remainingOffspringsCount).ToList();
            newGeneration.AddRange(eliteOffsprings);

            return new Population(newGeneration);
        }

        private Population GenerateInitialPopulation(Random random)
        {
            var chromosomes = new List<Chromosome>();

            for (var i = 0; i < _parameters.InitialPopulationSize; i++)
            {
                chromosomes.Add(CreateRandomChromosome(random));
            }

            return new Population(chromosomes);
        }

        private Chromosome CreateRandomChromosome(Random random)
        {
            return new NetworkSolution(_pathFinder, random);
        }

        private Network CalculateNetworkSolutionOptimizationResult(NetworkSolution networkSolution)
        {
            var network = _network.Clone();
            
            foreach (PathAllocation pathAllocation in networkSolution.PathAllocations)
            {
                ChooseLambdaForDemand(pathAllocation, network);
            }
            
            network.SetHighestLambda();
            return network;
        }

        private void ChooseLambdaForDemand(PathAllocation pathAllocation, Network clonedNetwork)
        {
            List<int> distinctLambdas = new List<int>();
            foreach (Link link in pathAllocation.ChosenPath.PathLinks)
            {
                distinctLambdas.AddRange(clonedNetwork.Links.Single(x => x.LinkId == link.LinkId).LambdasIds);
            }

            distinctLambdas = distinctLambdas.Distinct().ToList();
            distinctLambdas.Sort();
            int lambdaId = 0;

            for(int i=1; i<distinctLambdas.Count + 1; i++)
            {
                if (distinctLambdas[i-1] != i)
                {
                    lambdaId = i;
                    break;
                }
            }

            if (lambdaId == 0)
            {
                lambdaId = distinctLambdas.Count() == 0 ? 1 : distinctLambdas.Count() + 1;
            }

            pathAllocation.LambdaId = lambdaId;

            foreach (Link link in pathAllocation.ChosenPath.PathLinks)
            {
                clonedNetwork.Links.Single(x => x.LinkId == link.LinkId).LambdasIds.Add(lambdaId);
            }

        }

        private bool EvaluateStoppingCriteria(GeneticAlgorithmState state) =>
            _parameters.StoppingCriteria switch
            {
                StoppingCriteria.ElapsedTime => state.ElapsedTime.ElapsedMilliseconds / 1000 >= _parameters.LimitValue,
                StoppingCriteria.NoImprovement => state.NumberOfGenerationsWithoutImprovement >= _parameters.LimitValue,
                StoppingCriteria.NumberOfGenerations => state.NumberOfGenerations >= _parameters.LimitValue,
                StoppingCriteria.NumberOfMutations => state.NumberOfGenerations >= _parameters.LimitValue,
                _ => false
            };
    }
}
