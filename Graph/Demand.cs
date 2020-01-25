using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class Demand
    {
        public int DemandId { get; private set; }
        public Node StartNode { get; private set; }
        public Node EndNode { get; private set; }
        public List<Path> PossiblePaths { get; set; }

        public Demand(int id, Node startNode, Node endNode)
        {
            DemandId = id;
            StartNode = startNode;
            EndNode = endNode;
            PossiblePaths = new List<Path>();
        }
    }
}
