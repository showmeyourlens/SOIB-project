using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class Node
    {
        public int NodeId { get; set; }
        public List<Link> ConnectedLinks { get; set; }
        public List<Node> ConnectedNodes { get; set; }

        public Node(int id)
        {
            NodeId = id;
            ConnectedLinks = new List<Link>();
            ConnectedNodes = new List<Node>();
        }
    }
}
