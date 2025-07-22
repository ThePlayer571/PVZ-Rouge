using System.Collections;
using System.Collections.Generic;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using UnityEngine;

namespace TPL.PVZR.Classes.DataClasses_InLevel
{
    /// <summary>
    /// 以植物为第一人称，描述“我可以种在哪里”
    /// </summary>
    public enum AllowedPlantingLocation
    {
        OnDirt,
        OnPlat,
        OnSamePlantAndDirt,
    }

    public enum PlacementSlotInCell
    {
        Normal,
        Overlay,
        Air,
        Behind,
    }


    /// <summary>
    /// 关卡中每个格子的数据结构
    /// </summary>
    public class Cell
    {
        public Vector2Int Position;

        public TileState TileState { get; set; } = TileState.NotSet;
        public PlantInfo CellPlantInfo { get; } = new PlantInfo();

        public void SetPlant(Plant plant, PlacementSlotInCell placement)
        {
            switch (placement)
            {
                case PlacementSlotInCell.Normal:
                    CellPlantInfo.Normal = plant;
                    break;
                case PlacementSlotInCell.Overlay:
                    CellPlantInfo.Overlay = plant;
                    break;
                case PlacementSlotInCell.Air:
                    CellPlantInfo.Air = plant;
                    break;
                case PlacementSlotInCell.Behind:
                    CellPlantInfo.Behind = plant;
                    break;
            }
        }

        public class PlantInfo : IEnumerable<Plant>
        {
            public Plant Normal { get; set; }
            public Plant Overlay { get; set; }
            public Plant Air { get; set; }
            public Plant Behind { get; set; }

            public bool IsEmpty => Normal == null && Overlay == null && Air == null && Behind == null;


            public IEnumerator<Plant> GetEnumerator()
            {
                if (Normal != null) yield return Normal;
                if (Overlay != null) yield return Overlay;
                if (Air != null) yield return Air;
                if (Behind != null) yield return Behind;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


        public bool IsEmpty => TileState is TileState.Empty && CellPlantInfo.IsEmpty;

        public bool IsNormalGrowable => // 可以种植物的平台 - 豌豆射手可以放置其上
            TileState == TileState.Dirt ||
            (CellPlantInfo.Normal != null && CellPlantInfo.Normal.Def.Id is PlantId.Flowerpot);

        public bool IsPlat => // 平台 - 花盆可以放置其上
            TileState is TileState.Barrier or TileState.Dirt ||
            (CellPlantInfo.Normal != null && CellPlantInfo.Normal.Def.Id is PlantId.Flowerpot);

        public bool IsClimbable => // 可以攀爬 - 梯子
            TileState is TileState.Ladder;

        public bool IsBlock => // 地图中存在的墙 - 包括 Dirt, Barrier..., 不包括 Water, Ladder...
            TileState is TileState.Barrier or TileState.Bound or TileState.Dirt;


        public Cell(int x, int y)
        {
            Position = new Vector2Int(x, y);
        }
    }

    public enum TileState
    {
        NotSet,
        Empty,
        Barrier,
        Dirt,
        Bound,
        Water,
        Ladder,
    }
}