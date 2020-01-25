using System;
using Tools;

namespace Tools
{
    public class PathAllocation
    {
        public PathAllocation(Demand demand, Path path)
        {
            if (path == null) throw new NullReferenceException();
            Demand = demand;
            ChosenPath = path;
        }

        public Demand Demand { get; set; }
        public Path ChosenPath { get; set; }

        public int LambdaId { get; set; }

        public PathAllocation Clone()
        {
            return new PathAllocation(Demand, new Path(ChosenPath.PathId, ChosenPath));
        }
    }
}