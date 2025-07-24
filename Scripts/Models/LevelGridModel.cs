using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.DataReader;
using TPL.PVZR.Helpers.New.Methods;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers;
using TPL.PVZR.ViewControllers.Entities.Plants.Base;
using TPL.PVZR.ViewControllers.Others.LevelScene;
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
        bool CanSpawnPlantOn(Vector2Int pos, PlantDef def);
        bool CanStackPlantOn(Vector2Int pos, PlantDef def);
        Cell GetCell(Vector2Int cellPos);
        Cell GetCell(Vector2 worldPos);

        // Methods
        void Initialize(ILevelData levelData);
        void Reset();
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

        public bool CanSpawnPlantOn(Vector2Int pos, PlantDef def)
        {
            var unionConditionGroup = PlantConfigReader.GetAllowedPlantingLocations(def);

            if (!IsValidPos(pos) || !IsValidPos(pos.Down())) return false; // 超出地图
            var cell = LevelMatrix[pos.x, pos.y];
            var belowCell = LevelMatrix[pos.x, pos.y - 1];

            return unionConditionGroup.Any(condition => condition.CheckSpawn(def, cell, belowCell));
        }

        public bool CanStackPlantOn(Vector2Int pos, PlantDef def)
        {
            var unionConditionGroup = PlantConfigReader.GetAllowedPlantingLocations(def);

            if (!IsValidPos(pos) || !IsValidPos(pos.Down())) return false; // 超出地图
            var cell = LevelMatrix[pos.x, pos.y];
            var belowCell = LevelMatrix[pos.x, pos.y - 1];

            //todo 不同植物叠种的支持
            if (unionConditionGroup.Any(condition => condition.CheckStack(def, cell, belowCell)))
            {
                return (cell.CellPlantData.GetPlant(def) as ICanBeStackedOn).CanStack(def);
            }
            else
            {
                return false;
            }
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
            this.LevelMatrix = LevelMatrixHelper.BakeLevelMatrix(LevelTilemapController.Instance, levelData);
            // LevelMatrixHelper.SetDebugTiles(LevelMatrix, ReferenceHelper.LevelTilemap.Debug);
        }

        public void Reset()
        {
            this.LevelMatrix = null;
        }

        protected override void OnInit()
        {
        }

        public Matrix<Cell> LevelMatrix { get; private set; }
    }
}