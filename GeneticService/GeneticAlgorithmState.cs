using System.Diagnostics;
using Tools;

namespace GeneticServiceNS
{
    public class GeneticAlgorithmState
    {
        public Stopwatch ElapsedTime { get; set; }

        public int NumberOfGenerations { get; set; }

        public int NumberOfMutations { get; set; }

        public int NumberOfGenerationsWithoutImprovement { get; set; }

        public Network BestChromosomeOptimizationResult { get; set; }

        public NetworkSolution BestChromosomeNetworkSolution { get; set; }

        public int BestChromosomeFitness { get; set; }
    }
}
