using System;
using System.Collections.Generic;

namespace TPL.PVZR.Core
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
    }
}