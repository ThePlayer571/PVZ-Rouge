using System;
using System.Collections.Generic;
using System.Linq;

namespace TPL.PVZR.Core.Random
{
    public sealed class DeterministicRandom
    {
        # region 核心实现(我也看不懂的)

        // 随机算法状态
        private ulong _state0;
        private ulong _state1;

        /// <summary>
        /// 使用指定种子初始化（64位）
        /// </summary>
        public DeterministicRandom(ulong seed)
        {
            // 使用SplitMix64算法初始化状态
            _state0 = SplitMix64(ref seed);
            _state1 = SplitMix64(ref seed);
        }

        /// <summary>
        /// 使用时间相关种子初始化
        /// </summary>
        public DeterministicRandom() : this((ulong)DateTime.Now.Ticks)
        {
        }

        /// <summary>
        /// 使用指定种子初始化
        /// </summary>
        public DeterministicRandom(int seed) : this((ulong)seed)
        {
        }

        /// <summary>
        /// 生成下一个随机数（0 ~ UInt64.MaxValue）
        /// </summary>
        public ulong NextUnsigned()
        {
            ulong s1 = _state0;
            ulong s0 = _state1;
            _state0 = s0;
            s1 ^= s1 << 23;
            _state1 = s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26);
            return _state1 + s0;
        }

        #endregion

        #region 实用方法

        /// <summary>
        /// 生成区间内的整数 [min, max)
        /// </summary>
        public int Range(int min, int max)
        {
            if (min >= max) throw new ArgumentException();
            return (int)(Value * (max - min)) + min;
        }

        /// <summary>
        /// 生成区间内的浮点数 [min, max]
        /// </summary>
        public float Range(float min, float max)
        {
            if (min > max) throw new ArgumentException();
            return (float)Value * (max - min) + min;
        }

        /// <summary>
        /// 生成区间内的浮点数 [0.0, 1.0)
        /// </summary>
        public double Value => (NextUnsigned() >> 11) * (1.0 / (1ul << 53));

        /// <summary>
        /// 生成随机布尔值
        /// </summary>
        public bool NextBool()
        {
            return (NextUnsigned() & 1) == 1;
        }

        /// <summary>
        /// 从集合中随机选取指定数量的元素（无重复）
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="count">选取数量（需非负）</param>
        /// <exception cref="ArgumentNullException">源集合为空</exception>
        /// <exception cref="ArgumentOutOfRangeException">数量为负</exception>
        /// 
        public IEnumerable<T> RandomSubset<T>(IEnumerable<T> source, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "数量不能为负");

            var list = source.ToList();
            if (count == 0 || list.Count == 0)
                return Enumerable.Empty<T>();
            if (count >= list.Count)
                return Shuffle(list);

            // 部分洗牌算法（Fisher-Yates 优化版）
            for (int i = 0; i < count; i++)
            {
                int j = Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list.Take(count);
        }

        /// <summary>
        /// 从集合中随机选择一个元素
        /// </summary>
        public T RandomChoose<T>(IEnumerable<T> source)
        {
            var list = source?.ToList() ?? throw new ArgumentNullException(nameof(source));
            if (list.Count == 0) throw new InvalidOperationException("集合不能为空");
            return list[Range(0, list.Count)];
        }

        /// <summary>
        /// 从集合中随机移除并返回一个元素
        /// </summary>
        /// <param name="source">可变的集合（需支持按索引移除）</param>
        /// <exception cref="ArgumentNullException">集合为 null</exception>
        /// <exception cref="InvalidOperationException">集合为空</exception>
        public T RandomPop<T>(IList<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Count == 0) throw new InvalidOperationException("集合不能为空");

            int index = Range(0, source.Count);
            T item = source[index];
            source.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Fisher-Yates洗牌算法
        /// </summary>
        public IList<T> Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var list = source.ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }

            return list;
        }

        #endregion

        #region 状态管理

        /// <summary>
        /// 当前随机状态
        /// </summary>
        public struct State
        {
            public ulong state0;
            public ulong state1;

            public State(ulong seed)
            {
                state0 = SplitMix64(ref seed);
                state1 = SplitMix64(ref seed);
            }
        }

        /// <summary>
        /// 保存当前状态
        /// </summary>
        public State SaveState()
        {
            return new State { state0 = _state0, state1 = _state1 };
        }

        /// <summary>
        /// 恢复先前状态
        /// </summary>
        public void RestoreState(State state)
        {
            _state0 = state.state0;
            _state1 = state.state1;
        }

        public void RestoreState(ulong seed)
        {
            RestoreState(new State(seed));
        }

        #endregion

        #region 私有方法

        private static ulong SplitMix64(ref ulong x)
        {
            ulong z = (x += 0x9E3779B97F4A7C15);
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EB;
            return z ^ (z >> 31);
        }

        #endregion
    }
}