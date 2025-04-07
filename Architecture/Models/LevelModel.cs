using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.Items;
using TPL.PVZR.Gameplay.Class.Levels;
using UnityEngine;

namespace TPL.PVZR.Architecture.Models
{
    public interface ILevelModel : IModel
    {
        public ILevel level { get; }
        public MapConfig MapConfig { get; }
        public WaveConfig WaveConfig { get; }
        public ZombieSpawnConfig ZombieSpawnConfig { get; }
        public LootConfig LootConfig { get; }

        public void SetLevel(ILevel level);

        // 选卡相关
        // TODO:临时放在这里↓
        public int maxCardCount { get; }
        public List<Card> chosenCards { get; }
        public void SetChosenCards(List<Card> newChosenCards);

        // Gameplay
        public DynaGrid<Cell> CellGrid { get; }
        public BindableProperty<int> sunpoint { get; }

        // 方法
        void OnLevelInitialization();
        void OnExiting();
    }

    public class LevelModel : AbstractModel, ILevelModel
    {
        #region ILevelModel

        #region 数据

        // LevelConfig
        public ILevel level { get; private set; } = null;

        public MapConfig MapConfig { get; private set; } = null;
        public WaveConfig WaveConfig { get; private set; } = null;
        public ZombieSpawnConfig ZombieSpawnConfig { get; private set; } = null;

        public LootConfig LootConfig { get; private set; } = null;

        // 选卡相关
        public int maxCardCount { get; private set; } = 4;

        public List<Card> chosenCards { get; private set; } = new();

        // Gameplay
        public BindableProperty<int> sunpoint { get; private set; } = null;
        public DynaGrid<Cell> CellGrid { get; private set; } = null;

        #endregion

        #region 设置数据

        public void SetChosenCards(List<Card> newChosenCards)
        {
            chosenCards = newChosenCards.ToList();
        }
        public void SetLevel(ILevel level)
        {
            this.level = level;
            this.WaveConfig = level.WaveConfig;
            this.MapConfig = level.MapConfig;
            this.ZombieSpawnConfig = level.ZombieSpawnConfig;
            this.LootConfig = level.LootConfig;
        }

        #endregion

        #region 初始化相关

        public void OnLevelInitialization()
        {
            sunpoint.Value = 200;
            chosenCards = new List<Card>();
            // 网格数据
            for (int x = -1; x <= MapConfig.size.x; ++x)
            {
                for (int y = -1; y <= MapConfig.size.y; ++y)
                {
                    if (ReferenceModel.Get.GroundTilemap.HasTile(new Vector3Int(x, y, 0)) ||
                        ReferenceModel.Get.BoundTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        if (ReferenceModel.Get.DirtTilemap.HasTile(new Vector3Int(x, y, 0)))
                        {
                            CellGrid[x, y] = new Cell { cellState = Cell.CellState.HaveDirt };
                        }
                        else
                        {
                            CellGrid[x, y] = new Cell { cellState = Cell.CellState.HaveStone };
                        }
                    }
                    else
                    {
                        CellGrid[x, y] = new Cell { cellState = Cell.CellState.Empty };
                    }
                }
            }
        }

        public void OnExiting()
        {
            CellGrid = new DynaGrid<Cell>();
            // ↓不能换引用，不然已经Register了的东西会出错
            // sunpoint = new BindableProperty<int>();
            chosenCards.Clear();
            
            level = null;
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
            CellGrid = new DynaGrid<Cell>();
        }
    }
}