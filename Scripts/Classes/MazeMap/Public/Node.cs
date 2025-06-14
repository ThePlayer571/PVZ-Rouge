namespace TPL.PVZR.Classes.MazeMap
{
    public class Node
    {
        public int x;
        public int y;
        public bool isKey;
        public int level;
        public NodeConnection connections;

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.level = -1; // 默认值为 -1，表示非关键节点
            this.connections = new NodeConnection(); // 初始化连接关系
        }
    }

    /// <summary>
    /// 表示节点的连接关系
    /// </summary>
    public class NodeConnection
    {
        public bool Xp { get; set; } // 是否连接到X+
        public bool Xn { get; set; } // 是否连接到X-
        public bool Yp { get; set; } // 是否连接到Y+
        public bool Yn { get; set; } // 是否连接到Y-
    }
}