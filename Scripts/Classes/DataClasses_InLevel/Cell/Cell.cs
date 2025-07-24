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

        Plat, // 平台 - Block | 花盆, 睡莲
        PlatOfNormal, // Normal植物能接受的平台 - Dirt | 花盆, 睡莲

        HasPlant, // 有植物 - N/A | 有
    }

    /// <summary>
    /// 关卡中每个格子的数据结构
    /// </summary>
    public class Cell
    {
        public Vector2Int Position;

        public TileState CellTileState { get; set; } = TileState.NotSet;
        public CellPlantData CellPlantData { get; } = new CellPlantData();

        public bool Is(CellTypeId cellTypeId)
        {
            return cellTypeId switch
            {
                CellTypeId.Empty => CellTileState == TileState.Empty && CellPlantData.IsEmpty(),
                CellTypeId.TileEmpty => CellTileState == TileState.Empty,
                CellTypeId.Block => CellTileState is TileState.Dirt or TileState.Barrier or TileState.Bound,
                CellTypeId.Climbable => CellTileState == TileState.Ladder,
                CellTypeId.Water => CellTileState == TileState.Water,
                CellTypeId.Plat => CellTileState is TileState.Dirt or TileState.Barrier or TileState.Bound ||
                                   CellPlantData.HasPlant(PlantId.Flowerpot),
                CellTypeId.PlatOfNormal => CellTileState == TileState.Dirt ||
                                           CellPlantData.HasPlant(PlantId.Flowerpot),
                CellTypeId.HasPlant => CellPlantData.HasPlant(),
                _ => false
            };
        }

        public Cell(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }
    }
}