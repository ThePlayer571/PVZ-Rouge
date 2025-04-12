using System;
using UnityEngine;

namespace TPL.PVZR.Gameplay.Class.MazeMap.Core
{
    public struct MazePos : IEquatable<MazePos>
    {
        public MazePos(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public int row;
        public int col;

        
        // 转换
        public Vector3Int ToTileMapPos(int scaler = 1)
        {
            return new Vector3Int(this.col, this.row, 0)*scaler;
        }
            
        
        
        // 《基于需求写的》
        public static MazePos operator +(MazePos a, ValueTuple<int, int> b)
        {
            return new MazePos(a.row + b.Item1, a.col + b.Item2);
        }

        public static ValueTuple<int, int> operator -(MazePos a, MazePos b)
            // 测试的时候要用
        {
            return (a.row - b.row, a.col - b.col);
        }

        // deepseek
        // 重载 == 运算符
        public static bool operator ==(MazePos a, MazePos b) => a.row == b.row && a.col == b.col;

        // 重载 != 运算符
        public static bool operator !=(MazePos a, MazePos b) => !(a == b);

        // 实现 IEquatable<MazePos>
        public bool Equals(MazePos other) => this == other;

        // 重写 object.Equals
        public override bool Equals(object obj) => obj is MazePos pos && this == pos;

        // 重写 GetHashCode（确保哈希一致性）
        public override int GetHashCode() => HashCode.Combine(row, col);

    }
}