using UnityEngine;

namespace TPL.PVZR.Core
{
    public enum Direction2
    {
        Left,
        Right
    }
    public enum Direction4
    {
        NotSet, Left, Right, Up, Down, 
    }

    public static class DirectionHelper
    {
        public static Direction2 Reverse(this Direction2 direction)
        {
            return direction is Direction2.Left ? Direction2.Right : Direction2.Left;
        }

        public static Vector2 ToVector2(this Direction2 direction)
        {
            return direction == Direction2.Right ? Vector2.right : Vector2.left;
        }
    }
}