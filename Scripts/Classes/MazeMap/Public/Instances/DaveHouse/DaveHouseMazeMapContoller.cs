using System;
using System.Linq;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Models;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace TPL.PVZR.Classes.MazeMap.Instances.DaveHouse
{
    public class DaveHouseMazeMapController : MazeMapController
    {
        public DaveHouseMazeMapController(MazeMapData mazeMapData) : base(mazeMapData)
        {
        }

        public override void SetMazeMapTiles()
        {
            if (_PhaseModel.GamePhase != GamePhase.MazeMapInitialization)
                throw new Exception($"在不正确的时机SetMazeMapTiles：{_PhaseModel.GamePhase}");
            // 准备
            var resLoader = ResLoader.Allocate();
            var dirtTile = resLoader.LoadSync<Tile>(Mazemapdirttile_asset.BundleName, Mazemapdirttile_asset.MazeMapDirtTile);
            var grassTile = resLoader.LoadSync<Tile>(Mazemapgrasstile_asset.BundleName, Mazemapgrasstile_asset.MazeMapGrassTile);
            var GroundTilemap = GameObject.FindFirstObjectByType<Tilemap>();
            Matrix<Tile> tileMatrix = new(MazeMapData.mazeMatrix.Rows * 3, 3 * MazeMapData.mazeMatrix.Columns);
            //
            tileMatrix.Fill(grassTile);

            GroundTilemap.SetTilesBlock(
                new BoundsInt(new Vector3Int(0, 0, 0), new Vector3Int(tileMatrix.Rows, tileMatrix.Columns, 1)),
                tileMatrix.ToArray());
        }
    }
}