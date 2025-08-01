using UnityEngine;

namespace TPL.PVZR.Tools
{
    public enum Direction2
    {
        Left,
        Right
    }

    public static class Direction2Extensions
    {
        public static Direction2 Reverse(this Direction2 direction)
        {
            return direction == Direction2.Left ? Direction2.Right : Direction2.Left;
        }
        
        public static Vector2 ToVector2(this Direction2 direction)
        {
            return direction == Direction2.Left ? Vector2.left : Vector2.right;
        }

        public static int ToInt(this Direction2 direction)
        {
            return direction == Direction2.Left ? -1 : 1;
        }
    }
}