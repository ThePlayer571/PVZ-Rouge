using System.Diagnostics;
using System.Linq;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
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
    public struct OnTileChanged
    {
        public int x;
        public int y;
        public CellTileState OldState;
        public CellTileState NewState;
    }

    public interface ILevelGridModel : IModel
    {
        /// <summary>
        ///关卡地图最基本的数据结构
        /// 与Scene内的坐标|每个Tilemap的坐标对应
        /// </summary>
        Matrix<Cell> LevelMatrix { get; }

        EasyEvent<OnTileChanged> OnTileChanged { get; }

        // 修改方法
        void SetTile(int x, int y, CellTileState tileState);

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

        public EasyEvent<OnTileChanged> OnTileChanged { get; } = new();

        public void SetTile(int x, int y, CellTileState tileState)
        {
            var targetCell = LevelMatrix[x, y];
            if (targetCell.CellTileState != tileState)
            {
                var oldState = targetCell.CellTileState;
                targetCell.CellTileState = tileState;
                OnTileChanged.Trigger(new OnTileChanged { x = x, y = y, OldState = oldState, NewState = tileState });
            }
        }

        public bool IsValidPos(Vector2Int pos)
        {
            if (pos.x < 0 || pos.y < 0) return false;
            if (pos.y >= LevelMatrix.Columns) return false;
            if (pos.x >= LevelMatrix.Rows) return false;
            return true;
        }

        public bool CanSpawnPlantOn(Vector2Int pos, PlantDef def)
        {
            var unionConditionGroup = PlantConfigReader.GetAllowingPlantingLocations(def);

            if (!IsValidPos(pos) || !IsValidPos(pos.Down()) || !IsValidPos(pos.Up())) return false; // 超出地图
            var cell = LevelMatrix[pos.x, pos.y];
            var belowCell = LevelMatrix[pos.x, pos.y - 1];
            var aboveCell = LevelMatrix[pos.x, pos.y + 1];

            if (def.Id is PlantId.Flowerpot or PlantId.LilyPad && pos == Player.Instance.CellPos) return false;

            return unionConditionGroup.Any(condition => condition.CheckSpawn(def, cell, belowCell, aboveCell));
        }

        public bool CanStackPlantOn(Vector2Int pos, PlantDef def)
        {
            var unionConditionGroup = PlantConfigReader.GetAllowingPlantingLocations(def);

            if (!IsValidPos(pos) || !IsValidPos(pos.Down()) || !IsValidPos(pos.Up())) return false; // 超出地图
            var cell = LevelMatrix[pos.x, pos.y];
            var belowCell = LevelMatrix[pos.x, pos.y - 1];
            var aboveCell = LevelMatrix[pos.x, pos.y + 1];

            if (unionConditionGroup.Any(condition => condition.CheckStack(def, cell, belowCell, aboveCell)))
            {
                return true;
            }

            return false;
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.LevelMatrix = LevelMatrixHelper.BakeLevelMatrix(LevelTilemapNode.Instance, levelData);
            stopwatch.Stop();
            $"算法耗时：{stopwatch.ElapsedMilliseconds} ms".LogInfo();
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