using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PocMonoGame
{
    internal class Pathfinder
    {
        public IEnumerable<(int x, int y)> GeneratePathAstar((int x, int y) firstNode, (int x, int y) destinationNode, int boardWitdh, int boardHeight, Func<(int x, int y), bool> isObstacle)
        {
            PriorityQueue<(int x, int y), int> fronteir = new PriorityQueue<(int x, int y), int>();
            fronteir.Enqueue(firstNode, 0);

            Dictionary<(int x, int y), (int x, int y)> keyCameFromValue = new();
            keyCameFromValue.Add(firstNode, firstNode);

            Dictionary<(int x, int y), int> costSoFar = new();
            costSoFar.Add(firstNode, 0);

            while (fronteir.Count > 0)
            {
                var currentNode = fronteir.Dequeue();
                if (currentNode == destinationNode)
                    break;

                foreach (var direction in new (int alongX, int alongY)[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    (int x, int y) neighboor = (currentNode.x + direction.alongX, currentNode.y + direction.alongY);
                    
                    if (IsOutOfBounds(boardWitdh, boardHeight, neighboor))
                        continue;

                    if (isObstacle(neighboor))
                        continue;

                    var newCost = costSoFar[currentNode] + 1;
                    if (keyCameFromValue.ContainsKey(neighboor) && newCost >= costSoFar[neighboor])
                        continue;

                    if (costSoFar.ContainsKey(neighboor))
                    {
                        keyCameFromValue[neighboor] = currentNode;
                        costSoFar[neighboor] = newCost;
                    }
                    else
                    {
                        keyCameFromValue.Add(neighboor, currentNode);
                        costSoFar.Add(neighboor, newCost);
                    }

                    var priority = newCost + GetHeuristicDistanceToGoal(neighboor, destinationNode);
                    fronteir.Enqueue(neighboor, priority);
                }
            }

            if (!keyCameFromValue.ContainsKey(destinationNode))
                return [];

            return BuildPath(keyCameFromValue, firstNode, destinationNode);
        }

        private bool IsOutOfBounds(int boardWitdh, int boardHeight, (int x, int y) neighboor)
        {
            return neighboor.x < 0 || neighboor.x > boardWitdh || neighboor.y < 0 || neighboor.y > boardHeight;
        }

        private IEnumerable<(int x, int y)> BuildPath(Dictionary<(int x, int y), (int x, int y)> keyCameFromValue, (int x, int y) startNode, (int x, int y) destinationNode)
        {
            var path = new List<(int x, int y)>();
            var current = destinationNode;
            while (current != startNode)
            {
                path.Add(current);
                current = keyCameFromValue[current];
            }
            path.Add(startNode);
            path.Reverse();
            return path;
        }

        private int GetHeuristicDistanceToGoal((int x, int y) startNode, (int x, int y) destinationNode)
        {
            // Manhattan distance
            return Math.Abs(destinationNode.x - startNode.x) + Math.Abs(destinationNode.y - startNode.y);
        }
    }
}
