namespace TPL.PVZR.Classes.LevelStuff
{
    /// <summary>
    /// 关卡中的每个格子
    /// </summary>
    public class Cell
    {
        public int x { get; }
        public int y { get; }
        
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}