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
        bool OnlyOnce => false;
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
        private readonly DeterministicRandom Random;

        #endregion

        #region Constructor

        public RandomPool(IReadOnlyList<TGenerateInfo> infos, float value, DeterministicRandom random)
        {
            availableInfos = infos.ToList();
            remainingValue = value;
            allowedValueOffset = value * ValueOffsetRatio;
            Random = random;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 获取随机输出对象
        /// </summary>
        public TOutput GetRandomOutput()
        {
            if (availableInfos.Count == 0)
            {
                IsFinished = true;
                return null;
            }

            for (var tryCount = 0; tryCount < MaxRetryCount; tryCount++)
            {
                var selectedInfo = SelectRandomInfo();
                if (selectedInfo == null) break;

                if (IsInfoAffordable(selectedInfo))
                {
                    ConsumeValue(selectedInfo.Value);
                    
                    // 如果该物品只能被抽取一次，则从可用列表中移除
                    if (selectedInfo.OnlyOnce)
                    {
                        availableInfos.Remove(selectedInfo);
                        if (availableInfos.Count == 0)
                        {
                            IsFinished = true;
                        }
                    }
                    
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
        /// 获取所���剩余的输出对象
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

        public List<TOutput> GetRandomOutputs(int count)
        {
            List<TOutput> results = new List<TOutput>();
            
            for (int i = 0; i < count && !IsFinished; i++)
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
            var chosenWeight = Random.Value * totalWeight;

            var currentWeight = 0f;
            foreach (var info in availableInfos)
            {
                currentWeight += info.Weight;
                if (currentWeight >= chosenWeight)
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
        private void ConsumeValue(float value)
        {
            remainingValue -= value;
            if (remainingValue < allowedValueOffset)
            {
                IsFinished = true;
            }
        }

        /// <summary>移除���可承受的信息项</summary>
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