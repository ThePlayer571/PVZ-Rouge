using UnityEngine;
using QFramework;
using UnityEngine.Tilemaps;

namespace TPL.PVZR
{
    public interface ILevelModel : IModel,IInLevelSystem
    {
        // 引用
        public Dave Dave { get; }
        public Grid Grid { get; }
        public Tilemap GroundTilemap { get; }
        public Tilemap BoundTilemap { get; }
        public DynaGrid<Cell> CellGrid { get; }
        public Card[] cards { get; }
        public GameObject shovel { get; }
        public BindableProperty<int> sunpoint { get;}
        public Level level { get; }
        public void SetLevel(Level level);
    }
    public class LevelModel : AbstractModel, ILevelModel
    {
        /// <summary>
        /// 实现接口
        /// </summary>

        /// <summary>
        /// 数据
        /// </summary>

        // 数据

        public BindableProperty<int> sunpoint { get; private set; }
        public DynaGrid<Cell> CellGrid { get; private set; }
        public Level level { get; private set; }
        // 游戏引用
        public Dave Dave { get; private set; }
        public Grid Grid { get; private set; }
        // 编程引用
        public Tilemap GroundTilemap { get; private set; }

        public Tilemap BoundTilemap { get; private set; }
        // UI引用

        public Card[] cards { get; private set; }
        public GameObject shovel { get; private set; }




        protected override void OnInit()
        {
            sunpoint = new BindableProperty<int>();
            CellGrid = new DynaGrid<Cell>();
        }

        public void SetLevel(Level level)
        {
            this.level = level;
        }
        public void OnEnterLevel()
        {
            sunpoint.Value = 100000;
            // 引用
            Dave = Object.FindAnyObjectByType<Dave>();
            Grid = Object.FindAnyObjectByType<Grid>();
            GroundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
            BoundTilemap = GameObject.Find("Bound").GetComponent<Tilemap>();
            cards = Object.FindObjectsByType<Card>(FindObjectsSortMode.None);
            shovel = GameObject.Find("Shovel");
                
            // 网格数据
            for (int x = 0; x <= level.size.x; ++x)
            {
                for (int y = 0; y <= level.size.y; ++y)
                {
                    if (GroundTilemap.HasTile(new Vector3Int(x, y, 0)) || BoundTilemap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        CellGrid[x, y] = new Cell { cellState = Cell.CellState.HaveTile };
                    }
                    else
                    {
                        CellGrid[x, y] = new Cell { cellState = Cell.CellState.Empty };
                    }
                }
            }
        }

        public void OnExitLevel()
        {
            Dave = null;
            Grid = null;
            GroundTilemap = null;
            BoundTilemap = null;
            cards = null;
            shovel = null;
            CellGrid = new DynaGrid<Cell>();
            sunpoint = new BindableProperty<int>();

        }
    }
}