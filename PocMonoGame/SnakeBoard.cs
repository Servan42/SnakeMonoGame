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
                path = new Queue<(int x, int y)>(new Pathfinder().GeneratePathAstar(
                    GetSnakeHeadPos(),
                    food,
                    width,
                    height,
                    snake.Contains
                ));

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
    }
}
