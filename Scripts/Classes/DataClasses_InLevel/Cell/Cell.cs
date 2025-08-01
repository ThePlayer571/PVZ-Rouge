using TPL.PVZR.Classes.InfoClasses;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses_InLevel
{
    public enum CellTypeId
    {
        // 描述 - Tile状态 | Plant状态

        Empty, // 空格子 - Empty | 无

        TileEmpty, // 无Tile - Empty | N/A
        Block, // 有实体Tile - Dirt, Barrier, Bound | N/A
        Climbable, // 可攀爬 - Ladder | <待定义>
        Water, // 水 - Water | N/A
        Gravestone, // 墓碑 - Gravestone | N/A

        Plat, // 平台 - Block | 花盆, 睡莲
        PlatOfNormal, // Normal植物能接受的平台 - Dirt | 花盆, 睡莲

        HasPlant, // 有植物 - N/A | 有
        SoftObstacle // 装饰 -  | 无
    }

    /// <summary>
    /// 关卡中每个格子的数据结构
    /// </summary>
    public class Cell
    {
        public Vector2Int Position;

        public CellTileState CellTileState { get; set; } = CellTileState.NotSet;
        public CellPlantData CellPlantData { get; } = new CellPlantData();

        public bool Is(CellTypeId cellTypeId)
        {
            return cellTypeId switch
            {
                CellTypeId.Empty => CellTileState == CellTileState.Empty && CellPlantData.IsEmpty(),
                CellTypeId.TileEmpty => CellTileState == CellTileState.Empty,
                CellTypeId.Block => CellTileState is CellTileState.Dirt or CellTileState.Barrier or CellTileState.Bound,
                CellTypeId.Climbable => CellTileState == CellTileState.Ladder,
                CellTypeId.Water => CellTileState == CellTileState.Water,
                CellTypeId.Plat => CellTileState is CellTileState.Dirt or CellTileState.Barrier or CellTileState.Bound ||
                                   CellPlantData.HasPlant(PlantId.Flowerpot) ||
                                   CellPlantData.HasPlant(PlantId.LilyPad),
                CellTypeId.PlatOfNormal => CellTileState == CellTileState.Dirt ||
                                           CellPlantData.HasPlant(PlantId.Flowerpot) ||
                                           CellPlantData.HasPlant(PlantId.LilyPad),
                CellTypeId.HasPlant => CellPlantData.HasPlant(),
                CellTypeId.SoftObstacle => CellTileState == CellTileState.SoftObstacle,
                _ => false
            };
        }

        public Cell(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }
    }
}