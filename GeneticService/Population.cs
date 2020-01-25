using System.Collections.Generic;

namespace GeneticServiceNS
{
    public class Population
    {
        public Population(List<Chromosome> chromosomes)
        {
            Chromosomes = chromosomes;
        }

        public List<Chromosome> Chromosomes { get; set; }
    }
}
