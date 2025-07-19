using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public class Node : IEquatable<Node>
    {
        public int x;
        public int y;
        public bool isKey;
        public Vector2Int Position => new Vector2Int(x, y);
        /// <summary>
        /// 历史遗留问题：应该叫stage
        /// </summary>
        public int level;
        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.level = -1; // 默认值为 -1，表示非关键节点
        }


        public bool Equals(Node other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Node otherNode)
            {
                return Equals(otherNode);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
        
        public static bool operator ==(Node left, Node right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !(left == right);
        }
    }

}