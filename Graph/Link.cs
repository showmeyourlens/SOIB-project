using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    public class Link
    {
        public int linkId { get; set; }
        public int[] ConnectedByLink { get; set; }

        public Link(int linkId, int firstNodeId, int secondNodeId)
        {
            this.linkId = linkId;
            ConnectedByLink = new int[2];
            ConnectedByLink[0] = firstNodeId;
            ConnectedByLink[1] = secondNodeId;
        } 
    }
}
