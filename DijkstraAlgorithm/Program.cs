using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijkstraAlgorithm
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var graph = new Graph()
            {
                Nodes = new List<Node>()
                {
                    new Node("A"),new Node("B"),new Node("C"),new Node("D"),new Node("E")
                }
            };
            graph.AddLink("A", "B", 6);
            graph.AddLink("A", "D", 1);
            graph.AddLink("D", "B", 2);
            graph.AddLink("E", "B", 2);
            graph.AddLink("D", "E", 1);
            graph.AddLink("B", "C", 5);
            graph.AddLink("E", "C", 5);



            graph.Dij("A", "C");

            //graph.AddLink(graph["A"], graph["B"], 6);
            //graph.AddLink(graph["A"], graph["D"],)








            //var nodeE = graph["E"];
            //var nodeE = graph.Nodes.Where(node => node.Name == "E");
            ;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {

        }
    }

    public class Graph
    {
        //public List<Route> Routes { get; set; } = new List<Route>();

        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Route> Routes { get; set; } = new List<Route>();

        public void AddLink(string from, string to, double distance)
        {
            AddLink(from: this[from], to: this[to], distance: distance);
        }

        public void AddLink(Node from, Node to, double distance)
        {
            Routes.Add(new Route(from: from, to: to, distance: distance));
        }

        public double Dij(string from, string to)
        {

            var startNode = this[from];
            var endNode = this[to];
            List<Node> VisitedNodes = new List<Node>();
            List<Node> UnvisitedNodes = new List<Node>(this.Nodes);

            var shortestDistancesFromStartNode = new Dictionary<Node, TableRow>();


            UnvisitedNodes.ForEach((unvisitedNode) =>
            {
                if (unvisitedNode == startNode)
                {
                    shortestDistancesFromStartNode.Add(unvisitedNode, new TableRow(0, startNode));
                }
                else
                {
                    shortestDistancesFromStartNode.Add(unvisitedNode, new TableRow(double.MaxValue, startNode));
                }
            });





            return 0;
        }

        public Node this[string nodeName]
        {
            get
            {
                try
                {
                    return this.Nodes.Where(node => node.Name == nodeName).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot get node by this name!");
                }
            }
        }
    }

    public class TableRow
    {
        public TableRow(double distance, Node previousVertex)
        {
            ShortestDistance = distance;
            PreviousVertex = previousVertex;
        }
        public TableRow()
        {

        }

        public double ShortestDistance { get; set; }
        public Node PreviousVertex { get; set; }

        public override string ToString()
        {
            return String.Format("{0} from {1}", ShortestDistance, PreviousVertex.Name);
        }
    }


    public class Route
    {
        public Route()
        {

        }

        public Route(Node from, Node to, double distance)
        {
            From = from;
            To = to;
            Distance = distance;
        }

        public Node From { get; set; }
        public Node To { get; set; }
        public double Distance { get; set; }
    }



    public class Node
    {
        public Node(string name)
        {
            Name = name;
        }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

}
