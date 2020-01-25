using System;
using System.Collections.Generic;
using System.Linq;

namespace Tools
{
    public class Network
    {
        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        public int HighestLambdaId { get; set; }
        

        public Network()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            HighestLambdaId = 0;
        }

        public Node GetNode(int id)
        {
            return Nodes.FirstOrDefault(x => x.NodeId == id);
        }

        public Link FindLinkByNodes(int firstNodeId, int secondNodeId)
        {
            foreach (Link link in Links)
            {
                if (link.ConnectedByLink[0] == firstNodeId && link.ConnectedByLink[1] == secondNodeId
                    || link.ConnectedByLink[0] == secondNodeId && link.ConnectedByLink[1] == firstNodeId)
                {
                    return link;
                }
            }
            return null;
        }

        public Network Clone()
        {
            Network result = new Network();
            result.Nodes = new List<Node>(this.Nodes);
                        
            foreach (Link l in this.Links)
            {
                result.Links.Add(l.Clone());
            }
            return result;
        }

        public int SetHighestLambda()
        {
            HighestLambdaId = Links.Select(x => x.LambdasIds.Count == 0 ? 0 : x.LambdasIds.Max()).Max();
            return HighestLambdaId;
        }
    }
}
