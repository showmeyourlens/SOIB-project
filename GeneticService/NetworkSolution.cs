using PathFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace GeneticServiceNS
{
    public class NetworkSolution : Chromosome
    {
        public List<PathAllocation> PathAllocations { get; set; }

        public NetworkSolution()
        {
        }

        public NetworkSolution(AllPathFinder allPaths, Random random)
        {
            PathAllocations = new List<PathAllocation>();

            for (var i = 0; i < allPaths.AllDemands.Count; i++)
            {
                PathAllocations.Add(new PathAllocation(allPaths.AllDemands[i], allPaths.AllDemands[i].PossiblePaths[random.Next(allPaths.AllDemands[i].PossiblePaths.Count)]));
            }
        }

        public NetworkSolution(List<PathAllocation> pathAllocations)
        {
            PathAllocations = pathAllocations;
        }


        public override List<Chromosome> Crossover(Chromosome chromosomeToCrossWith, Random random)
        {
            var parent2 = (NetworkSolution) chromosomeToCrossWith;

            var crossoverPoint = random.Next(PathAllocations.Count);
            var firstPartLength = crossoverPoint;
            var lastPartLength = PathAllocations.Count - crossoverPoint;

            var parent1FirstPart = PathAllocations.GetRange(0, firstPartLength);
            var parent1LastPart = PathAllocations.GetRange(firstPartLength, lastPartLength);

            var parent2FirstPart = parent2.PathAllocations.GetRange(0, firstPartLength);
            var parent2LastPart = parent2.PathAllocations.GetRange(firstPartLength, lastPartLength);

            var child1 = (Chromosome) new NetworkSolution(parent1FirstPart.Concat(parent2LastPart).ToList());
            var child2 = (Chromosome) new NetworkSolution(parent2FirstPart.Concat(parent1LastPart).ToList());

            return new List<Chromosome> {child1, child2};
        }

        public override Chromosome Mutate(Random random)
        {
            var pathAllocations = new List<PathAllocation>();
            foreach(PathAllocation pa in PathAllocations)
            {
                pathAllocations.Add(pa.Clone());
            }

            var geneToMutate = random.Next(0, pathAllocations.Count);

            var pathAllocationToMutate = pathAllocations.Single(x => x.Demand.DemandId == geneToMutate);

            pathAllocationToMutate.ChosenPath = pathAllocationToMutate.Demand.PossiblePaths[random.Next(0, pathAllocationToMutate.Demand.PossiblePaths.Count)];

            return new NetworkSolution(pathAllocations);
        }

        public override Chromosome Clone()
        {
            return new NetworkSolution
            {
                Fitness = Fitness,
                PathAllocations = new List<PathAllocation>(PathAllocations),
            };
        }

        public NetworkSolution CloneNS()
        {
            return new NetworkSolution
            {
                Fitness = Fitness,
                PathAllocations = new List<PathAllocation>(PathAllocations),
            };
        }
    }
}