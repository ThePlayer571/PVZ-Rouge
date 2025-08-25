using System;
using System.Collections.Generic;
using UnityEngine;

namespace TPL.PVZR.Helpers.New.Methods
{
    public static class CellSelectHelper
    {
        /// <summary>
        /// 获取两个点之间的所有格子位置（包含起点和终点）
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="end">终点</param>
        /// <returns>两点间所有格子位置</returns>
        public static IList<Vector2Int> GetCellsBetween(Vector2Int start, Vector2Int end)
        {
            var result = new List<Vector2Int>();

            int x0 = start.x, y0 = start.y;
            int x1 = end.x, y1 = end.y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                result.Add(new Vector2Int(x0, y0));

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取指定中心点和半径范围内的所有格子位置（包含中心格子）
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">半径</param>
        /// <returns>范围内所有格子位置</returns>
        public static IList<Vector2Int> GetCellsInRadius(Vector2Int center, float radius)
        {
            var result = new List<Vector2Int>();
            int intRadius = Mathf.CeilToInt(radius);

            for (int x = center.x - intRadius; x <= center.x + intRadius; x++)
            {
                for (int y = center.y - intRadius; y <= center.y + intRadius; y++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    float distance = Vector2.Distance(center, cell);

                    if (distance <= radius)
                    {
                        result.Add(cell);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取两个点形成的矩形区域内的所有格子位置
        /// </summary>
        /// <param name="start">第一个点</param>
        /// <param name="end">第二个点</param>
        /// <returns>矩形区域内所有格子位置</returns>
        public static IList<Vector2Int> GetCellsInRect(Vector2Int start, Vector2Int end)
        {
            var result = new List<Vector2Int>();

            int minX = Mathf.Min(start.x, end.x);
            int maxX = Mathf.Max(start.x, end.x);
            int minY = Mathf.Min(start.y, end.y);
            int maxY = Mathf.Max(start.y, end.y);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }

            return result;
        }
    }
}