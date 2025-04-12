using System.Collections.Generic;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Core.Extensions;
using TPL.PVZR.Core.Save.Modules;
using TPL.PVZR.Gameplay.Class.Levels;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using TPL.PVZR.Gameplay.ViewControllers.InMazeMap;
using UnityEngine;
using UnityEngine.Tilemaps;
using MapConfig = TPL.PVZR.Gameplay.Class.MazeMap.Core.MapConfig;

namespace TPL.PVZR.Gameplay.Class.MazeMap.MazeMaps
{
    public class DaveLawn : Core.MazeMap
    {
        public DaveLawn(MazeMapSaveData data) : base(data)
        {
        }

        public override MapConfig MapConfig { get; } = new MapConfigDaveLawn();
        public override LevelConfig LevelConfig { get; } = new LevelConfigDaveLawn();

        public override GameObject GenerateMazeMapGO()
        {
            var _ResLoader = ResLoader.Allocate();
            var mazeMapGrid = _ResLoader
                .LoadSync<GameObject>(Mazemap_grid_prefab.BundleName, Mazemap_grid_prefab.MazeMap_Grid).Instantiate();
            #region 设置mazeMapGrid

            

            // 预制数据
            var Ground = mazeMapGrid.transform.Find("Ground").GetComponent<Tilemap>();
            var Spots = mazeMapGrid.transform.Find("Spots").GetComponent<Tilemap>();
            var RoadTile = _ResLoader.LoadSync<Tile>(Mapdirt_asset.BundleName, Mapdirt_asset.MapDirt);
            var GroundTile = _ResLoader.LoadSync<Tile>(Mapgrass_asset.BundleName, Mapgrass_asset.MapGrass);
            var TombGO =
                _ResLoader.LoadSync<GameObject>(Mazemap_tomb_prefab.BundleName, Mazemap_tomb_prefab.MazeMap_Tomb);
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

                // 生成spot
                if (fromNode.carrySpot)
                {
                    var go = TombGO.Instantiate(
                        Spots.CellToLocal(fromNode.mazePos.ToTileMapPos(2)) + new Vector3(0,0.18f,0),
                        Quaternion.identity,
                        Spots.transform).GetComponent<Tomb>();
                    go.Init(fromNode);
                }
            }
            #endregion

            _MazeMapGirdGO = mazeMapGrid;
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