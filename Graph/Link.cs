using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class Link
    {
        public int LinkId { get; set; }
        public int[] ConnectedByLink { get; set; }
        public List<int> LambdasIds { get; set; }

        public Link(int linkId, int firstNodeId, int secondNodeId)
        {
            this.LinkId = linkId;
            ConnectedByLink = new int[2];
            ConnectedByLink[0] = firstNodeId;
            ConnectedByLink[1] = secondNodeId;
            LambdasIds = new List<int>();
        } 

        public Link Clone()
        {
            return new Link(this.LinkId, this.ConnectedByLink[0], this.ConnectedByLink[1]);
        }
    }
}
