using System;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Helpers.Methods;
using TPL.PVZR.Tools;
using UnityEngine;

namespace TPL.PVZR.Models
{
    public interface ILevelGridModel : IModel
    {
        /// <summary>
        ///关卡地图最基本的数据结构
        /// 与Scene内的坐标|每个Tilemap的坐标对应
        /// </summary>
        Matrix<Cell> LevelMatrix { get; }

        // 实用方法
        bool IsValidPos(Vector2Int pos);
        bool CanSpawnPlantOn(Vector2Int pos, PlantId id);
        Cell GetCell(Vector2Int cellPos);
        Cell GetCell(Vector2 worldPos);

        // Methods
        void Initialize(ILevelData levelData);
    }

    public class LevelGridModel : AbstractModel, ILevelGridModel
    {
        #region 实用方法

        public bool IsValidPos(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0) return false;
            if (pos.y >= LevelMatrix.Columns) return false;
            if (pos.x >= LevelMatrix.Rows) return false;
            return true;
        }

        public bool CanSpawnPlantOn(Vector2Int pos, PlantId id)
        {
            if (id == PlantId.Flowerpot) // 花盆
            {
                if (!IsValidPos(pos) || !IsValidPos(pos.Down())) return false;

                var cell = LevelMatrix[pos.x, pos.y];
                var belowCell = LevelMatrix[pos.x, pos.y - 1];
                return cell.IsEmpty && belowCell.IsPlat;
            }
            else
            {
                if (!IsValidPos(pos) || !IsValidPos(pos.Down())) return false;

                var cell = LevelMatrix[pos.x, pos.y];
                var belowCell = LevelMatrix[pos.x, pos.y - 1];
                if (cell == null) throw new Exception($"调用CanSpawnPlantOn时，cell为null，pos: {pos}");
                if (belowCell == null) throw new Exception($"调用CanSpawnPlantOn时，belowCell为null，pos: {pos}.Down");
                return cell.IsEmpty && belowCell.IsDirtPlat;
            }

            throw new ArgumentException($"出现未考虑的plantId: {id}");
        }

        public Cell GetCell(Vector2Int cellPos)
        {
            return LevelMatrix[cellPos.x, cellPos.y];
        }

        public Cell GetCell(Vector2 worldPos)
        {
            var cellPos = LevelGridHelper.WorldToCell(worldPos);
            return LevelMatrix[cellPos.x, cellPos.y];
        }

        #endregion

        public void Initialize(ILevelData levelData)
        {
            this.LevelMatrix = LevelMatrixHelper.BakeLevelMatrix(ReferenceHelper.LevelTilemap, levelData);
            // LevelMatrixHelper.SetDebugTiles(LevelMatrix, ReferenceHelper.LevelTilemap.Debug);
        }

        protected override void OnInit()
        {
        }

        public Matrix<Cell> LevelMatrix { get; private set; }
    }
}