using System;
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

    /// <summary>权重变化函数委托</summary>
    public delegate float WeightModifierDelegate<in TGenerateInfo>(TGenerateInfo info);

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
        
        /// <summary>权重变化函数</summary>
        private WeightModifierDelegate<TGenerateInfo> weightModifier;

        #endregion

        #region Constructor

        public RandomPool(IReadOnlyList<TGenerateInfo> infos, float value, DeterministicRandom random)
        {
            availableInfos = infos.ToList();
            remainingValue = value;
            allowedValueOffset = value * ValueOffsetRatio;
            Random = random;
            
            // 检查初始状态是否已完成
            CheckInitialFinishedState();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 设置权重变化函数
        /// </summary>
        /// <param name="modifier">权重变化函数，接收物品信息和原始权重，返回新的权重</param>
        public void SetWeightModifier(WeightModifierDelegate<TGenerateInfo> modifier)
        {
            weightModifier = modifier;
        }

        /// <summary>
        /// 清除权重变化函数
        /// </summary>
        public void ClearWeightModifier()
        {
            weightModifier = null;
        }

        /// <summary>
        /// 获取物品的有效权重（应用变化函数后）
        /// </summary>
        /// <param name="info">物品信息</param>
        /// <returns>有效权重</returns>
        private float GetEffectiveWeight(TGenerateInfo info)
        {
            float originalWeight = info.Weight;
            if (weightModifier != null)
            {
                return weightModifier(info);
            }
            return originalWeight;
        }

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
            var totalWeight = availableInfos.Sum(item => GetEffectiveWeight(item));
            var chosenWeight = Random.Value * totalWeight;

            var currentWeight = 0f;
            foreach (var info in availableInfos)
            {
                currentWeight += GetEffectiveWeight(info);
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

        /// <summary>检查初始状态是否已完成</summary>
        private void CheckInitialFinishedState()
        {
            // 如果没有可用项目，直接标记为完成
            if (availableInfos.Count == 0)
            {
                IsFinished = true;
                return;
            }
            
            // 如果剩余价值太少，无法承受任何物品，标记为完成
            if (remainingValue < allowedValueOffset)
            {
                IsFinished = true;
                return;
            }
            
            // 检查是否存在任何可承受的物品
            bool hasAffordableItem = availableInfos.Any(info => IsInfoAffordable(info));
            if (!hasAffordableItem)
            {
                IsFinished = true;
            }
        }

        #endregion
    }
}