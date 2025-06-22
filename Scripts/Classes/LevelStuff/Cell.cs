using TPL.PVZR.ViewControllers.Entities.Plants;

namespace TPL.PVZR.Classes.LevelStuff
{
    /// <summary>
    /// 关卡中每个格子的数据结构
    /// </summary>
    public class Cell
    {
        public int x { get; }
        public int y { get; }

        public CellState CellState { get; set; } = CellState.NotSet;
        public Plant Plant { get; set; } = null;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public enum CellState
    {
        NotSet,
        Empty,
        Barrier,
        Dirt,
        Bound,
        Water,
        Ladder,
        HavePlant,
    }
}