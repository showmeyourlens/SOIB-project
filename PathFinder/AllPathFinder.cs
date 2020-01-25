using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace PathFinder
{
    public class AllPathFinder
    {
        public List<Demand> AllDemands { get; private set; }
        private int _currentPathId;
        private int _currentDemandId;

        public AllPathFinder()
        {
            _currentPathId = 0;
            _currentDemandId = 0;
            AllDemands = new List<Demand>();
        }
        public void FindAllPaths(int startNode, int endNode, Network inputNetwork)
        {
            Demand demand = new Demand(_currentDemandId, inputNetwork.GetNode(startNode), inputNetwork.GetNode(endNode));
            _currentDemandId++;
            AllDemands.Add(demand);
            FindPathsRecursive(demand, inputNetwork.GetNode(startNode), inputNetwork, new Path(_currentPathId, inputNetwork.GetNode(startNode)));
        }

        public void FindPathsRecursive(Demand demand, Node currentNode, Network inputNetwork, Path currentPath)
        {
            if (currentNode == demand.EndNode)
            {
                demand.PossiblePaths.Add(currentPath);
                return;
            }

            List<Node> currentNodeNeighbours = currentNode.ConnectedNodes;
            for (int i=0; i < currentNodeNeighbours.Count; i++)
            {
                if (!currentPath.PathNodes.Contains(currentNodeNeighbours[i]))
                {
                    _currentPathId++;
                    Path currentPathCopy = new Path(_currentPathId, currentPath);
                    currentPathCopy.PathNodes.Add(currentNodeNeighbours[i]);
                    Link l = inputNetwork.FindLinkByNodes(currentNode.NodeId, currentNodeNeighbours[i].NodeId);
                    if (l != null)
                    {
                        currentPathCopy.PathLinks.Add(l);
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                    FindPathsRecursive(demand, currentNodeNeighbours[i], inputNetwork, new Path(_currentPathId, currentPathCopy));
                }
            }
        }

        public void FindLinksForPaths()
        {
            
        }
    }
}
