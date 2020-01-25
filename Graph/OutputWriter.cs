using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tools
{
    public class OutputWriter
    {
        private static StreamWriter CreateFile(string fileName)
        {
            bool freeNameNotFound = true;
            int currentFileNameNumber = 1;
            string baseFileName = fileName;
            string pathToFile, newFileName;
            do
            {
                newFileName = baseFileName + "_" + currentFileNameNumber;
                pathToFile = System.IO.Path.Combine(Environment.CurrentDirectory, newFileName);
                if (!File.Exists(pathToFile + ".txt"))
                {
                    freeNameNotFound = false;
                }
                currentFileNameNumber++;
            } 
            while (freeNameNotFound);
            Console.WriteLine("Saving output to: {0}.txt", newFileName);


            return File.CreateText(pathToFile + ".txt");
        }

        public void SaveOutputToTheFile(Network bestChromosomeOptimizationResult, List<PathAllocation> paths, string fileName)
        {
            using (StreamWriter fileStream = CreateFile(fileName))
            {
                fileStream.WriteLine("Solution for " + fileName + ".txt");
                fileStream.WriteLine("Used lambdas: {0}", bestChromosomeOptimizationResult.HighestLambdaId);
                fileStream.WriteLine("Defined demands: (Demand_ID:  First_Node_ID  Second_Node_ID)");
                foreach (PathAllocation pathAllocation in paths)
                {
                    fileStream.WriteLine(String.Format("{0}: {1} {2}", pathAllocation.Demand.DemandId, pathAllocation.Demand.StartNode.NodeId, pathAllocation.Demand.EndNode.NodeId));
                }
                fileStream.WriteLine();
                fileStream.WriteLine("Links defined in network: (Link_ID: Start_Node - End_Node)");
                bestChromosomeOptimizationResult.Links.OrderBy(x => x.LinkId);
                foreach(Link link1 in bestChromosomeOptimizationResult.Links)
                {
                    fileStream.WriteLine(String.Format("{0}: {1}-{2}", link1.LinkId, link1.ConnectedByLink[0], link1.ConnectedByLink[1]));
                }
                fileStream.WriteLine();
                fileStream.WriteLine("Chosen paths for each demand: (Demand_ID: <list of links>)");
                foreach (PathAllocation pathAllocation1 in paths)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Link link in pathAllocation1.ChosenPath.PathLinks)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(link.LinkId);
                    }
                    fileStream.WriteLine(String.Format("{0}:  {1}", pathAllocation1.Demand.DemandId, sb.ToString()));
                }
                fileStream.WriteLine();
                fileStream.WriteLine("Easier to imagine version: (Start_node -> End_Node: *Lambda_ID* <list of nodes on path>)");
                foreach (PathAllocation pathAllocation2 in paths)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Node node in pathAllocation2.ChosenPath.PathNodes)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(" -> ");
                        }
                        sb.Append(node.NodeId);
                    }
                    fileStream.WriteLine(String.Format("{0} -> {1}: *{2}* {3}", pathAllocation2.Demand.StartNode.NodeId, pathAllocation2.Demand.EndNode.NodeId, pathAllocation2.LambdaId, sb.ToString()));
                }
                fileStream.WriteLine();
                fileStream.WriteLine("Links load (Link_ID:  <list of lambdas>)");
                foreach (Link link in bestChromosomeOptimizationResult.Links)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (int lambdaId in link.LambdasIds)
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(" ");
                        }
                        sb.Append(lambdaId);
                    }
                    fileStream.WriteLine(String.Format("{0}: {1}", link.LinkId, sb.ToString()));
                }
            }

        }
    }
}
