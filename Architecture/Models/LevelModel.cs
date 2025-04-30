using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using QFramework;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.Items;
using TPL.PVZR.Gameplay.Class.Levels;
using UnityEngine;

namespace TPL.PVZR.Architecture.Models
{
    public interface ILevelModel : IModel
    {
        /// <summary>
        /// 当前正在运行的关卡
        /// </summary>
        /// <remarks>当且仅当LevelInitializationEarly阶段更新（所以说"当前正在运行"的说法并不准确）</remarks>>
        public ILevel currentLevel { get; }

        public void SetCurrentLevel(ILevel level);
        public MapConfig MapConfig { get; }
        public WaveConfig WaveConfig { get; }
        public ZombieSpawnConfig ZombieSpawnConfig { get; }
        public LootConfig LootConfig { get; }


        // 选卡相关
        // TODO:临时放在这里↓
        public int maxCardCount { get; }
        public List<Card> chosenCards { get; }
        public void SetChosenCards(List<Card> newChosenCards);

        // Gameplay
        public Matrix<Cell> CellGrid { get; }
        public BindableProperty<int> sunpoint { get; }

        // 方法
        void OnLevelInitialization(ILevel levelToEnter);
        void OnLevelExiting();
    }

    public class LevelModel : AbstractModel, ILevelModel
    {
        #region ILevelModel

        #region 数据

        // LevelConfig
        public ILevel currentLevel { get; private set; } = null;

        public MapConfig MapConfig { get; private set; } = null;
        public WaveConfig WaveConfig { get; private set; } = null;
        public ZombieSpawnConfig ZombieSpawnConfig { get; private set; } = null;

        public LootConfig LootConfig { get; private set; } = null;

        // 选卡相关
        public int maxCardCount { get; private set; } = 4;

        public List<Card> chosenCards { get; private set; } = new();

        // Gameplay
        public BindableProperty<int> sunpoint { get; private set; } = null;
        public Matrix<Cell> CellGrid { get; private set; } = null;

        #endregion

        #region 设置数据

        public void SetChosenCards(List<Card> newChosenCards)
        {
            chosenCards = newChosenCards.ToList();
        }

        public void SetCurrentLevel(ILevel level)
        {
            this.currentLevel = level;
            this.WaveConfig = level.WaveConfig;
            this.MapConfig = level.MapConfig;
            this.ZombieSpawnConfig = level.ZombieSpawnConfig;
            this.LootConfig = level.LootConfig;
        }

        #endregion

        #region 初始化相关

        public void OnLevelInitialization(ILevel levelToEnter)
        {
            CellGrid = new Matrix<Cell>(levelToEnter.MapConfig.size.x, levelToEnter.MapConfig.size.y);
            sunpoint.Value = 11451400;
            chosenCards = new List<Card>();
            // 网格数据
            var GroundTilemap = ReferenceModel.Get.TilemapGroup.Ground;
            var BoundTilemap = ReferenceModel.Get.TilemapGroup.Bound;
            var DirtTilemap = ReferenceModel.Get.TilemapGroup.DirtNotice;
            for (int x = 0; x < MapConfig.size.x; ++x)
            {
                for (int y = 0; y < MapConfig.size.y; ++y)
                {
                    // 读取场景中的Tile数据，生成CellGrid
                    SetCellFromTilemapToCellGrid(x, y);
                }
            }

            return;

            void SetCellFromTilemapToCellGrid(int x, int y)
            {
                if (GroundTilemap.HasTile(new Vector3Int(x, y, 0)) ||
                    BoundTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    if (DirtTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        CellGrid[x, y] = new Cell(Cell.CellState.HaveDirt);
                    }
                    else
                    {
                        CellGrid[x, y] = new Cell(Cell.CellState.HaveStone);
                    }
                }
                else
                {
                    CellGrid[x, y] = new Cell(Cell.CellState.Empty);
                }
            }
        }

        public void OnLevelExiting()
        {
            CellGrid = null;
            // ↓不能换引用，不然已经Register了的东西会出错
            // sunpoint = new BindableProperty<int>();
            chosenCards.Clear();

            currentLevel = null;
            MapConfig = null;
            WaveConfig = null;
            ZombieSpawnConfig = null;
            LootConfig = null;
        }

        #endregion

        #endregion

        protected override void OnInit()
        {
            sunpoint = new BindableProperty<int>();
            CellGrid = null;
        }
    }
}