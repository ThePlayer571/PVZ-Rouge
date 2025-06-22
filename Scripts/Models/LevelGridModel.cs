using System;
using QFramework;
using TPL.PVZR.Classes.GameStuff;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.Helpers.Methods;
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

        // Methods
        void Initialize(ILevelData levelData);
    }

    public class LevelGridModel : AbstractModel, ILevelGridModel
    {
        public bool IsValidPos(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0) return false;
            if (pos.y >= LevelMatrix.Rows) return false;
            if (pos.x >= LevelMatrix.Columns) return false;
            return true;
        }

        public bool CanSpawnPlantOn(Vector2Int pos, PlantId id)
        {
            if (id == PlantId.Flowerpot) // 花盆
            {
                if (!IsValidPos(pos) && !IsValidPos(pos.Down())) return false;

                var cell = LevelMatrix[pos.x, pos.y];
                var belowCell = LevelMatrix[pos.x, pos.y - 1];
                return cell.IsEmpty && belowCell.IsPlat;
            }
            else // 普通植物(豌豆射手)
            {
            }

            throw new ArgumentException($"出现未考虑的plantId: {id}");
        }

        public void Initialize(ILevelData levelData)
        {
            this.LevelMatrix = LevelMatrixHelper.BakeLevelMatrix(ReferenceHelper.LevelTilemap, levelData);
            LevelMatrixHelper.SetDebugTiles(LevelMatrix, ReferenceHelper.LevelTilemap.Debug);
        }

        protected override void OnInit()
        {
        }

        public Matrix<Cell> LevelMatrix { get; private set; }
    }
}