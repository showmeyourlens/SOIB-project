using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class Path
    {
        public int PathId { get; private set; }
        public List<Link> PathLinks { get; set; }
        public List<Node> PathNodes { get; set; }
        public Path(int id, Node startNode)
        {
            PathId = id;
            PathNodes = new List<Node>();
            PathLinks = new List<Link>();
            PathNodes.Add(startNode);
        }

        public Path(int id, Path path)
        {
            PathId = id;
            PathNodes = new List<Node>(path.PathNodes);
            PathLinks = new List<Link>();

            foreach (Link link in path.PathLinks)
            {
                PathLinks.Add(link.Clone());
            }

        }
    }
}
