using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TPL.PVZR.Tools
{
    // by deepseek
    /// <summary>
    /// 泛型矩阵类，支持二维数据存储和 LINQ 操作
    /// </summary>
    /// <typeparam name="T">矩阵元素类型</typeparam>
    public sealed class Matrix<T> : IEnumerable<T>, ICloneable
    {
        private readonly T[,] _data;

        #region 公有

        #region 构造函数

        /// <summary>
        /// 初始化矩阵
        /// </summary>
        /// <param name="data">二维数据源（自动深拷贝）</param>
        public Matrix(T[,] data)
        {
            // 深拷贝以防止外部修改
            _data = (T[,])data.Clone();
        }

        /// <summary>
        /// 创建指定行列的空矩阵（元素为 default(T)）
        /// </summary>
        public Matrix(int rows, int columns)
        {
            ValidateDimensions(rows, columns);
            _data = new T[rows, columns];
        }

        #endregion

        #region 公有属性

        /// <summary>
        /// 矩阵行数
        /// </summary>
        public int Rows => _data.GetLength(0);

        /// <summary>
        /// 矩阵列数
        /// </summary>
        public int Columns => _data.GetLength(1);

        #endregion

        #region 公有方法

        /// <summary>
        /// 用指定值填充整个矩阵
        /// </summary>
        public void Fill(T value)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _data[i, j] = value;
                }
            }
        }

        /// <summary>
        /// 用指定值填充指定区域
        /// </summary>
        /// <param name="startRow">起始行（包含）</param>
        /// <param name="startCol">起始列（包含）</param>
        /// <param name="endRow">结束行（包含）</param>
        /// <param name="endCol">结束列（包含）</param>
        /// <param name="value">填充值</param>
        public void Fill(int startRow, int startCol, int endRow, int endCol, T value)
        {
            // 自动识别起止点
            int minRow = Math.Min(startRow, endRow);
            int maxRow = Math.Max(startRow, endRow);
            int minCol = Math.Min(startCol, endCol);
            int maxCol = Math.Max(startCol, endCol);

            if (minRow < 0 || maxRow >= Rows)
                throw new ArgumentOutOfRangeException("行索引越界");
            if (minCol < 0 || maxCol >= Columns)
                throw new ArgumentOutOfRangeException("列索引越界");

            for (int i = minRow; i <= maxRow; i++)
            {
                for (int j = minCol; j <= maxCol; j++)
                {
                    _data[i, j] = value;
                }
            }
            
        }

        /// <summary>
        /// 用指定值填充指定区域（使用Vector2Int，自动识别起止点大小）
        /// </summary>
        /// <param name="start">第一个点</param>
        /// <param name="end">第二个点</param>
        /// <param name="value">填充值</param>
        public void Fill(Vector2Int start, Vector2Int end, T value)
        {
            Fill(start.x, start.y, end.x, end.y, value);
        }

        /// <summary>
        /// 用生成函数填充矩阵
        /// </summary>
        /// <param name="generator">接收行列索引，返回填充值的函数</param>
        public void Fill(Func<int, int, T> generator)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _data[i, j] = generator(i, j);
                }
            }
        }

        /// <summary>
        /// 获取指定位置周围3x3区域内的元素（包括自身）
        /// </summary>
        /// <param name="row">中心行索引</param>
        /// <param name="column">中心列索引</param>
        /// <returns>可枚举的邻居元素集合</returns>
        public IEnumerable<T> GetNeighbors(int row, int column, bool includeSelf = true)
        {
            ValidateIndex(row, column);

            // 3x3邻域的行列偏移量
            int[] offsets = { -1, 0, 1 };

            foreach (int iOffset in offsets)
            {
                int neighborRow = row + iOffset;
                if (neighborRow < 0 || neighborRow >= Rows) continue;

                foreach (int jOffset in offsets)
                {
                    int neighborCol = column + jOffset;
                    if (neighborCol < 0 || neighborCol >= Columns) continue;

                    // 如果不包含自身，跳过中心点
                    if (!includeSelf && iOffset == 0 && jOffset == 0) continue;

                    yield return _data[neighborRow, neighborCol];
                }
            }
        }

        /// <summary>
        /// 获取指定位置直接相邻的四格（上下左右）
        /// </summary>
        /// <param name="row">中心行索引</param>
        /// <param name="column">中心列索引</param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns>可枚举的直接邻居元素集合</returns>
        public IEnumerable<T> GetDirectNeighbors(int row, int column, bool includeSelf = true)
        {
            ValidateIndex(row, column);

            // 四个方向的偏移量：上、下、左、右
            int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };

            // 如果包含自身，先返回中心点
            if (includeSelf)
            {
                yield return _data[row, column];
            }

            for (int i = 0; i < 4; i++)
            {
                int neighborRow = row + directions[i, 0];
                int neighborCol = column + directions[i, 1];

                if (neighborRow >= 0 && neighborRow < Rows && 
                    neighborCol >= 0 && neighborCol < Columns)
                {
                    yield return _data[neighborRow, neighborCol];
                }
            }
        }


        /// <summary>
        /// 二维索引器
        /// </summary>
        public T this[int row, int column]
        {
            get
            {
                ValidateIndex(row, column);
                return _data[row, column];
            }
            set
            {
                ValidateIndex(row, column);
                _data[row, column] = value;
            }
        }

        /// <summary>
        /// 遍历所有元素及其索引，执行指定操作
        /// </summary>
        /// <param name="action">接收行、列索引和值的操作</param>
        public void Foreach(Action<int, int, T> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    action(i, j, _data[i, j]);
                }
            }
        }

        #endregion

        #endregion

        //

        /// <summary>
        /// 实现 IEnumerable<T> 以支持 LINQ
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _data) yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 按行迭代（保留二维结构）
        /// </summary>
        public IEnumerable<IEnumerable<T>> IterateByRow()
        {
            for (int i = 0; i < Rows; i++)
            {
                yield return Enumerable.Range(0, Columns).Select(j => _data[i, j]);
            }
        }

        /// <summary>
        /// 按列迭代（保留二维结构）
        /// </summary>
        public IEnumerable<IEnumerable<T>> IterateByColumn()
        {
            for (int j = 0; j < Columns; j++)
            {
                yield return Enumerable.Range(0, Rows).Select(i => _data[i, j]);
            }
        }

        // 验证索引有效性
        private void ValidateIndex(int row, int column)
        {
            if (row < 0 || row >= Rows)
                throw new IndexOutOfRangeException($"行索引 {row} 越界，有效范围 [0, {Rows - 1}]");
            if (column < 0 || column >= Columns)
                throw new IndexOutOfRangeException($"列索引 {column} 越界，有效范围 [0, {Columns - 1}]");
        }

        private static void ValidateDimensions(int rows, int columns)
        {
            if (rows <= 0)
                throw new ArgumentOutOfRangeException(nameof(rows), "行数必须大于0");
            if (columns <= 0)
                throw new ArgumentOutOfRangeException(nameof(columns), "列数必须大于0");
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}