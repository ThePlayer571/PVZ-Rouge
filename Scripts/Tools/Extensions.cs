using System;
using System.Collections.Generic;
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
}