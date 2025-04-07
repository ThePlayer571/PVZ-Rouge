using System;
using System.Collections.Generic;
using TPL.PVZR.Gameplay.Class.Levels;

namespace TPL.PVZR.Gameplay.Class.MazeMap
{public class Node : IEquatable<Node>
    {
        public Node(int i, int j)
        {
            this.mazePos.row = i;
            this.mazePos.col = j;
        }
        // 基础数据
        public int level = -1; // -1: not set; n: 这个节点在keyNode n~n+1 之间
        public bool isKeyNode = false;
        public MazePos mazePos;
        // 路径数据
        
        public HashSet<Node> fromKey = new();
        public HashSet<Node> toKey =new();

        public Dictionary<Node, List<Node>> toKeyRoute = new();
        //
        public HashSet<Node> fromNode =new(); // 需要自动去重
        public HashSet<Node> toNode = new();
        // 关卡数据
        public bool carrySpot = false;

        // 实现 IEquatable<Node>（基于 MazePos 判断相等性）
        public bool Equals(Node other)
        {
            if (other is null) return false;
            return this.mazePos == other.mazePos;
        }

        // 重写 object.Equals
        public override bool Equals(object obj) => Equals(obj as Node);

        // 重写 GetHashCode（基于 MazePos 的哈希）
        public override int GetHashCode() => mazePos.GetHashCode();

        // 重载 == 和 != 运算符（可选）
        public static bool operator ==(Node a, Node b) => a?.mazePos == b?.mazePos;
        public static bool operator !=(Node a, Node b) => !(a == b);
    }
}