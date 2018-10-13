using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijkstraAlgorithm
{
    [TestClass]
    public class MyTestClass
    {
        /// <summary>
        /// Implement of this video https://www.youtube.com/watch?v=pVfj6mxhdMw
        /// </summary>
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
            var result = graph.Dij("A", "C");
            result.Routes.ForEach((route) =>
            {
                Trace.WriteLine(String.Format("{0} -> {1} , {2}", route.From.Name, route.To.Name, route.Distance));
            });
            Trace.WriteLine(String.Format("Total Distance: {0}", result.TotalDistance));
            ;
        }
    }

    class Program
    {
        static void Main(string[] args)
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
            var result = graph.Dij("A", "C");
            result.Routes.ForEach((route) =>
            {
                Console.WriteLine(String.Format("{0} -> {1} , {2}", route.From.Name, route.To.Name, route.Distance));
            });
            Console.WriteLine(String.Format("Total Distance: {0}", result.TotalDistance));
            Console.ReadLine();
        }
    }

    public class Graph
    {

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

        public Route GetLink(Node node1, Node node2)
        {
            return Routes.Find((route) =>
            {
                return (route.From == node1 && route.To == node2) || (route.From == node2 && route.To == node1);
            });
        }

        public DijResult Dij(string from, string to)
        {
            var startNode = this[from];
            var endNode = this[to];
            List<Node> visitedNodes = new List<Node>();
            List<Node> unvisitedNodes = new List<Node>(this.Nodes);
            var shortestDistancesFromStartNode = new ShortestPathsTable();
            unvisitedNodes.ForEach((unvisitedNode) =>
            {
                if (unvisitedNode == startNode)
                {
                    shortestDistancesFromStartNode.Add(unvisitedNode, new ShortestPathsTableRow(0, startNode));
                }
                else
                {
                    shortestDistancesFromStartNode.Add(unvisitedNode, new ShortestPathsTableRow(double.MaxValue, null));
                }
            });
            var visitingNode = startNode;
            var routes = RoutesContainGivenNode(startNode);
            routes.ForEach((route) =>
            {
                var otherNode = route.OtherNode(visitingNode);
                var tableRow = shortestDistancesFromStartNode[otherNode];
                var oldDistance = tableRow.ShortestDistance;
                var newDistance = route.Distance;
                if (newDistance < oldDistance)
                {
                    tableRow.ShortestDistance = newDistance;
                    tableRow.PreviousVertex = visitingNode;
                }
            });
            unvisitedNodes.Remove(visitingNode);
            visitedNodes.Add(visitingNode);

            while (unvisitedNodes.Count != 0)
            {
                var min = double.MaxValue;
                Node closestNodeUnvisited = null;
                unvisitedNodes.ForEach((node) =>
                {
                    var current = shortestDistancesFromStartNode[node].ShortestDistance;
                    if (current < min)
                    {
                        min = current;
                        closestNodeUnvisited = node;
                    }
                });
                visitingNode = closestNodeUnvisited;
                var routesInLoop = RoutesContainGivenNode(visitingNode, visitedNodes);
                routesInLoop.ForEach((route) =>
                {
                    var otherNode = route.OtherNode(visitingNode);
                    var tableRow = shortestDistancesFromStartNode[otherNode];
                    var oldDistance = tableRow.ShortestDistance;
                    var newDistance = route.Distance + shortestDistancesFromStartNode[visitingNode].ShortestDistance;
                    if (newDistance < oldDistance)
                    {
                        tableRow.ShortestDistance = newDistance;
                        tableRow.PreviousVertex = visitingNode;
                    }
                    ;
                });
                unvisitedNodes.Remove(visitingNode);
                visitedNodes.Add(visitingNode);
                ;
            }
            var dijResult = new DijResult();
            var dijResultRoutes = dijResult.Routes;
            var routeQuene = new Queue<Route>();
            var result = shortestDistancesFromStartNode.GetDirResult(startNode: startNode, endNode: endNode, graph: this);
            result.Graph = this;
            return result;
        }


        public List<Route> RoutesContainGivenNode(Node givenNode, IEnumerable<Node> exceptNodes)
        {
            var routes = RoutesContainGivenNode(givenNode);
            var processedRoutes = routes.Where((route) =>
            {
                var twoNode = new List<Node>() { route.From, route.To };
                var intersetionNodes = twoNode.Intersect(exceptNodes).ToList();
                ;
                var count = intersetionNodes.Count;
                return count == 0;
            });
            return processedRoutes.ToList();
        }

        public List<Route> RoutesContainGivenNode(Node givenNode)
        {
            return Routes.Where((route) =>
            {
                return (route.From == givenNode || route.To == givenNode);
            }).ToList();
        }



        public Node this[string nodeName]
        {
            get
            {
                try
                {
                    return this.Nodes.Where(node => node.Name == nodeName).FirstOrDefault();
                }
                catch (Exception )
                {
                    throw new Exception("Cannot get node by this name!");
                }
            }
        }
    }


    public class ShortestPathsTable : Dictionary<Node, ShortestPathsTableRow>
    {
        public Node StartNode { get; set; }

        public DijResult GetDirResult(Node startNode, Node endNode, Graph graph)
        {
            var result = new DijResult();
            var routes = result.Routes = new List<Route>();
            Node tempStartNode = null;
            Node tempEndNode = endNode;
            while (tempStartNode != startNode)
            {

                //var row = this[endNode]
                tempStartNode = this[tempEndNode].PreviousVertex;
                var route = graph.GetLink(tempStartNode, tempEndNode);
                routes.Add(new Route(from: tempStartNode, to: tempEndNode, distance: route.Distance));
                tempEndNode = tempStartNode;
            }
            routes.Reverse();
            return result;
        }
    }

    public class ShortestPathsTableRow
    {
        public ShortestPathsTableRow(double distance, Node previousVertex)
        {
            ShortestDistance = distance;
            PreviousVertex = previousVertex;
        }
        public ShortestPathsTableRow()
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

        public bool Has(Node node)
        {
            return (From == node || To == node);
        }

        public Node OtherNode(Node inputNode)
        {
            if (inputNode != From && inputNode != To)
            {
                throw new Exception("The node input does not exist in this route!");
            }
            return inputNode == From ? To : From;
        }

        public Node From { get; set; }
        public Node To { get; set; }
        public double Distance { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", From.Name, To.Name, Distance);
        }
    }

    public class DijResult
    {
        public Graph Graph { get; set; }
        public double TotalDistance => Routes.Sum(route => route.Distance);
        public List<Route> Routes { get; set; }
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
