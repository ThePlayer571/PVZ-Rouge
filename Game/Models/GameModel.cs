using UnityEngine;
using QFramework;
using UnityEngine.Tilemaps;

namespace TPL.PVZR
{
    public interface IGameModel : IModel
    {
        // 引用
        public Dave Dave { get; }
        public Grid Grid { get; }
        public Tilemap GroundTilemap { get; }
        public Tilemap BoundTilemap { get; }
        public DynaGrid<Cell> CellGrid { get; }
        public Vector2Int levelStartConner { get; }
        public Vector2Int levelEndConner { get; }
        public Card[] cards { get; }
        public GameObject shovel { get; }
        public BindableProperty<int> sunpoint { get; set; }
        // public Vector3 SunpointTextPos { get; }
        

        //
        public void OnEnterGameSceneInit();
    }
    public class GameModel : AbstractModel, IGameModel
    {
        /// <summary>
        /// 实现接口
        /// </summary>
        public Dave Dave { get; private set; }
        public Grid Grid { get; private set; }

        /// <summary>
        /// 数据
        /// </summary>

        // 数据

        public BindableProperty<int> sunpoint { get; set; } = new();
        public Vector2Int levelStartConner { get; private set; }
        public Vector2Int levelEndConner { get; private set; }
        // 引用
        public Tilemap GroundTilemap { get; private set; }

        public Tilemap BoundTilemap { get; private set; }

        public Card[] cards { get; private set; }
        public GameObject shovel { get; private set; }

        public DynaGrid<Cell> CellGrid { get; private set; } = new DynaGrid<Cell>();



        protected override void OnInit()
        {
            
        }

        public void OnEnterGameSceneInit()
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
            levelStartConner = new Vector2Int(0, 0);
            levelEndConner = new Vector2Int(22, 10);
            for (int x = this.levelStartConner.x; x <= this.levelEndConner.x; ++x)
            {
                for (int y = this.levelStartConner.y; y <= this.levelEndConner.y; ++y)
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

    }
}