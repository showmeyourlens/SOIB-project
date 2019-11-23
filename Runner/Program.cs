using FileParser;
using PathFinder;
using System;
using Tools;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            InputFileParser parser = new InputFileParser();
            Console.WriteLine("Choose network to optimize:");
            Console.WriteLine("1: Network_1.txt");
            Console.WriteLine("2: Network_2.txt");
            string choice = Console.ReadLine();
            Network inputNetwork = new Network();
            if (choice.Equals("1") || choice.Equals("2"))
            {
                inputNetwork = parser.ReadNetwork("Network_" + choice + ".txt");
            }

            AllPathFinder pathFinder = new AllPathFinder();
            pathFinder.FindAllPaths();

        }
    }
}
