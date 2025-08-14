using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QFramework;
using UnityEngine;

namespace TPL.PVZR.Tools
{
    public static class ListExtensions
    {
        /// <summary>
        /// 将 List 调整为指定大小。如果变大，则使用指定默认值填充；如果变小，则移除多余元素。
        /// </summary>
        public static void Resize<T>(this List<T> list, int size, T defaultValue = default)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size), "Size cannot be negative.");

            int current = list.Count;

            if (size < current)
            {
                list.RemoveRange(size, current - size);
            }
            else if (size > current)
            {
                if (size > list.Capacity)
                    list.Capacity = size; // 提前扩容避免重复申请内存

                list.AddRange(new T[size - current]);
                // 用循环替换 AddRange 以支持 defaultValue ≠ default(T)
                for (int i = current; i < size; i++)
                    list[i] = defaultValue;
            }
        }


        /// <summary>
        /// 移除并返回列表末尾的元素（类似Python的pop()）
        /// </summary>
        public static T Pop<T>(this List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) throw new InvalidOperationException("Cannot pop from empty list.");

            int lastIndex = list.Count - 1;
            T item = list[lastIndex];
            list.RemoveAt(lastIndex);
            return item;
        }

        /// <summary>
        /// 移除并返回指定索引处的元素（类似Python的pop(index)）
        /// </summary>
        public static T Pop<T>(this List<T> list, int index)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (index < 0 || index >= list.Count) throw new ArgumentOutOfRangeException(nameof(index));

            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
    }


    public static class Vector2IntExtensions
    {
        /// <summary>
        /// 返回上方（y+1，相同x）的新Vector2Int
        /// </summary>
        public static Vector2Int Up(this Vector2Int v)
        {
            return new Vector2Int(v.x, v.y + 1);
        }

        /// <summary>
        /// 返回下方（y-1，相同x）的新Vector2Int
        /// </summary>
        public static Vector2Int Down(this Vector2Int v)
        {
            return new Vector2Int(v.x, v.y - 1);
        }

        /// <summary>
        /// 返回左方（x-1，相同y）的新Vector2Int
        /// </summary>
        public static Vector2Int Left(this Vector2Int v)
        {
            return new Vector2Int(v.x - 1, v.y);
        }

        /// <summary>
        /// 返回右方（x+1，相同y）的新Vector2Int
        /// </summary>
        public static Vector2Int Right(this Vector2Int v)
        {
            return new Vector2Int(v.x + 1, v.y);
        }
    }

    public static class Vector2Extensions
    {
        /// <summary>
        /// 返回 vector 顺时针旋转 angleDegrees 度后的新 Vector2。
        /// </summary>
        /// <param name="vector">原始向量</param>
        /// <param name="angleDegrees">旋转角度（度），正值为顺时针</param>
        /// <returns>旋转后的 Vector2</returns>
        public static Vector2 Rotate(this Vector2 vector, float angleDegrees)
        {
            float rad = angleDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(
                vector.x * cos + vector.y * sin,
                -vector.x * sin + vector.y * cos
            );
        }
    }

    public static class LinqExtensions
    {
        // 兼容老环境自定义 MinBy
        public static TSource MinBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
            where TKey : IComparable<TKey>
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");
            var min = e.Current;
            var minKey = selector(min);
            while (e.MoveNext())
            {
                var key = selector(e.Current);
                if (key.CompareTo(minKey) < 0)
                {
                    min = e.Current;
                    minKey = key;
                }
            }

            return min;
        }

        // 兼容老环境自定义 MaxBy
        public static TSource MaxBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
            where TKey : IComparable<TKey>
        {
            using var e = source.GetEnumerator();
            if (!e.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");
            var max = e.Current;
            var maxKey = selector(max);
            while (e.MoveNext())
            {
                var key = selector(e.Current);
                if (key.CompareTo(maxKey) > 0)
                {
                    max = e.Current;
                    maxKey = key;
                }
            }

            return max;
        }
    }

    public static class Physics2DExtensions
    {
        /// <summary>
        /// 从起点沿方向发射射线，返回第一个阻挡物体的点；若无阻挡则返回最大距离点。
        /// </summary>
        /// <param name="origin">射线起点</param>
        /// <param name="direction">射线方向（无需单位化，内部自动归一）</param>
        /// <param name="maxDistance">最大检测距离</param>
        /// <param name="blockingLayer">阻挡层 LayerMask</param>
        /// <returns>命中点或最大距离点</returns>
        public static Vector2 GetRaycastEndPoint(Vector2 origin, Vector2 direction, float maxDistance,
            LayerMask blockingLayer)
        {
            var hit = Physics2D.Raycast(origin, direction.normalized, maxDistance, blockingLayer);
            if (hit.collider)
            {
                return hit.point;
            }
            else
            {
                return origin + direction.normalized * maxDistance;
            }
        }
    }

    public static class ActionTask
    {
        /// <summary>
        /// 返回一个等待指定帧数后完成的Task。
        /// 需要在Unity环境下配合MonoBehaviour的Update或使用第三方如ActionKit等帧调度器。
        /// </summary>
        public static Task WaitForFrame(int frameCount)
        {
            var tcs = new TaskCompletionSource<bool>();
            ActionKit.DelayFrame(frameCount, () => tcs.SetResult(true)).StartGlobal();

            return tcs.Task;
        }
    }
}