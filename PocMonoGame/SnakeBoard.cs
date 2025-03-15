using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMonoGame
{
    internal class SnakeBoard
    {
        Random random;

        private int width;
        private int height;

        private List<(int x, int y)> snake = new();
        private Queue<(int x, int y)> path = new();
        private (int x, int y) food;
        private int bestScore;

        public SnakeBoard(int width, int height)
        {
            random = new Random();
            this.width = width;
            this.height = height;
            snake.Add((0, 0));
            food = (width / 2, height / 2);
            food = (width / 2, height / 2);
        }

        internal (int x, int y) GetFoodPos()
        {
            return food;
        }

        internal (int x, int y) GetSnakeHeadPos()
        {
            return snake.Last();
        }

        internal IEnumerable<(int x, int y)> GetTail()
        {
            return snake.Count > 1 ? snake.SkipLast(1) : [];
        }

        internal void Tick()
        {
            if(path.Count == 0)
            {
                food = GenerateRandomFoodPosition();
                GenerateNextPath();
                if(path.Count == 0)
                {
                    bestScore = Math.Max(bestScore, snake.Count - 1);
                    Reset();
                }
            }
            else if( snake.Count > 0)
            {
                snake.RemoveAt(0);
            }

            snake.Add(path.Dequeue());
        }

        internal string GetScoreString()
        {
            return $"Score: {snake.Count - 1}\nBest: {bestScore}";
        }

        private void Reset()
        {
            snake.Clear();
            path.Clear();
            path.Enqueue(food);
        }

        private (int, int) GenerateRandomFoodPosition()
        {
            (int, int) food;
            do
            {
                food = (random.Next(0, width), random.Next(0, height));
            } while(snake.Contains(food));
            return food;
        }

        private void GenerateNextPath()
        {
            var firstNode = GetSnakeHeadPos();

            PriorityQueue<(int x, int y), int> fronteir = new PriorityQueue<(int x, int y), int>();
            fronteir.Enqueue(firstNode, 0);

            Dictionary<(int x, int y), (int x, int y)> keyCameFromValue = new();
            keyCameFromValue.Add(firstNode, firstNode);

            Dictionary<(int x, int y), int> costSoFar = new();
            costSoFar.Add(firstNode, 0);

            while (fronteir.Count > 0)
            {
                var currentNode = fronteir.Dequeue();
                if (currentNode == food)
                    break;

                foreach (var direction in new (int alongX, int alongY)[] { (1, 0), (-1, 0), (0, 1), (0, -1)})
                {
                    (int x, int y) neighboor = (currentNode.x + direction.alongX, currentNode.y + direction.alongY);
                    if(neighboor.x < 0 || neighboor.x > width || neighboor.y < 0 || neighboor.y > height)
                        continue;

                    if (snake.Contains(neighboor))
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

                    var priority = newCost + GetHeuristicDistanceToGoal(neighboor);
                    fronteir.Enqueue(neighboor, priority);
                }
            }

            if (!keyCameFromValue.ContainsKey(food))
                return;

            BuildPath(keyCameFromValue, firstNode);
        }

        private void BuildPath(Dictionary<(int x, int y), (int x, int y)> keyCameFromValue, (int x, int y) startNode)
        {
            var current = food;
            while (current != startNode)
            {
                path.Enqueue(current);
                current = keyCameFromValue[current];
            }

            path.Enqueue(startNode);
            path = new Queue<(int x, int y)>(path.Reverse());
        }

        private int GetHeuristicDistanceToGoal((int x, int y) pos)
        {
            return Math.Abs(food.x - pos.x) + Math.Abs(food.y - pos.y);
        }
    }
}
