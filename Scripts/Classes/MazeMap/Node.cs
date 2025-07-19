using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Classes.MazeMap
{
    public class Node
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
    }

}