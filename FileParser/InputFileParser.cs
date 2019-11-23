using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;

namespace FileParser
{
    public class InputFileParser
    {
        Network result;

        public Network ReadNetwork(string fileName) 
        {
            string pathToFile = Path.Combine(Environment.CurrentDirectory, fileName);
            if (File.Exists(pathToFile))
                return ParseFileToNetwork(pathToFile);

            Console.WriteLine("File does not exist");
            return null;
        }

        private Network ParseFileToNetwork(string pathToFile)
        {
            result = new Network();
            List<string> fileLines = File.ReadAllLines(pathToFile).ToList();
            AddNodesToNetwork(fileLines);
            AddNodesConnections(fileLines);
            AddLinksToNetwork();
            return result;
        }

        private void AddLinksToNetwork()
        {
            int currentLinkId = 1;
            foreach(Node currentNode in result.Nodes)
            {
                foreach(Node connectedToCurrentNode in currentNode.ConnectedNodes)
                {
                    if (!result.FindLinkByNodes(currentNode.NodeId, connectedToCurrentNode.NodeId)) 
                    {
                        result.Links.Add(new Link(currentLinkId, currentNode.NodeId, connectedToCurrentNode.NodeId));
                        currentLinkId++;
                    }
                }
            }
        }

        private void AddNodesConnections(List<string> fileLines)
        {
            foreach(string line in fileLines)
            {
                int currentNodeId = Int32.Parse(line.Split(':')[0]);
                Node currentNode = result.Nodes.Find(x => x.NodeId == currentNodeId);
                string[] connectedNodes = line.Split(':')[1].Split(',');
                foreach(string connectedNodeId in connectedNodes) 
                {
                    currentNode.ConnectedNodes.Add(result.Nodes.Find(x => x.NodeId == Int32.Parse(connectedNodeId)));
                }              
            }
        }

        private void AddNodesToNetwork(List<string> fileLines)
        {
            for(int i=0; i<fileLines.Count(); i++)
            {
                int nodeId = Int32.Parse(fileLines[i].Split(':')[0]);
                result.Nodes.Add(new Node(nodeId));
            }
        }
    }
}
