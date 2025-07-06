using TPL.PVZR.ViewControllers.Entities.Plants;

namespace TPL.PVZR.Classes.DataClasses
{
    /// <summary>
    /// 关卡中每个格子的数据结构
    /// </summary>
    public class Cell
    {
        public int x { get; }
        public int y { get; }

        public CellTileState CellTileState { get; set; } = CellTileState.NotSet;
        public CellPlantState CellPlantState { get; set; } = CellPlantState.Empty;
        public Plant Plant { get; set; } = null;

        public bool IsEmpty => CellTileState is CellTileState.Empty && CellPlantState is CellPlantState.Empty;

        public bool IsDirtPlat => // 可以种植物的平台
            CellTileState == CellTileState.Dirt ||
            (CellPlantState == CellPlantState.HavePlant && Plant.Id == PlantId.Flowerpot);

        public bool IsPlat => // 可以站的平台
            CellTileState is CellTileState.Barrier or CellTileState.Barrier or CellTileState.Dirt ||
            (CellPlantState is CellPlantState.HavePlant && Plant.Id is PlantId.Flowerpot);

        public bool IsClimbable => // 可以攀爬的平台
            CellTileState is CellTileState.Ladder;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public enum CellTileState
    {
        NotSet,
        Empty,
        Barrier,
        Dirt,
        Bound,
        Water,
        Ladder,
    }

    public enum CellPlantState
    {
        Empty,
        HavePlant,
        Locked,
    }
}