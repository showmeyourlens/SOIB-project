using System;
using System.Collections.Generic;

namespace Tools
{
    public class Network
    {
        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }

        public Network()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
        }

        public bool FindLinkByNodes(int firstNodeId, int secondNodeId)
        {
            foreach (Link link in Links)
            {
                if (link.ConnectedByLink[0] == firstNodeId && link.ConnectedByLink[1] == secondNodeId
                    || link.ConnectedByLink[0] == secondNodeId && link.ConnectedByLink[1] == firstNodeId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
