using System.Collections.Generic;
using System.Linq;
using TPL.PVZR.Tools.Random;

namespace TPL.PVZR.Tools
{
    public interface IGenerateInfo<out TOutput>
    {
        float Value { get; }
        float Weight { get; }
        TOutput Output { get; }
    }

    public class RandomPool<TGenerateInfo, TOutput>
        where TGenerateInfo : class, IGenerateInfo<TOutput>
        where TOutput : class
    {
        #region Constants

        private const int MaxRetryCount = 100;
        private const float ValueOffsetRatio = 0.05f;

        #endregion

        #region Public Properties

        /// <summary>是否已完成生成（价值耗尽或无可用项）</summary>
        public bool IsFinished { get; private set; }

        #endregion

        #region Private Fields

        private readonly List<TGenerateInfo> availableInfos;
        private float remainingValue;
        private readonly float allowedValueOffset;

        #endregion

        #region Constructor

        public RandomPool(List<TGenerateInfo> infos, float totalValue)
        {
            availableInfos = infos.ToList();
            remainingValue = totalValue;
            allowedValueOffset = totalValue * ValueOffsetRatio;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取随机输出对象
        /// </summary>
        public TOutput GetRandomOutput()
        {
            if (availableInfos.Count == 0) return null;

            for (var tryCount = 0; tryCount < MaxRetryCount; tryCount++)
            {
                var selectedInfo = SelectRandomInfo();
                if (selectedInfo == null) break;

                if (IsInfoAffordable(selectedInfo))
                {
                    ConsumeValue(selectedInfo.Value);
                    return selectedInfo.Output;
                }
                else
                {
                    RemoveUnaffordableInfo(selectedInfo);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有剩余的输出对象
        /// </summary>
        public List<TOutput> GetAllRemainingOutputs()
        {
            List<TOutput> results = new List<TOutput>();
            
            while (!IsFinished)
            {
                var output = GetRandomOutput();
                if (output != null)
                {
                    results.Add(output);
                }
                else
                {
                    break;
                }
            }

            return results;
        }

        public List<TOutput> GetRandomOutputs(int maxCount)
        {
            List<TOutput> results = new List<TOutput>();
            
            for (int i = 0; i < maxCount && !IsFinished; i++)
            {
                var output = GetRandomOutput();
                if (output != null)
                {
                    results.Add(output);
                }
                else
                {
                    break;
                }
            }

            return results;
        }

        #endregion

        #region Private Methods

        /// <summary>按权重随机选择一个信息项</summary>
        private TGenerateInfo SelectRandomInfo()
        {
            var totalWeight = availableInfos.Sum(item => item.Weight);
            var randomWeight = RandomHelper.Default.Value * totalWeight;

            var accumulatedWeight = 0f;
            foreach (var info in availableInfos)
            {
                accumulatedWeight += info.Weight;
                if (accumulatedWeight >= randomWeight)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>检查信息项是否可承受（价值足够）</summary>
        private bool IsInfoAffordable(TGenerateInfo info)
        {
            return remainingValue - info.Value > -allowedValueOffset;
        }

        /// <summary>消耗价值并检查是否完成</summary>
        private void ConsumeValue(float valueToConsume)
        {
            remainingValue -= valueToConsume;
            if (remainingValue < allowedValueOffset)
            {
                IsFinished = true;
            }
        }

        /// <summary>移除不可承受的信息项</summary>
        private void RemoveUnaffordableInfo(TGenerateInfo info)
        {
            availableInfos.Remove(info);
            if (availableInfos.Count == 0)
            {
                IsFinished = true;
            }
        }

        #endregion
    }
}