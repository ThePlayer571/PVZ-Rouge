using QAssetBundle;
using QFramework;
using TPL.PVZR.Classes.DataClasses_InLevel;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Others.LevelScene;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Helpers.New.Methods
{
    public static class LevelMatrixHelper
    {
        // 这个逻辑不能转到System里面，因为Level初始化阶段很紧，System找不到合适的时机
        public static Matrix<Cell> BakeLevelMatrix(LevelTilemapController LevelTileMap, ILevelData levelData)
        {
            var LevelMatrix =
                new Matrix<Cell>(levelData.MapSize.x, levelData.MapSize.y);
            for (int x = 0; x < levelData.MapSize.x; x++)
            {
                for (int y = 0; y < levelData.MapSize.y; y++)
                {
                    var pos = new Vector3Int(x, y, 0);
                    var cell = new Cell(x, y);
                    LevelMatrix[x, y] = cell;
                    if (LevelTileMap.Bound.HasTile(pos)) cell.TileState = TileState.Bound;
                    else if (LevelTileMap.Ground.HasTile(pos)) cell.TileState = TileState.Barrier;
                    else if (LevelTileMap.Dirt.HasTile(pos)) cell.TileState = TileState.Dirt;
                    else if (LevelTileMap.Ladder.HasTile(pos)) cell.TileState = TileState.Ladder;
                    else cell.TileState = TileState.Empty;
                }
            }

            return LevelMatrix;
        }

        // 缺乏优化：多次SetTile性能不好
        public static void SetDebugTiles(Matrix<Cell> LevelMatrix, Tilemap DebugTilemap)
        {
            var resLoader = ResLoader.Allocate();
            var dirt = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugDirt);
            var barrier = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugBarrier);
            var climbable = resLoader.LoadSync<Tile>(Leveldebug.BundleName, Leveldebug.DebugLadder);
            for (int x = 0; x < LevelMatrix.Rows; x++)
            {
                for (int y = 0; y < LevelMatrix.Columns; y++)
                {
                    if (LevelMatrix[x, y].TileState == TileState.Dirt)
                        DebugTilemap.SetTile(new Vector3Int(x, y, 0), dirt);
                    else if (LevelMatrix[x, y].TileState == TileState.Barrier)
                        DebugTilemap.SetTile(new Vector3Int(x, y, 0), barrier);
                    else if (LevelMatrix[x, y].TileState == TileState.Ladder)
                        DebugTilemap.SetTile(new Vector3Int(x, y, 0), climbable);
                }
            }
        }
    }
}