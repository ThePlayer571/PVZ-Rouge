namespace TPL.PVZR.Gameplay.Class.ZombieAI
{
    public enum AllowPassHeight
    {
        NotSet, One, TwoAndMore
    }
    public class Vertex
    {
        #region 构造函数

        public Vertex(int x,int y,AllowPassHeight allowPassHeight, bool isKey = false)
        {
            this.x = x;
            this.y = y;
            this.allowPassHeight = allowPassHeight;
            this.isKey = isKey;
        }

        #endregion

        public int x;
        public int y;
        /// <summary>
        /// Vertex之上有的空间（能允许几格高的僵尸通过）
        /// </summary>
        public AllowPassHeight allowPassHeight;
        /// <summary>
        /// 是否是关键结点
        /// </summary>
        /// <remarks>定义：两个邻接关键结点之间的路，可以用同一种移动方式通过</remarks>>
        public bool isKey;
    }
}