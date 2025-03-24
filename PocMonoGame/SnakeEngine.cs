using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocMonoGame
{
    internal class SnakeEngine
    {
        Random random;

        private int width;
        private int height;

        private List<(int x, int y)> snake = new();
        private Queue<(int x, int y)> pathToFood = new();
        private (int x, int y) food;
        private int bestScore;
        // Set this to 0 to disable the rule where the food tries to spawn in a reachable position
        private const int MAX_RETRIES_FOR_NEXT_FOOD = 100;
        private const bool ENABLE_AGGRESSIVE_PATH_RECALCULATION = true;

        public SnakeEngine(int width, int height)
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
            if(pathToFood.Count == 0)
            {
                GenerateNextFoodAndPathToIt();
            }
            else if( snake.Count > 0)
            {
                snake.RemoveAt(0);
                if(ENABLE_AGGRESSIVE_PATH_RECALCULATION)
                {
                    AggressivePathRecalculation();
                }
            }

            snake.Add(pathToFood.Dequeue());
        }

        private void AggressivePathRecalculation()
        {
            var tickOptimalPath = new Queue<(int x, int y)>(new Pathfinder().GeneratePathAstar(
                GetSnakeHeadPos(),
                food,
                width,
                height,
                snake.Contains
            ));
            if (tickOptimalPath.Count + 1 < pathToFood.Count)
                pathToFood = tickOptimalPath;
        }

        private void GenerateNextFoodAndPathToIt()
        {
            int nbRetry = 0;
            do
            { 
                food = GenerateRandomFoodPosition();
                pathToFood = new Queue<(int x, int y)>(new Pathfinder().GeneratePathAstar(
                    GetSnakeHeadPos(),
                    food,
                    width,
                    height,
                    snake.Contains
                ));
                nbRetry++;
            }
            while(pathToFood.Count == 0 && nbRetry < MAX_RETRIES_FOR_NEXT_FOOD);

            if (pathToFood.Count == 0)
            {
                bestScore = Math.Max(bestScore, snake.Count - 1);
                Reset();
            }
        }

        internal string GetScoreString()
        {
            return $"Score: {snake.Count - 1}\nBest: {bestScore}";
        }

        private void Reset()
        {
            snake.Clear();
            pathToFood.Clear();
            pathToFood.Enqueue(food);
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
