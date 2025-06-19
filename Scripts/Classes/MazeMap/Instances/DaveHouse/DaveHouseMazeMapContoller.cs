using System;
using System.Data;
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
            var dirtTile =
                resLoader.LoadSync<Tile>(Mazemapdirttile_asset.BundleName, Mazemapdirttile_asset.MazeMapDirtTile);
            var grassTile = resLoader.LoadSync<Tile>(Mazemapgrasstile_asset.BundleName,
                Mazemapgrasstile_asset.MazeMapGrassTile);
            var stoneTile = resLoader.LoadSync<Tile>(Mazemapstonetile_asset.BundleName,
                Mazemapstonetile_asset.MazeMapStoneTile);
            var GroundTilemap = GameObject.FindFirstObjectByType<Tilemap>();
            Matrix<Tile> tileMatrix = new(MazeMapData.mazeMatrix.Rows * 3 - 2, MazeMapData.mazeMatrix.Columns * 3 - 2);
            //
            tileMatrix.Fill(grassTile);
            //
            foreach (var node in MazeMapData.mazeMatrix)
            {
                if (node.level == -1) continue;
                var startPos = new Vector2Int(node.x, node.y) * 3;
                var neighbors = MazeMapData.adjacencyList[node];
                foreach (var neighbor in neighbors)
                {
                    var endPos = new Vector2Int(neighbor.x, neighbor.y) * 3;
                    tileMatrix.Fill(startPos, endPos, dirtTile);
                }
            }

            // 填充石头
            foreach (var node in MazeMapData.mazeMatrix)
            {
                if (node.isKey)
                {
                    tileMatrix[3 * node.x, 3 * node.y] = stoneTile;
                }
            }

            // 修正：Tilemap 需要列优先的数组，BoundsInt 的 size.x=列数，size.y=行数
            Tile[] ToArrayByColumn(Matrix<Tile> matrix)
            {
                var arr = new Tile[matrix.Rows * matrix.Columns];
                int idx = 0;
                for (int y = 0; y < matrix.Columns; y++)
                {
                    for (int x = 0; x < matrix.Rows; x++)
                    {
                        arr[idx++] = matrix[x, y];
                    }
                }

                return arr;
            }

            var bounds = new BoundsInt(0, 0, 0, tileMatrix.Rows, tileMatrix.Columns, 1);
            GroundTilemap.SetTilesBlock(bounds, ToArrayByColumn(tileMatrix));
        }
    }
}