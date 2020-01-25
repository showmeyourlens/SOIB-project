using FileParser;
using GeneticServiceNS;
using PathFinder;
using System;
using Tools;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            InputFileParser parser = new InputFileParser();
            Console.WriteLine("Choose network to optimize:");
            Console.WriteLine("1: Network_1.txt");
            Console.WriteLine("2: Network_2.txt");
            string choice = Console.ReadLine();
            Network inputNetwork = new Network();
            if (choice.Equals("1") || choice.Equals("2"))
            {
                inputNetwork = parser.ReadNetwork("Network_" + choice + ".txt");
            }
            else
            {
                return;
            }

            //Find all node pairs
            AllPathFinder pathFinder = new AllPathFinder();

            for (int i = 1; i < inputNetwork.Nodes.Count; i++)
            {
                for (int j = i + 1; j < inputNetwork.Nodes.Count + 1; j++)
                {
                    pathFinder.FindAllPaths(i, j, inputNetwork);
                }
            }

            // all paths generated. Finding perfect combination.
            GeneticAlgorithmParameters parameters = new GeneticAlgorithmParameters();
            parameters.InitialPopulationSize = 100;
            parameters.CrossoverProbability = (float)0.2;
            parameters.MutationProbability = (float)0.1;
            parameters.RandomSeed = 4253;
            parameters.LimitValue = 30;
            parameters.StoppingCriteria = StoppingCriteria.NoImprovement;

            string fileName = "Network_" + choice;
            GeneticService geneticService = new GeneticService(parameters, inputNetwork, pathFinder, fileName);
            geneticService.Solve();

        }
    }
}
