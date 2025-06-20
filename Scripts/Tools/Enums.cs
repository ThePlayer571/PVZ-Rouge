using UnityEngine;

namespace TPL.PVZR.Core
{
    public enum Direction2
    {
        Left,
        Right
    }

    public static class Direction2Extensions
    {
        public static Vector2 ToVector2(this Direction2 direction)
        {
            return direction == Direction2.Left ? Vector2.left : Vector2.right;
        }
    }
}