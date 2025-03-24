using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeMonoGame
{
    internal static class Extensions
    {
        public static Vector2 ToVector(this (int x, int y) pos)
        {
            return new Vector2(pos.x, pos.y);
        }

        public static Vector2 Scale(this Vector2 vec, float scale)
        {
            return new Vector2(vec.X * scale, vec.Y * scale);
        }
    }
}
