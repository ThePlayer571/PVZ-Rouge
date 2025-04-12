using System;
using System.Collections.Generic;

namespace TPL.PVZR.Gameplay.Class.MazeMap.Core
{public class Node : IEquatable<Node>
    {
        public Node(int i, int j)
        {
            this.mazePos.row = i;
            this.mazePos.col = j;
            //
            this.id = newIdShouldBe;
            newIdShouldBe++;
        }
        // 基础数据
        public int level = -1; // -1: not set; n: 这个节点在keyNode n~n+1 之间
        public bool isKeyNode = false;
        public MazePos mazePos;


        /// <summary>
        /// 节点的唯一标识符，原则上只在持久化存储时使用，其余时刻应使用Node记录Node
        /// </summary>
        /// <remarks>1: 未设置; 1..: 正常id</remarks>>
        private static int newIdShouldBe = 1;
        public static void ResetNodeId()
        {
            newIdShouldBe = 1;
        }
        public int id { get; private set; } = -1;
        // 路径数据
        
        public HashSet<Node> fromKey = new();
        public HashSet<Node> toKey =new();

        public Dictionary<Node, List<Node>> toKeyRoute = new();
        //
        public HashSet<Node> fromNode =new(); // 需要自动去重
        public HashSet<Node> toNode = new();
        // 关卡数据
        public bool carrySpot = false;

        #region 接口实现

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
        

        #endregion
    }
}