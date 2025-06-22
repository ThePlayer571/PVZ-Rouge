using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.Level;
using TPL.PVZR.Classes.LevelStuff;
using TPL.PVZR.Core;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Helpers.Methods
{
    public static class LevelMatrixHelper
    {
        public static Matrix<Cell> BakeLevelMatrix(LevelTilemapController LevelTileMap, ILevelData levelData)
        {
            var LevelMatrix =
                new Matrix<Cell>(levelData.LevelDefinition.MapSize.x, levelData.LevelDefinition.MapSize.y);
            for (int x = 0; x < levelData.LevelDefinition.MapSize.x; x++)
            {
                for (int y = 0; y < levelData.LevelDefinition.MapSize.y; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var cell = new Cell(x, y);
                    LevelMatrix[x, y] = cell;
                    if (LevelTileMap.Bound.HasTile(pos)) cell.CellTileState = CellTileState.Bound;
                    else if (LevelTileMap.Ground.HasTile(pos)) cell.CellTileState = CellTileState.Barrier;
                    else if (LevelTileMap.Dirt.HasTile(pos)) cell.CellTileState = CellTileState.Dirt;
                    else cell.CellTileState = CellTileState.Empty;
                }
            }

            return LevelMatrix;
        }

        // TODO:  缺乏优化：多次SetTile性能不好
        public static void SetDebugTiles(Matrix<Cell> LevelMatrix, Tilemap DebugTilemap)
        {
            var resLoader = ResLoader.Allocate();
            var dirt = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugDirt);
            var barrier = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugBarrier);
            for (int x = 0; x < LevelMatrix.Rows; x++)
            {
                for (int y = 0; y < LevelMatrix.Columns; y++)
                {
                    if (LevelMatrix[x, y].CellTileState == CellTileState.Dirt)
                        DebugTilemap.SetTile(new Vector3Int(x, y, 0), dirt);
                    else if (LevelMatrix[x, y].CellTileState == CellTileState.Barrier)
                        DebugTilemap.SetTile(new Vector3Int(x, y, 0), barrier);
                }
            }
        }
    }
}