using System;
using TPL.PVZR.Gameplay.Class.MazeMap.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TPL.PVZR.Core.Extensions
{
    using MazeGrid = Matrix<Node>;
    public static class Extensions
    {
        public static bool InRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }
        public static Node Get(this MazeGrid mazeGrid, MazePos mazePos)
        {
            // Exception use DeepSeek
            // 1. 检查 mazeGrid 是否为 null
            if (mazeGrid == null)
            {
                throw new ArgumentNullException(nameof(mazeGrid), "MazeGrid 不能为 null。");
            }

            // 2. 检查 mazePos 是否为 null
            // if (mazePos == null)
            // {
            //     throw new ArgumentNullException(nameof(mazePos), "MazePos 不能为 null。");
            // }

            // 3. 检查 mazePos 的行和列是否在合法范围内
            try
            {
                return mazeGrid[mazePos.row, mazePos.col];
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException(
                    $"MazePos [{mazePos.row}, {mazePos.col}] 超出 MazeGrid 的有效范围。",
                    ex
                );
            }
            catch (Exception ex)
            {
                // 捕获其他可能的异常（如 mazeGrid 未正确初始化）
                throw new InvalidOperationException("获取 MazeGrid 节点时发生错误。", ex);
            }
        }

        /// <summary>
    /// 使用指定Tile填充矩形区域
    /// </summary>
    /// <param name="tilemap">目标Tilemap</param>
    /// <param name="bounds">填充区域（BoundsInt坐标系）</param>
    /// <param name="tile">要设置的Tile</param>
    public static void Fill(this Tilemap tilemap, BoundsInt bounds, TileBase tile)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap cannot be null");
            return;
        }

        if (tile == null)
        {
            Debug.LogWarning("Fill tile is null, operation skipped");
            return;
        }

        // 计算有效填充区域
        var validBounds = ValidateBounds(bounds);
        
        // 创建并填充数组
        int total = validBounds.size.x * validBounds.size.y * validBounds.size.z;
        TileBase[] tiles = new TileBase[total];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = tile;
        }

        // 批量设置
        tilemap.SetTilesBlock(validBounds, tiles);
    }

    /// <summary>
    /// 使用指定Tile填充任意两点定义的区域
    /// </summary>
    /// <param name="tilemap">目标Tilemap</param>
    /// <param name="from">起始坐标</param>
    /// <param name="to">结束坐标</param>
    /// <param name="tile">要设置的Tile</param>
    public static void Fill(this Tilemap tilemap, Vector3Int from, Vector3Int to, TileBase tile)
    {
        if (tilemap == null) return;

        // 确定区域边界
        Vector3Int min = new Vector3Int(
            Mathf.Min(from.x, to.x),
            Mathf.Min(from.y, to.y),
            Mathf.Min(from.z, to.z)
        );

        Vector3Int max = new Vector3Int(
            Mathf.Max(from.x, to.x),
            Mathf.Max(from.y, to.y),
            Mathf.Max(from.z, to.z)
        );

        // 转换为BoundsInt
        BoundsInt bounds = new BoundsInt
        {
            min = min,
            max = max + Vector3Int.one // BoundsInt的max是排他的
        };

        tilemap.Fill(bounds, tile);
    }

    /// <summary>
    /// 验证并修正Bounds有效性
    /// </summary>
    private static BoundsInt ValidateBounds(BoundsInt original)
    {
        // 创建新变量避免修改原结构
        Vector3Int newPosition = original.position;
        Vector3Int newSize = original.size;

        // 修正 X 轴
        if (newSize.x < 0)
        {
            newPosition.x += newSize.x;
            newSize.x = -newSize.x;
        }

        // 修正 Y 轴
        if (newSize.y < 0)
        {
            newPosition.y += newSize.y;
            newSize.y = -newSize.y;
        }

        // 修正 Z 轴
        if (newSize.z < 0)
        {
            newPosition.z += newSize.z;
            newSize.z = -newSize.z;
        }

        // 创建新的 BoundsInt
        return new BoundsInt(newPosition, newSize);
    }
    }
}