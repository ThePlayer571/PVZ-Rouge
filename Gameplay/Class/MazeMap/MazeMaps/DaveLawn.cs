using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class.Levels;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Gameplay.Class.MazeMap.MazeMaps
{
    public class DaveLawn : MazeMap
    {
        public DaveLawn()
        {
            CreateMain();
            MazeMapGirdGO = GenerateMazeMapGO();
        }

        public override MapConfig MapConfig { get; } = new MapConfigDaveLawn();
        public override LevelConfig LevelConfig { get; } = new LevelConfigDaveLawn();

        public override GameObject GenerateMazeMapGO()
        {
            // 预制数据
            var _ResLoader = ResLoader.Allocate();
            var mazeMapGrid = _ResLoader.LoadSync<GameObject>("MazeMapGrid").Instantiate();
            var Ground = mazeMapGrid.transform.Find("Ground").GetComponent<Tilemap>();
            var Spots = mazeMapGrid.transform.Find("Spots").GetComponent<Tilemap>();
            var RoadTile = _ResLoader.LoadSync<Tile>("MapDirt");
            var GroundTile = _ResLoader.LoadSync<Tile>("MapGrass");
            var SpotTile = _ResLoader.LoadSync<Tile>("MapTomb");
            // 设置背景
            var bounds = new BoundsInt(0, 0, 0, 4 * (MapConfig.colCount - 1) + 1, 4 * (MapConfig.rowCount - 1) + 1, 1);
            Ground.Fill(bounds, GroundTile);
            // 设置路径
            foreach (var fromNode in mazeGrid)
            {
                // from to road
                if (fromNode.toNode.Any())
                {
                    foreach (var toNode in fromNode.toNode)
                    {
                        Ground.Fill(fromNode.mazePos.ToTileMapPos(2), toNode.mazePos.ToTileMapPos(2), RoadTile);
                    }
                }
                // spot
                if (fromNode.carrySpot)
                {
                    Spots.SetTile(fromNode.mazePos.ToTileMapPos(2),SpotTile );
                }
            }
            // for (int row = 0; row < MapConfig.rowCount; row++)
            // {
            //     Vector3Int[] positions = new Vector3Int[MapConfig.colCount];
            //     TileBase[] tiles = new TileBase[MapConfig.colCount];
            //     Ground.SetTiles(positions, tiles);
            // }

            return mazeMapGrid;
        }
    }

    public class MapConfigDaveLawn : MapConfig
    {
        public override int rowCount { get; } = 5;
        public override int colCount { get; } = 11;
        protected override int levelCount { get; } = 5;
    }

    public class LevelConfigDaveLawn : LevelConfig
    {
        public override List<LevelIdentifier> EasyLevels { get; } = new() { LevelIdentifier.DaveHouse };
        public override List<LevelIdentifier> NormalLevels { get; } = new() { LevelIdentifier.DaveHouse };
        public override List<LevelIdentifier> HardLevels { get; } = new() { LevelIdentifier.DaveHouse };
        public override List<int> EasyLevelsCount { get; }
        public override List<int> NormalLevelsCount { get; }
        public override List<int> HardLevelsCount { get; }
    }

}