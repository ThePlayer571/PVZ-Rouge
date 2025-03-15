using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.Tilemaps;

namespace TPL.PVZR
{
    public interface ILevelModel : IModel, IInLevelSystem
    {
        // Build
        public ILevel level { get; }
        public void SetLevel(ILevel level);
        // ChooseCards
        public int maxCardCount { get; }

        // Gameplay
        public Dave Dave { get; }
        public Grid Grid { get; }
        public Tilemap GroundTilemap { get; }
        public Tilemap BoundTilemap { get; }
        public Tilemap DirtTilemap { get; }
        public DynaGrid<Cell> CellGrid { get; }
        public Seed[] seeds { get; }
        public Seed GetSeed(int seedIndex);
        public GameObject shovel { get; }
        public BindableProperty<int> sunpoint { get; }
    }
    public class LevelModel : AbstractModel, ILevelModel
    {
        /// <summary>
        /// 实现接口
        /// </summary>

        /// <summary>
        /// 数据
        /// </summary>
        // == Level
        public ILevel level { get; private set; }
        // == ChooseCard
        public int maxCardCount { get; private set; } = 4;
        // == Gameplay
        // 游戏数据
        public BindableProperty<int> sunpoint { get; private set; }

        public DynaGrid<Cell> CellGrid { get; private set; }
        // 游戏引用
        public Dave Dave { get; private set; }
        public Grid Grid { get; private set; }
        // 编程引用
        public Tilemap GroundTilemap { get; private set; }
        public Tilemap DirtTilemap { get; private set; }

        public Tilemap BoundTilemap { get; private set; }
        // UI引用

        public Seed[] seeds { get; private set; }
        public Seed GetSeed(int seedIndex)
        {
            foreach (var seed in seeds)
            {
                if (seed.seedIndex == seedIndex) return seed;
            }
            return seeds[0];
        }

        public GameObject shovel { get; private set; }




        protected override void OnInit()
        {
            sunpoint = new BindableProperty<int>();
            CellGrid = new DynaGrid<Cell>();
        }

        public void SetLevel(ILevel level)
        {
            this.level = level;
        }
        public void OnBuildingLevel()
        {
            sunpoint.Value = 100;
            // 引用
            Dave = Object.FindAnyObjectByType<Dave>();
            Grid = Object.FindAnyObjectByType<Grid>();
            GroundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
            BoundTilemap = GameObject.Find("Bound").GetComponent<Tilemap>();
            DirtTilemap = GameObject.Find("DirtNotice").GetComponent<Tilemap>();

            // 网格数据
            for (int x = -1; x <= level.size.x; ++x)
            {
                for (int y = -1; y <= level.size.y; ++y)
                {
                    if (GroundTilemap.HasTile(new Vector3Int(x, y, 0)) || BoundTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        if (DirtTilemap.HasTile(new Vector3Int(x, y, 0)))
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

        public void OnGameplay()
        {
            seeds = Object.FindObjectsByType<Seed>(FindObjectsSortMode.None);
            shovel = GameObject.Find("Shovel");
        }

        public void OnExitLevel()
        {
            Dave = null;
            Grid = null;
            GroundTilemap = null;
            BoundTilemap = null;
            seeds = null;
            shovel = null;
            CellGrid = new DynaGrid<Cell>();
            sunpoint = new BindableProperty<int>();

        }
    }
}