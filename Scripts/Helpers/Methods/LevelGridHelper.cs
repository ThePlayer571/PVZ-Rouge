using TPL.PVZR.Tools;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Helpers.New.Methods
{
    public static class LevelGridHelper
    {
        private static Tilemap CriterionTilemap => ReferenceHelper.LevelTilemap.Ground;

        public static Vector2Int WorldToCell(Vector2 worldPos)
        {
            var cellPos = CriterionTilemap.WorldToCell(worldPos);
            return new Vector2Int(cellPos.x, cellPos.y);
        }

        public static Vector2 CellToWorld(Vector2Int cellPos)
        {
            Vector3 worldPos = CriterionTilemap.GetCellCenterWorld(new Vector3Int(cellPos.x, cellPos.y, 0));
            return new Vector2(worldPos.x, worldPos.y);
        }

        public static Vector2 CellToWorldBottom(Vector2Int cellPos)
        {
            Vector3 cellWorldOrigin = CriterionTilemap.CellToWorld(new Vector3Int(cellPos.x, cellPos.y, 0));
            Vector3 cellSize = CriterionTilemap.cellSize;
            // 得到底部中心
            Vector3 bottomCenter = cellWorldOrigin + new Vector3(cellSize.x / 2f, 0, 0);
            return new Vector2(bottomCenter.x, bottomCenter.y);
        }
    }
}